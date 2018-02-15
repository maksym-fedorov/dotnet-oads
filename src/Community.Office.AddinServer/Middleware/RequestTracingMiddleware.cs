using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Community.Office.AddinServer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Represents add-in request tracking middleware.</summary>
    internal sealed class RequestTracingMiddleware : IMiddleware
    {
        private static readonly object _consoleSyncRoot = new object();

        private readonly LoggingOptions _options;

        public RequestTracingMiddleware(IOptions<LoggingOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next.Invoke(context).ConfigureAwait(false);

            var message = string.Format(CultureInfo.InvariantCulture, "{0:O} {1} {2} \"{3}{4}\"",
                DateTime.Now, context.Response.StatusCode, context.Request.Method, context.Request.Path, context.Request.QueryString);

            WriteLine(message, context.Response.StatusCode >= (int)HttpStatusCode.BadRequest ? ConsoleColor.Red : (ConsoleColor?)null);

            if (_options.File != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_options.File));

                using (var stream = new FileStream(_options.File, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.WriteLine(message);
                    }
                }
            }
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