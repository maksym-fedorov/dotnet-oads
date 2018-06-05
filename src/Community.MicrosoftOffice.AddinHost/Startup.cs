using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Community.MicrosoftOffice.AddinHost.Data;
using Community.MicrosoftOffice.AddinHost.Middleware;
using Community.MicrosoftOffice.AddinHost.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Community.MicrosoftOffice.AddinHost
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
            return services
                .AddSingleton(CreateLogger)
                .AddSingleton<RequestFilteringMiddleware, RequestFilteringMiddleware>()
                .AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>()
                .BuildServiceProvider();
        }

        private ILogger CreateLogger(IServiceProvider services)
        {
            var configuration = new LoggerConfiguration();

            var theme = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
            {
                [ConsoleThemeStyle.Text] = "\x1b[38;5;0253m",
                [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0246m",
                [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0253m",
                [ConsoleThemeStyle.Invalid] = "\x1b[33;1m",
                [ConsoleThemeStyle.Null] = "\x1b[38;5;0038m",
                [ConsoleThemeStyle.Name] = "\x1b[38;5;0081m",
                [ConsoleThemeStyle.String] = "\x1b[38;5;0216m",
                [ConsoleThemeStyle.Number] = "\x1b[38;5;0151m",
                [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0038m",
                [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0079m",
                [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m",
                [ConsoleThemeStyle.LevelDebug] = "\x1b[37m",
                [ConsoleThemeStyle.LevelInformation] = "\x1b[37;1m",
                [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;0229m",
                [ConsoleThemeStyle.LevelError] = "\x1b[38;5;0196m",
                [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;0196m"
            });

            configuration.WriteTo.Async(c => c.Console(outputTemplate: _loggingTemplate, theme: theme));

            if (_options.LoggingFilePath != null)
            {
                configuration.WriteTo.Async(c => c.File(outputTemplate: _loggingTemplate, path: _options.LoggingFilePath));
            }

            return configuration.CreateLogger();
        }

        private static Task CreateStatusCodePageAsync(StatusCodeContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";

            var message = _errorPageTemplate.Replace("{message}", context.HttpContext.Request.GetEncodedPathAndQuery());

            return context.HttpContext.Response.WriteAsync(message, context.HttpContext.RequestAborted);
        }
    }
}