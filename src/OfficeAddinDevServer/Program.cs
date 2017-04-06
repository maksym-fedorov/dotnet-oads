using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
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

            var app = new CommandLineApplication();

            var serverPortOption = app.Option("-sp | --server-port <value>", "Server port", CommandOptionType.SingleValue);
            var serverRootOption = app.Option("-sr | --server-root <value>", "Server root directory", CommandOptionType.SingleValue);
            var certFileOption = app.Option("-xf | --x509-file <value>", "X.509 certificate file", CommandOptionType.SingleValue);
            var certPasswordOption = app.Option("-xp | --cert-password <value>", "X.509 certificate password", CommandOptionType.SingleValue);

            var serverPortValue = 44300;
            var serverRootValue = default(string);
            var certFileValue = Path.Combine(Path.GetDirectoryName(assembly.Location), "certificate.pfx");
            var certPasswordValue = string.Empty;

            try
            {
                app.Execute(args);

                if (serverPortOption.HasValue())
                {
                    if (!int.TryParse(serverPortOption.Value(), NumberStyles.None, CultureInfo.InvariantCulture, out serverPortValue))
                        throw new InvalidOperationException($"{serverPortOption.Description} has invalid value");
                }

                if (!serverRootOption.HasValue())
                    throw new InvalidOperationException($"{serverRootOption.Description} is not specified");
                if (!Directory.Exists(serverRootOption.Value()))
                    throw new InvalidOperationException($"{serverRootOption.Description} doesn't exist");

                serverRootValue = Path.GetFullPath(serverRootOption.Value());

                if (certFileOption.HasValue())
                    certFileValue = Path.GetFullPath(certFileOption.Value());
                if (!File.Exists(certFileValue))
                    throw new InvalidOperationException($"{certFileOption.Description} doesn't exist");
                if (certPasswordOption.HasValue())
                    certPasswordValue = certPasswordOption.Value();

            }
            catch (Exception ex)
            {
                Console.Write($"ERROR: {ex.Message}");

                app.ShowHelp();

                Environment.Exit(1);
            }

            try
            {
                var certificate = new X509Certificate2(certFileValue, certPasswordValue);

                var host = new WebHostBuilder()
                    .UseKestrel(x => x.UseHttps(certificate))
                    .UseUrls(new UriBuilder("https", "localhost", serverPortValue).Uri.OriginalString)
                    .UseStartup<Startup>()
                    .UseWebRoot(serverRootValue)
                    .UseContentRoot(serverRootValue)
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

                        Console.WriteLine($"SERVER_ROOT: \"{serverRootValue}\"");
                        Console.WriteLine($"SERVER_PORT: {serverPortValue}");
                        Console.WriteLine($"X509_FILE: \"{certFileValue}\"");
                        Console.WriteLine($"X509_SUBJECT: \"{certificate.Subject}\"");
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
            }
        }
    }
}