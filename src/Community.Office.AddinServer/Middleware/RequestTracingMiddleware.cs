using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Add-in request tracking middleware.</summary>
    internal sealed class RequestTracingMiddleware
    {
        private static readonly ConsoleColor _foregroundColor = Console.ForegroundColor;
        private static readonly object _syncRoot = new object();

        private readonly RequestDelegate _next;

        public RequestTracingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context).ConfigureAwait(false);

            var message = string.Format(CultureInfo.InvariantCulture, "{0:O} {1} {2} \"{3}{4}\"",
                DateTime.Now, context.Response.StatusCode, context.Request.Method, context.Request.Path, context.Request.QueryString);

            lock (_syncRoot)
            {
                if (context.Response.StatusCode < (int)HttpStatusCode.BadRequest)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = _foregroundColor;
                }
            }
        }
    }
}