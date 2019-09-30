using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using Serilog;
using Serilog.Events;

namespace WebTools.MicrosoftOffice.AddinHost.Middleware
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
            try
            {
                await next.Invoke(context);
            }
            finally
            {
                var level = context.Response.StatusCode < StatusCodes.Status400BadRequest ? LogEventLevel.Information : LogEventLevel.Error;

                _logger.Write(level, "{0} {1}", context.Response.StatusCode, context.Request.GetEncodedPathAndQuery());
            }
        }
    }
}
