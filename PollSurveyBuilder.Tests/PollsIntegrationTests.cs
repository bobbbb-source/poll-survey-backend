using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PollSurveyBuilder.API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace PollSurveyBuilder.Tests
{
    public class PollsIntegrationTests
        : IClassFixture<PollApiFactory>
    {
        private readonly HttpClient _client;

        public PollsIntegrationTests(PollApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreatePoll_ReturnsCreated()
        {
            var request = new
            {
                question = "Integration test poll",
                options = new[]
                {
                    "Yes",
                    "No"
                }
            };

            var response = await _client.PostAsJsonAsync(
                "/api/polls",
                request
            );

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode
            );
        }
    }

    public class PollApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ApplicationDbContext>();
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestDb");
                });
            });
        }
    }
}