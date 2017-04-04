using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OfficeAddinDevServer
{
    internal sealed class Startup
    {
        public void Configure(IApplicationBuilder appBuilder, ILoggerFactory loggerFactory)
        {
            appBuilder.UseStaticFiles();
            appBuilder.UseStatusCodePages(CreateStatus);

            loggerFactory.AddConsole();
        }

        private static async Task CreateStatus(StatusCodeContext statusCodeContext)
        {
            if (statusCodeContext == null)
                throw new ArgumentNullException(nameof(statusCodeContext));

            statusCodeContext.HttpContext.Response.ContentType = "text/plain";

            await statusCodeContext.HttpContext.Response.WriteAsync($"Status Code: {statusCodeContext.HttpContext.Response.StatusCode}; URL: \"{statusCodeContext.HttpContext.Request.Path}\"");
        }
    }
}