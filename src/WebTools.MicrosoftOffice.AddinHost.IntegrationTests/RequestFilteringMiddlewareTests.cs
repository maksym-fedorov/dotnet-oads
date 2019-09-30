using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WebTools.MicrosoftOffice.AddinHost.Middleware;

namespace WebTools.MicrosoftOffice.AddinHost.IntegrationTests
{
    [TestClass]
    public sealed class RequestFilteringMiddlewareTests
    {
        [DataTestMethod]
        [DataRow("GET", 404)]
        [DataRow("OPTIONS", 405)]
        [DataRow("POST", 405)]
        public async Task FilterHttpMethod(string method, int status)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(sc => sc
                    .AddSingleton<RequestFilteringMiddleware, RequestFilteringMiddleware>())
                .Configure(ab => ab
                    .UseMiddleware<RequestFilteringMiddleware>());

            using var server = new TestServer(builder);
            using var client = server.CreateClient();
            using var request = new HttpRequestMessage(new HttpMethod(method), server.BaseAddress);
            using var response = await client.SendAsync(request);

            Assert.AreEqual((HttpStatusCode)status, response.StatusCode);
        }
    }
}
