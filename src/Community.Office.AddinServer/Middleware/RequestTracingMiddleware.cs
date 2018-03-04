using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Serilog.Events;

namespace Community.Office.AddinServer.Middleware
{
    /// <summary>Represents request tracking middleware.</summary>
    public sealed class RequestTracingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        /// <summary>Initializes a new instance of the <see cref="RequestTracingMiddleware" /> class.</summary>
        /// <param name="logger">The logger instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
        public RequestTracingMiddleware(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next.Invoke(context);

            var level = context.Response.StatusCode < StatusCodes.Status400BadRequest ? LogEventLevel.Information : LogEventLevel.Warning;

            var values = new object[]
            {
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.GetEncodedPathAndQuery()
            };

            _logger.Write(level, "{0} {1} {2}", values);
        }
    }
}