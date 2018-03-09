using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Community.Office.AddinServer.Data;
using Community.Office.AddinServer.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Community.Office.AddinServer
{
    public static class Program
    {
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim();

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += OnCancelKeyPress;

            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine(Strings.GetString("program.assembly_info"), assembly.GetCustomAttribute<AssemblyProductAttribute>().Product, assembly.GetName().Version.ToString(3));
            Console.WriteLine(assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright?.Replace("\u00A9", "(c)"));
            Console.WriteLine();

            var assemblyName = Path.GetFileNameWithoutExtension(assembly.Location);

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(assembly.Location), assemblyName + ".json"), true, false)
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, assemblyName + ".json"), true, false)
                .AddCommandLine(args);

            try
            {
                var configuration = configurationBuilder.Build();
                var serverRoot = Path.GetFullPath(configuration["server-root"] ?? Environment.CurrentDirectory);

                if (!int.TryParse(configuration["server-port"], NumberStyles.None, CultureInfo.InvariantCulture, out var serverPort))
                {
                    serverPort = 44300;
                }

                var logFileValue = configuration["log-file"];
                var logFile = logFileValue != null ? Path.GetFullPath(logFileValue) : null;
                var certificateFile = Path.Combine(Path.GetDirectoryName(assembly.Location), "https.pfx");

                if (!File.Exists(certificateFile))
                {
                    throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("server.certificate.absent_file"), certificateFile));
                }

                void ConfigureKestrelAction(KestrelServerOptions kso)
                {
                    kso.Listen(IPAddress.Loopback, serverPort, lo => lo.UseHttps(new X509Certificate2(certificateFile)));
                    kso.Limits.KeepAliveTimeout = TimeSpan.FromHours(1);
                    kso.AddServerHeader = false;
                }

                var hostBuilder = new WebHostBuilder()
                    .ConfigureServices(sc => sc.Configure<ServerOptions>(so => so.LoggingFilePath = logFile))
                    .UseKestrel(ConfigureKestrelAction)
                    .UseWebRoot(serverRoot)
                    .UseContentRoot(serverRoot)
                    .UseStartup<Startup>();

                using (var host = hostBuilder.Build())
                {
                    host.Start();

                    var serverPortToken = serverPort == 80 ? string.Empty : string.Format(CultureInfo.InvariantCulture, ":{0}", serverPort);

                    Console.WriteLine(Strings.GetString("server.root_info"), serverRoot);
                    Console.WriteLine(Strings.GetString("server.address_info"), serverPortToken);
                    Console.WriteLine();

                    var applicationLifetime = host.Services.GetRequiredService<IApplicationLifetime>();

                    _cancellationTokenSource.Token.Register(state => ((IApplicationLifetime)state).StopApplication(), applicationLifetime);

                    applicationLifetime.ApplicationStopping.WaitHandle.WaitOne();
                }

                _resetEvent.Set();
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                var foregroundColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(Strings.GetString("program.error_message"), ex.Message);
                Console.ForegroundColor = foregroundColor;
                Console.WriteLine();
                Console.WriteLine(Strings.GetString("program.usage_message"), Path.GetFileName(assembly.Location));
                Console.WriteLine();
                Console.WriteLine(Strings.GetString("program.usage_arguments"));
                Console.WriteLine();
                Console.WriteLine(Strings.GetString("program.usage_argument_server_root"));
                Console.WriteLine(Strings.GetString("program.usage_argument_server_port"));
                Console.WriteLine(Strings.GetString("program.usage_argument_log_file"));
            }
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _resetEvent.Wait();

            e.Cancel = true;

            Console.CancelKeyPress -= OnCancelKeyPress;
        }
    }
}