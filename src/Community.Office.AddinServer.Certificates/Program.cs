using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Community.Office.AddinServer.Certificates.Generation;
using Community.Office.AddinServer.Certificates.Resources;
using Microsoft.Extensions.Configuration;

namespace Community.Office.AddinServer.Certificates
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine(Strings.GetString("program.assembly_info"), assembly.GetCustomAttribute<AssemblyProductAttribute>().Product, assembly.GetName().Version.ToString(3));
            Console.WriteLine(assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright?.Replace("\u00A9", "(c)"));
            Console.WriteLine();

            var configurationBuilder = new ConfigurationBuilder().AddCommandLine(args);

            try
            {
                var configuration = configurationBuilder.Build();
                var certificateFileValue = configuration["cert-file"];
                var certificateFile = certificateFileValue != null ? Path.GetFullPath(certificateFileValue) : Path.Combine(Environment.CurrentDirectory, "https.pfx");

                switch (configuration["command"])
                {
                    case "create":
                        {
                            var manager = new CertificateManager();

                            using (var certificate = manager.CreateDevelopmentCertificate(DateTime.UtcNow, 1))
                            {
                                File.WriteAllBytes(certificateFile, certificate.Export(X509ContentType.Pkcs12));

                                Console.WriteLine(Strings.GetString("manager.create.cert_file"), certificateFile);
                                Console.WriteLine();
                                Console.WriteLine(Strings.GetString("manager.create.cert_period"), certificate.NotBefore.ToShortDateString(), certificate.NotAfter.ToShortDateString());
                                Console.WriteLine(Strings.GetString("manager.create.cert_thumbprint"), certificate.Thumbprint);
                                Console.WriteLine();
                            }
                        }
                        break;
                    default:
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Strings.GetString("program.invalid_parameter"), "command"));
                        }
                }
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
                Console.WriteLine(Strings.GetString("program.usage_argument_command"));
                Console.WriteLine(Strings.GetString("program.usage_argument_cert_file"));
            }
        }
    }
}