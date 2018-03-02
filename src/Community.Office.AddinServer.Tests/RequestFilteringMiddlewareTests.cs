using System.Net;
using System.Net.Http;
using Community.Office.AddinServer.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Community.Office.AddinServer.Tests
{
    public sealed class RequestFilteringMiddlewareTests
    {
        private readonly ITestOutputHelper _output;

        public RequestFilteringMiddlewareTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("GET", 404)]
        [InlineData("OPTIONS", 405)]
        [InlineData("POST", 405)]
        public async void FilterHttpMethod(string method, int status)
        {
            var builder = new WebHostBuilder()
                .ConfigureLogging(lb => lb
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddXunit(_output))
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

                    Assert.Equal((HttpStatusCode)status, response.StatusCode);
                }
            }
        }
    }
}