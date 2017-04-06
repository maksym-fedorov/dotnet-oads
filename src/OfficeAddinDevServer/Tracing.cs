using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OfficeAddinDevServer
{
    internal sealed class Tracing
    {
        private static readonly object _syncRoot = new object();
        private readonly RequestDelegate _next;

        public Tracing(RequestDelegate next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            await _next(context);

            var messageColor = Console.ForegroundColor;

            if (context.Response.StatusCode < 300)
                messageColor = ConsoleColor.Green;
            else if (context.Response.StatusCode < 400)
                messageColor = ConsoleColor.Yellow;
            else
                messageColor = ConsoleColor.Red;

            var messageTimestamp = DateTime.Now.ToString("s", CultureInfo.InvariantCulture);
            var message = $"{messageTimestamp} {context.Request.Method} \"{context.Request.Path}{context.Request.QueryString}\" / {context.Response.StatusCode}";

            var foregroundColor = Console.ForegroundColor;

            lock (_syncRoot)
            {
                Console.ForegroundColor = messageColor;
                Console.WriteLine(message);
                Console.ForegroundColor = foregroundColor;
            }
        }
    }
}