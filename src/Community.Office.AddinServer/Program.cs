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
            Console.WriteLine(assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright);
            Console.WriteLine();

            var assemblyName = Path.GetFileNameWithoutExtension(assembly.Location);

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(assembly.Location), assemblyName + ".json"), true, false)
                .AddCommandLine(args);

            try
            {
                var configuration = configurationBuilder.Build();

                var serverRootValue = configuration["server-root"];
                var serverPortValue = configuration["server-port"];
                var x509FileValue = configuration["x509-file"];

                if (serverRootValue == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("program.undefined_parameter"), "server-root"));
                }

                var serverRoot = Path.GetFullPath(serverRootValue);
                var serverPort = 44300;

                if (serverPortValue != null)
                {
                    if (!int.TryParse(serverPortValue, NumberStyles.None, CultureInfo.InvariantCulture, out serverPort))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("program.invalid_parameter"), "server-port"));
                    }
                }

                var x509File = x509FileValue != null ?
                    Path.GetFullPath(x509FileValue) :
                    Path.Combine(Path.GetDirectoryName(assembly.Location), assemblyName + ".pfx");

                var x509Password = configuration["x509-pass"] ?? string.Empty;
                var logFileValue = configuration["log-file"];
                var logFile = logFileValue != null ? Path.GetFullPath(logFileValue) : null;
                var certificate = new X509Certificate2(x509File, x509Password);

                void ConfigureKestrelAction(KestrelServerOptions options)
                {
                    options.Limits.KeepAliveTimeout = TimeSpan.FromHours(1);
                    options.Listen(IPAddress.Loopback, serverPort, lo => lo.UseHttps(certificate));
                    options.AddServerHeader = false;
                }

                var host = new WebHostBuilder()
                    .ConfigureServices(sc => sc.Configure<ServerOptions>(lo => lo.LoggingFilePath = logFile))
                    .UseKestrel(ConfigureKestrelAction)
                    .UseWebRoot(serverRoot)
                    .UseContentRoot(serverRoot)
                    .UseStartup<Startup>()
                    .Build();

                using (host)
                {
                    host.Start();

                    var serverPortToken = serverPort == 80 ? string.Empty : string.Format(CultureInfo.InvariantCulture, ":{0}", serverPort);

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
        }
    }
}