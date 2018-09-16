using System.Net.Http;
using System.Threading.Tasks;
using Anemonis.MicrosoftOffice.AddinHost.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Anemonis.MicrosoftOffice.AddinHost.IntegrationTests
{
    [TestClass]
    public sealed class RequestTracingMiddlewareTests
    {
        [TestMethod]
        public async Task Trace()
        {
            var loggerMock = new Mock<Serilog.ILogger>(MockBehavior.Strict);

            loggerMock.Setup(o => o.Write(It.IsAny<Serilog.Events.LogEventLevel>(), It.IsAny<string>(), It.IsAny<object[]>()));

            var builder = new WebHostBuilder()
                .ConfigureServices(sc => sc
                    .AddSingleton(loggerMock.Object)
                    .AddSingleton<RequestTracingMiddleware, RequestTracingMiddleware>())
                .Configure(ab => ab
                    .UseMiddleware<RequestTracingMiddleware>());

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, server.BaseAddress);
                    var response = await client.SendAsync(request);
                }
            }

            loggerMock.Verify(o => o.Write(It.IsAny<Serilog.Events.LogEventLevel>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(1));
        }
    }
}