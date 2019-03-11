using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebTools.MicrosoftOffice.AddinHost.Middleware
{
    /// <summary>Represents request filtering middleware.</summary>
    public sealed class RequestFilteringMiddleware : IMiddleware
    {
        /// <summary>Initializes a new instance of the <see cref="RequestFilteringMiddleware" /> class.</summary>
        public RequestFilteringMiddleware()
        {
        }

        Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!string.Equals(context.Request.Method, HttpMethods.Get, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

                return Task.CompletedTask;
            }

            return next.Invoke(context);
        }
    }
}