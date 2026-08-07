using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using PollSurveyBuilder.API.Controllers;
using PollSurveyBuilder.API.Data;
using PollSurveyBuilder.API.DTOs;
using PollSurveyBuilder.API.Hubs;
using Xunit;

namespace PollSurveyBuilder.Tests
{
    public class PollsControllerTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreatePoll_WithLessThanTwoOptions_ReturnsBadRequest()
        {
            // Arrange
            var context = CreateDbContext();

            var hubContextMock = new Mock<IHubContext<PollHub>>();

            var controller = new PollsController(
                context,
                hubContextMock.Object
            );

            var dto = new CreatePollDto
            {
                Question = "Favourite colour?",
                Options = new List<string>
                {
                    "Red"
                }
            };

            // Act
            var result = await controller.CreatePoll(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task CreatePoll_WithValidOptions_ReturnsCreatedAtAction()
        {
            // Arrange
            var context = CreateDbContext();

            var hubContextMock = new Mock<IHubContext<PollHub>>();

            var controller = new PollsController(
                context,
                hubContextMock.Object
            );

            var dto = new CreatePollDto
            {
                Question = "Favourite colour?",
                Options = new List<string>
        {
            "Red",
            "Blue"
        }
            };

            // Act
            var result = await controller.CreatePoll(dto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, await context.Polls.CountAsync());
        }
        [Fact]
        public async Task CreatePoll_WithMoreThanSixOptions_ReturnsBadRequest()
        {
            // Arrange
            var context = CreateDbContext();

            var hubContextMock = new Mock<IHubContext<PollHub>>();

            var controller = new PollsController(
                context,
                hubContextMock.Object
            );

            var dto = new CreatePollDto
            {
                Question = "Choose one",
                Options = new List<string>
        {
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven"
        }
            };

            // Act
            var result = await controller.CreatePoll(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task CreatePoll_WithDuplicateOptions_ReturnsBadRequest()
        {
            // Arrange
            var context = CreateDbContext();

            var hubContextMock = new Mock<IHubContext<PollHub>>();

            var controller = new PollsController(
                context,
                hubContextMock.Object
            );

            var dto = new CreatePollDto
            {
                Question = "Favourite colour?",
                Options = new List<string>
        {
            "Red",
            "red"
        }
            };

            // Act
            var result = await controller.CreatePoll(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
