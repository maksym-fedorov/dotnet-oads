using System;
using System.Threading.Tasks;
using Community.Office.AddinServer.Data;
using Community.Office.AddinServer.Middleware;
using Community.Office.AddinServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Community.Office.AddinServer
{
    /// <summary>Represents server startup logic.</summary>
    internal sealed class Startup : IStartup
    {
        private const string _loggingTemplate = "{Timestamp:yyyy-MM-dd/HH:mm:ss.ffzzz} {Level:u3} {Message:lj}{NewLine}";

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

            builder
                .UseMiddleware<RequestFilteringMiddleware>()
                .UseMiddleware<RequestTracingMiddleware>()
                .UseStaticFiles(staticFileOptions)
                .UseDirectoryBrowser()
                .UseStatusCodePages(CreateStatusCodePageAsync);
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            var loggerConfiguration = new LoggerConfiguration();

            loggerConfiguration.WriteTo.Async(c => c.Console(outputTemplate: _loggingTemplate, theme: AnsiConsoleTheme.Code));

            if (_options.LoggingFilePath != null)
            {
                loggerConfiguration.WriteTo.Async(c => c.File(outputTemplate: _loggingTemplate, path: _options.LoggingFilePath));
            }

            return services
                .AddSingleton<ILogger>(sp => loggerConfiguration.CreateLogger())
                .AddSingleton<RequestFilteringMiddleware, RequestFilteringMiddleware>()
                .AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>()
                .BuildServiceProvider();
        }

        private static Task CreateStatusCodePageAsync(StatusCodeContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";

            var message = _errorPageTemplate.Replace("{message}", context.HttpContext.Request.GetEncodedPathAndQuery());

            return context.HttpContext.Response.WriteAsync(message, context.HttpContext.RequestAborted);
        }
    }
}