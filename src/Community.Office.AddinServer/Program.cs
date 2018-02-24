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
        public static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine(assembly.GetCustomAttribute<AssemblyProductAttribute>().Product + " " + assembly.GetName().Version.ToString(3));
            Console.WriteLine();

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(assembly.Location), "settings.json"), true, false)
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

                var x509File = x509FileValue != null ? Path.GetFullPath(x509FileValue) : Path.Combine(Path.GetDirectoryName(assembly.Location), "certificate.pfx");
                var x509Password = configuration["x509-password"] ?? string.Empty;

                var logFileValue = configuration["log-file"];
                var logFile = logFileValue != null ? Path.GetFullPath(logFileValue) : null;

                var certificate = new X509Certificate2(x509File, x509Password);

                void ConfigureServices(IServiceCollection services)
                {
                    services.Configure<LoggingOptions>(lo => lo.File = logFile);
                }
                void ConfigureKestrel(KestrelServerOptions options)
                {
                    options.Limits.KeepAliveTimeout = TimeSpan.FromHours(1);
                    options.Listen(IPAddress.Loopback, serverPort, lo => lo.UseHttps(certificate));
                    options.AddServerHeader = false;
                }

                var host = new WebHostBuilder()
                    .UseStartup<Startup>()
                    .ConfigureServices(ConfigureServices)
                    .UseKestrel(ConfigureKestrel)
                    .UseWebRoot(serverRoot)
                    .UseContentRoot(serverRoot)
                    .Build();

                var resetEvent = new ManualResetEventSlim(false);

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        if (!cancellationTokenSource.IsCancellationRequested)
                        {
                            cancellationTokenSource.Cancel();
                        }

                        resetEvent.Wait();
                        e.Cancel = true;
                    };

                    using (host)
                    {
                        host.Start();

                        Console.WriteLine(Strings.GetString("info.server_root"), serverRoot);
                        Console.WriteLine(Strings.GetString("info.server_port"), serverPort);
                        Console.WriteLine(Strings.GetString("info.x509_file"), x509File);
                        Console.WriteLine(Strings.GetString("info.x509_info"), certificate.Subject, certificate.NotBefore, certificate.NotAfter);

                        if (logFile != null)
                        {
                            Console.WriteLine(Strings.GetString("info.log_file"), logFile);
                        }

                        Console.WriteLine();

                        var applicationLifetime = host.Services.GetService<IApplicationLifetime>();

                        cancellationTokenSource.Token.Register(state => ((IApplicationLifetime)state).StopApplication(), applicationLifetime);
                        applicationLifetime.ApplicationStopping.WaitHandle.WaitOne();
                    }

                    resetEvent.Set();
                }
            }
            catch (Exception ex)
            {
                Environment.ExitCode = 1;

                Console.WriteLine(Strings.GetString("program.error_message"), ex.Message);
                Console.WriteLine();
                Console.WriteLine(Strings.GetString("program.usage_message"), Path.GetFileName(assembly.Location));
            }
        }
    }
}