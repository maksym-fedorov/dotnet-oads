using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebTools.MicrosoftOffice.AddinHost.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            using (var server = new TestServer(builder))
            {
                using (var client = server.CreateClient())
                {
                    var request = new HttpRequestMessage(new HttpMethod(method), server.BaseAddress);
                    var response = await client.SendAsync(request);

                    response.Dispose();
                    request.Dispose();

                    Assert.AreEqual((HttpStatusCode)status, response.StatusCode);
                }
            }
        }
    }
}