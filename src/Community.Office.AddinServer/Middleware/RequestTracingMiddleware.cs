using System;
using System.Globalization;
using System.IO;
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

        private readonly string _filePath;
        private readonly string _folderPath;

        /// <summary>Initializes a new instance of the <see cref="RequestTracingMiddleware" /> class.</summary>
        /// <param name="options">The logging options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options" /> is <see langword="null" />.</exception>
        public RequestTracingMiddleware(IOptions<LoggingOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _filePath = options.Value.FilePath;

            if (_filePath != null)
            {
                _folderPath = Path.GetDirectoryName(_filePath);
            }
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next.Invoke(context);

            var message = string.Format(CultureInfo.InvariantCulture, "{0:O} {1} {2} \"{3}{4}\"",
                DateTime.Now, context.Response.StatusCode, context.Request.Method, context.Request.Path, context.Request.QueryString);

            WriteLine(message, context.Response.StatusCode >= StatusCodes.Status400BadRequest ? ConsoleColor.Red : (ConsoleColor?)null);

            if (_filePath != null)
            {
                Directory.CreateDirectory(_folderPath);

                using (var fileStream = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        streamWriter.WriteLine(message);
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