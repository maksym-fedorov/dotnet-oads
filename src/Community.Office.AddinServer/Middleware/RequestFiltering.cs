using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Add-in request filtering middleware.</summary>
    internal sealed class RequestFiltering
    {
        private readonly RequestDelegate _next;

        public RequestFiltering(RequestDelegate next)
        {
            _next = next;
        }

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