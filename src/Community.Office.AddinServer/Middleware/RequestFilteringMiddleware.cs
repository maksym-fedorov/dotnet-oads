using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Add-in request filtering middleware.</summary>
    internal sealed class RequestFilteringMiddleware : IMiddleware
    {
        Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (string.Compare(context.Request.Method, HttpMethods.Get, StringComparison.OrdinalIgnoreCase) != 0)
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;

                return Task.CompletedTask;
            }

            return next(context);
        }
    }
}