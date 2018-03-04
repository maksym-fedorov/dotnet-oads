using System;
using System.Globalization;
using System.Threading.Tasks;
using Community.Office.AddinServer.Data;
using Community.Office.AddinServer.Middleware;
using Community.Office.AddinServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Community.Office.AddinServer
{
    /// <summary>Represents server startup logic.</summary>
    internal sealed class Startup : IStartup
    {
        private const string _loggingTemplate = "{Timestamp:yyyy-MM-dd/HH:mm:ss.ffzzz} {Level:u4} {Message:lj}{NewLine}";

        private static readonly string _errorPageTemplate = EmbeddedResourceManager.GetString("Assets.ErrorPage.html");

        private readonly ServerOptions _options;

        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        /// <param name="options">The server options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
        public Startup(IOptions<ServerOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        void IStartup.Configure(IApplicationBuilder builder)
        {
            var staticFileOptions = new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            };

            builder.UseMiddleware<RequestFilteringMiddleware>();
            builder.UseMiddleware<RequestTracingMiddleware>();
            builder.UseStaticFiles(staticFileOptions);
            builder.UseDirectoryBrowser();
            builder.UseStatusCodePages(CreateStatusAsync);
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            var loggerConfiguration = new LoggerConfiguration();

            loggerConfiguration.WriteTo.Async(c => c.Console(outputTemplate: _loggingTemplate, theme: AnsiConsoleTheme.Code));

            if (_options.LoggingFilePath != null)
            {
                loggerConfiguration.WriteTo.Async(c => c.File(outputTemplate: _loggingTemplate, path: _options.LoggingFilePath));
            }

            services.AddSingleton<ILogger>(sp => loggerConfiguration.CreateLogger());
            services.AddSingleton<RequestFilteringMiddleware, RequestFilteringMiddleware>();
            services.AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>();

            return services.BuildServiceProvider();
        }

        private static Task CreateStatusAsync(StatusCodeContext context)
        {
            var message = string.Format(CultureInfo.InvariantCulture, "\"{0}{1}\"", context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);
            var content = _errorPageTemplate.Replace("{message}", message);

            context.HttpContext.Response.ContentType = "text/html";

            return context.HttpContext.Response.WriteAsync(content, context.HttpContext.RequestAborted);
        }
    }
}