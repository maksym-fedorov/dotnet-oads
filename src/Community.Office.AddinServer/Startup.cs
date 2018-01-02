using System.Globalization;
using System.Threading.Tasks;
using Community.Office.AddinServer.Middleware;
using Community.Office.AddinServer.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Community.Office.AddinServer
{
    /// <summary>Server startup logic.</summary>
    internal sealed class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<RequestFilteringMiddleware>();
            app.UseMiddleware<RequestTracingMiddleware>();

            var staticFileOptions = new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            };

            app.UseStaticFiles(staticFileOptions);
            app.UseStatusCodePages(CreateStatusAsync);
        }

        private static Task CreateStatusAsync(StatusCodeContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";

            var message = string.Format(CultureInfo.InvariantCulture, "{0} \"{1}{2}\"",
                context.HttpContext.Request.Method, context.HttpContext.Request.Path, context.HttpContext.Request.QueryString);

            var content = EmbeddedResourceManager.GetString("Assets.ErrorPage.html").Replace("{message}", message);

            return context.HttpContext.Response.WriteAsync(content);
        }
    }
}