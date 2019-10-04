using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using WebTools.MicrosoftOffice.AddinHost.Middleware;

namespace WebTools.MicrosoftOffice.AddinHost.IntegrationTests
{
    [TestClass]
    public sealed class RequestTracingMiddlewareTests
    {
        [TestMethod]
        public async Task TraceHttpRequestMessage()
        {
            var loggerMock = new Mock<Serilog.ILogger>(MockBehavior.Strict);

            loggerMock
                .Setup(o => o.Write(It.IsAny<Serilog.Events.LogEventLevel>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback<Serilog.Events.LogEventLevel, string, int, string>((l, _, pv0, pv1) => Trace.WriteLine($"Level: '{l}', Value #0: '{pv0}', Value #1: '{pv1}'"));

            var builder = new WebHostBuilder()
                .ConfigureServices(sc => sc
                    .AddSingleton(loggerMock.Object)
                    .AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>())
                .Configure(ab => ab
                    .UseMiddleware<RequestTracingMiddleware>());

            using var server = new TestServer(builder);
            using var client = server.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, server.BaseAddress);
            using var response = await client.SendAsync(request);

            loggerMock
                .Verify(o => o.Write(It.IsAny<Serilog.Events.LogEventLevel>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
