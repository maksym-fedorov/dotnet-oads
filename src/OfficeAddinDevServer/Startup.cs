using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using OfficeAddinDevServer.Middleware;
using OfficeAddinDevServer.Resources;

namespace OfficeAddinDevServer
{
    /// <summary>Server startup logic.</summary>
    internal sealed class Startup
    {
        private static readonly string _errorPageContent = EmbeddedResourceManager.GetString("Assets.ErrorPage.html");
        private static readonly Regex _errorPageRegex = new Regex(@"\{message\}", RegexOptions.Compiled);

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<RequestHandling>();
            app.UseMiddleware<RequestTracing>();
            app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
            app.UseStatusCodePages(CreateStatus);
        }

        private static async Task CreateStatus(StatusCodeContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";

            var message = $"{context.HttpContext.Response.StatusCode} \"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}\"";

            await context.HttpContext.Response.WriteAsync(_errorPageRegex.Replace(_errorPageContent, message));
        }
    }
}