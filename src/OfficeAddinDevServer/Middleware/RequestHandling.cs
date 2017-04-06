using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OfficeAddinDevServer.Middleware
{
    /// <summary>Add-in request handling middleware.</summary>
    internal sealed class RequestHandling
    {
        private readonly RequestDelegate _next;

        public RequestHandling(RequestDelegate next) => _next = next;

        public Task Invoke(HttpContext context)
        {
            if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;

                return Task.CompletedTask;
            }

            return _next(context);
        }
    }
}