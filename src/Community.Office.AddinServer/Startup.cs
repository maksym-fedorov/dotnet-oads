using System;
using System.Globalization;
using System.Threading.Tasks;
using Community.Office.AddinServer.Middleware;
using Community.Office.AddinServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Community.Office.AddinServer
{
    /// <summary>Represents server startup logic.</summary>
    internal sealed class Startup : IStartup
    {
        private static readonly string _errorPageTemplate = EmbeddedResourceManager.GetString("Assets.ErrorPage.html");

        /// <summary>Initializes a new instance of the <see cref="Startup" /> class.</summary>
        public Startup()
        {
        }

        void IStartup.Configure(IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestFilteringMiddleware>();
            builder.UseMiddleware<RequestTracingMiddleware>();

            var staticFileOptions = new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            };

            builder.UseStaticFiles(staticFileOptions);
            builder.UseStatusCodePages(CreateStatusAsync);
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton<RequestFilteringMiddleware, RequestFilteringMiddleware>();
            services.AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>();

            return services.BuildServiceProvider();
        }

        private static Task CreateStatusAsync(StatusCodeContext context)
        {
            var message = string.Format(CultureInfo.InvariantCulture, "{0} \"{1}{2}\"",
                context.HttpContext.Request.Method, context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);

            var content = _errorPageTemplate.Replace("{message}", message);

            context.HttpContext.Response.ContentType = "text/html";

            return context.HttpContext.Response.WriteAsync(content, context.HttpContext.RequestAborted);
        }
    }
}