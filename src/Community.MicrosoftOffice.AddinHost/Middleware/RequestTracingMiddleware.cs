// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Community.MicrosoftOffice.AddinHost.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Serilog.Events;

namespace Community.MicrosoftOffice.AddinHost.Middleware
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

            if (!context.RequestAborted.IsCancellationRequested)
            {
                var level = context.Response.StatusCode < StatusCodes.Status400BadRequest ? LogEventLevel.Information : LogEventLevel.Error;

                var values = new object[]
                {
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.GetEncodedPathAndQuery()
                };

                _logger.Write(level, "{0} {1} {2}", values);
            }
            else
            {
                var values = new object[]
                {
                    context.Response.StatusCode,
                    context.Request.Method,
                    context.Request.GetEncodedPathAndQuery(),
                    Strings.GetString("server.abort_token").ToUpperInvariant()
                };

                _logger.Write(LogEventLevel.Error, "{0} {1} {2} {3}", values);
            }
        }
    }
}