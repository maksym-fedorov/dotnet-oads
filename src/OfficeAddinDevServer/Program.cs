using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace OfficeAddinDevServer
{
    internal static class Program
    {
        private static readonly Assembly _assembly = typeof(Program).GetTypeInfo().Assembly;

        public static void Main(string[] args)
        {
            Console.WriteLine($"{_assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {_assembly.GetName().Version}");
            Console.WriteLine();

            var clapp = new CommandLineApplication();

            var portOption = clapp.Option("-sp | --server-port <value>", "Server port", CommandOptionType.SingleValue);
            var siteRootOption = clapp.Option("-sr | --server-root <value>", "Server root directory", CommandOptionType.SingleValue);
            var certFileOption = clapp.Option("-cf | --cert-file <value>", "Certificate file in PKCS #12 format", CommandOptionType.SingleValue);
            var certPasswordOption = clapp.Option("-cp | --cert-password <value>", "Certificate password", CommandOptionType.SingleValue);

            var portValue = 44300;
            var siteRootValue = default(string);
            var certFileValue = Path.Combine(Path.GetDirectoryName(_assembly.Location), "certificate.pfx");
            var certPasswordValue = string.Empty;

            try
            {
                clapp.Execute(args);

                if (portOption.HasValue())
                {
                    if (!int.TryParse(portOption.Value(), NumberStyles.None, CultureInfo.InvariantCulture, out portValue))
                        throw new InvalidOperationException($"{portOption.Description} has invalid value");
                }

                if (!siteRootOption.HasValue())
                    throw new InvalidOperationException($"{siteRootOption.Description} is not specified");

                siteRootValue = siteRootOption.Value();

                if (!Directory.Exists(siteRootValue))
                    throw new InvalidOperationException($"{siteRootOption.Description} doesn't exist");
                if (certFileOption.HasValue())
                    certFileValue = Path.GetFullPath(certFileOption.Value());
                if (!File.Exists(certFileValue))
                    throw new InvalidOperationException($"{certFileOption.Description} doesn't exist");
                if (certPasswordOption.HasValue())
                    certPasswordValue = certPasswordOption.Value();

            }
            catch (Exception e)
            {
                Console.Write($"ERROR: {e.Message}");

                clapp.ShowHelp();

                Environment.Exit(1);
            }

            try
            {
                new WebHostBuilder()
                    .UseKestrel(x => x.UseHttps(certFileValue, certPasswordValue))
                    .UseUrls(new UriBuilder("https", "localhost", portValue).Uri.OriginalString)
                    .UseStartup<Startup>()
                    .UseWebRoot(siteRootValue)
                    .UseContentRoot(siteRootValue)
                    .Build()
                    .Run();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }

        private sealed class Startup
        {
            public void Configure(IApplicationBuilder appBuilder, ILoggerFactory loggerFactory)
            {
                appBuilder.UseStaticFiles();
                appBuilder.UseStatusCodePages();
                loggerFactory.AddConsole();
            }
        }
    }
}