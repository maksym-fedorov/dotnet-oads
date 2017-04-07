using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OfficeAddinDevServer
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;

            Console.WriteLine($"{assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {assembly.GetName().Version}");
            Console.WriteLine();

            var serverPort = 44300;
            var serverRoot = default(string);
            var x509File = Path.Combine(Path.GetDirectoryName(assembly.Location), "certificate.pfx");
            var x509Password = string.Empty;

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Path.GetDirectoryName(assembly.Location), "settings.json"), true, false)
                .AddCommandLine(args);

            try
            {
                var configuration = configurationBuilder.Build();

                if (configuration["server-root"] == null)
                    throw new InvalidOperationException("Server root directory is not specified");
                if (!Directory.Exists(configuration["server-root"]))
                    throw new InvalidOperationException("Server root directory doesn't exist");

                serverRoot = Path.GetFullPath(configuration["server-root"]);

                if (configuration["server-port"] != null)
                {
                    if (!int.TryParse(configuration["server-port"], NumberStyles.None, CultureInfo.InvariantCulture, out serverPort))
                        throw new InvalidOperationException("Server port value is invalid");
                }

                if (configuration["x509-file"] != null)
                    x509File = Path.GetFullPath(configuration["x509-file"]);
                if (!File.Exists(x509File))
                    throw new InvalidOperationException("X.509 certificate file doesn't exist");
                if (configuration["x509-password"] != null)
                    x509Password = configuration["x509-password"];

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine();

                var assemblyFile = Path.GetFileName(assembly.Location);

                Console.WriteLine($"Usage: dotnet {assemblyFile} [--server-port <value>] [--server-root <value>] [--x509-file <value>] [--x509-password <value>]");

                Environment.Exit(1);
            }

            try
            {
                var certificate = new X509Certificate2(x509File, x509Password);

                var host = new WebHostBuilder()
                    .UseKestrel(x => x.UseHttps(certificate))
                    .UseUrls(new UriBuilder("https", "localhost", serverPort).Uri.OriginalString)
                    .UseStartup<Startup>()
                    .UseWebRoot(serverRoot)
                    .UseContentRoot(serverRoot)
                    .Build();

                var resetEvent = new ManualResetEventSlim(false);

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        if (!cancellationTokenSource.IsCancellationRequested)
                            cancellationTokenSource.Cancel();

                        resetEvent.Wait();
                        e.Cancel = true;
                    };

                    using (host)
                    {
                        host.Start();

                        Console.WriteLine($"server-root: \"{serverRoot}\"");
                        Console.WriteLine($"server-port: {serverPort}");
                        Console.WriteLine($"x509-file: \"{x509File}\"");
                        Console.WriteLine($"x509-subject: \"{certificate.Subject}\"");
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
                Console.WriteLine($"ERROR: {ex.Message}");

                Environment.Exit(1);
            }
        }
    }
}