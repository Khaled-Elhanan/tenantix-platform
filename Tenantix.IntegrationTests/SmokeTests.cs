using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tenantix.IntegrationTests
{
    public class SmokeTests : IClassFixture<TestAppFactory>
    {
        private readonly TestAppFactory _factory;

        public SmokeTests(TestAppFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Server_Should_Start_And_Serve_Requests()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
         
            var response = await client.GetAsync("/api/non-existent-endpoint-for-smoke-test");

            // Assert
             // Here i expect 404 Not Found ==> which means the server responded.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
