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
            var siteRootOption = clapp.Option("-sr | --site-root <value>", "Site root directory", CommandOptionType.SingleValue);
            var certPasswordOption = clapp.Option("-cp | --cert-password <value>", "Certificate password", CommandOptionType.SingleValue);

            var portValue = default(int);
            var siteRootValue = default(string);
            var certPasswordValue = string.Empty;

            try
            {
                clapp.Execute(args);

                if (!portOption.HasValue())
                    throw new InvalidOperationException($"{portOption.Description} is not specified");
                if (!siteRootOption.HasValue())
                    throw new InvalidOperationException($"{siteRootOption.Description} is not specified");

                if (!int.TryParse(portOption.Value(), NumberStyles.None, CultureInfo.InvariantCulture, out portValue))
                    throw new InvalidOperationException($"{portOption.Description} has invalid value");
                if (!Directory.Exists(siteRootOption.Value()))
                    throw new InvalidOperationException($"{siteRootOption.Description} doesn't exist");

                siteRootValue = siteRootOption.Value();

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
                    .UseKestrel(x => x.UseHttps(Path.Combine(Path.GetDirectoryName(_assembly.Location), "certificate.pfx"), certPasswordValue))
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