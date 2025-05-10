using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using SoundStore.Core.Services;
using System.Text;

namespace SoundStore.Service.Test
{
    public class FileStorageUnitTest
    {
        private readonly Mock<ICloudinaryService> _mockCloudinaryService;
        private readonly Mock<IFormFile> _mockFormFile;
        private readonly FileStorageService _fileStorageService;

        public FileStorageUnitTest()
        {
            // Setup mock CloudinaryService
            _mockCloudinaryService = new Mock<ICloudinaryService>();

            // Create the service with mocked dependency
            _fileStorageService = new FileStorageService(_mockCloudinaryService.Object);

            // Setup mock form file
            _mockFormFile = new Mock<IFormFile>();
        }

        #region UploadImage Test Suites

        [Fact]
        public void Constructor_ShouldInitializeService_WithCloudinaryService()
        {
            // Act
            var service = new FileStorageService(_mockCloudinaryService.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task UploadImage_ShouldReturnSecureUrl_WhenFileIsValid()
        {
            // Arrange
            const string expectedUrl = "https://res.cloudinary.com/test-cloud/image/upload/test.jpg";

            var content = "Fake image content";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            _mockFormFile.Setup(f => f.FileName).Returns(fileName);
            _mockFormFile.Setup(f => f.Length).Returns(ms.Length);
            _mockFormFile.Setup(f => f.OpenReadStream()).Returns(ms);

            _mockCloudinaryService
                .Setup(cs => cs.UploadImageAsync(
                    It.Is<ImageUploadParams>(p => p.Folder == "affiliate-network" 
                        && p.File.FileName == fileName),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageUploadResult
                {
                    SecureUrl = new Uri(expectedUrl)
                });

            // Act
            var result = await _fileStorageService.UploadImage(_mockFormFile.Object);

            // Assert
            Assert.Equal(expectedUrl, result);
            _mockCloudinaryService.Verify(cs => cs.UploadImageAsync(
                It.Is<ImageUploadParams>(p => p.Folder == "affiliate-network" 
                    && p.File.FileName == fileName),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadImage_ShouldThrowArgumentException_WhenFileIsNull()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileStorageService.UploadImage(null));

            Assert.Equal("No file uploaded!", exception.Message);
            _mockCloudinaryService.Verify(cs => cs.UploadImageAsync(
                It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UploadImage_ShouldThrowArgumentException_WhenFileIsEmpty()
        {
            // Arrange
            _mockFormFile.Setup(f => f.Length).Returns(0);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileStorageService.UploadImage(_mockFormFile.Object));

            Assert.Equal("No file uploaded!", exception.Message);
            _mockCloudinaryService.Verify(cs => cs.UploadImageAsync(
                It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UploadImage_ShouldPropagateExceptions_WhenCloudinaryServiceFails()
        {
            // Arrange
            var content = "Fake image content";
            var fileName = "test.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            _mockFormFile.Setup(f => f.FileName).Returns(fileName);
            _mockFormFile.Setup(f => f.Length).Returns(ms.Length);
            _mockFormFile.Setup(f => f.OpenReadStream()).Returns(ms);

            var expectedException = new Exception("Upload failed");
            _mockCloudinaryService
                .Setup(cs => cs.UploadImageAsync(
                    It.IsAny<ImageUploadParams>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _fileStorageService.UploadImage(_mockFormFile.Object));

            Assert.Same(expectedException, exception);
        }

        #endregion
    }
}
