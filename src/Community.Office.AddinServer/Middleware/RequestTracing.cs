using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Add-in request tracking middleware.</summary>
    internal sealed class RequestTracing
    {
        private static readonly ConsoleColor _defaultConsoleColor = Console.ForegroundColor;
        private static readonly object _syncRoot = new object();

        private readonly RequestDelegate _next;

        public RequestTracing(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (string.Compare(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return;
            }

            var messageTimestamp = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'ffff", CultureInfo.InvariantCulture);
            var message = $"{messageTimestamp} {context.Response.StatusCode} \"{context.Request.Path}{context.Request.QueryString}\"";

            if (context.Response.StatusCode < (int)HttpStatusCode.BadRequest)
            {
                Console.WriteLine(message);
            }
            else
            {
                lock (_syncRoot)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = _defaultConsoleColor;
                }
            }
        }
    }
}