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
        private static readonly object _consoleSyncRoot = new object();

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

            WriteLine(message, context.Response.StatusCode < (int)HttpStatusCode.BadRequest ? ConsoleColor.Red : (ConsoleColor?)null);
        }

        private static void WriteLine(string value, ConsoleColor? color = null)
        {
            lock (_consoleSyncRoot)
            {
                var foregroundColor = Console.ForegroundColor;

                if (color.HasValue && (color.Value != foregroundColor))
                {
                    Console.ForegroundColor = color.Value;
                }

                Console.WriteLine(value);

                if (color.HasValue && (color.Value != foregroundColor))
                {
                    Console.ForegroundColor = foregroundColor;
                }
            }
        }
    }
}