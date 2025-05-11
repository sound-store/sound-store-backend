using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SoundStore.API.Controllers.v1;
using SoundStore.Core.Commons;
using System.Net;

namespace SoundStore.API.Test.Controllers
{
    public class BaseApiControllerUnitTest
    {
        [Fact]
        public void HandleValidationErrors_ReturnsNull_WhenModelIsValid()
        {
            // Arrange
            var controller = new TestableApiController(); // No error added

            // Act
            var result = controller.InvokeValidationHandler();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void HandleValidationErrors_ReturnsJsonResult_WhenModelIsInvalid()
        {
            // Arrange
            var controller = new TestableApiController();
            controller.AddModelError("Point", "Rating point is required.");

            // Act
            var result = controller.InvokeValidationHandler();

            // Assert
            var json = Assert.IsType<JsonResult>(result);
            json.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var error = Assert.IsType<ErrorResponse>(json.Value);
            error.Message.Should().Be("Validation failed.");
            error.Details.Should().Be("Rating point is required.");
        }
    }
}
