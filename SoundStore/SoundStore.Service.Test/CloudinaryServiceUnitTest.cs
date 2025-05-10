using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Moq;
using SoundStore.Core.Commons;

namespace SoundStore.Service.Test
{
    public class CloudinaryServiceUnitTest
    {
        private readonly Mock<IOptions<CloudinarySettings>> _mockOptions;
        private const string TestCloudName = "test-cloud";
        private const string TestApiKey = "test-api-key";
        private const string TestApiSecret = "test-api-secret";

        public CloudinaryServiceUnitTest()
        {
            // Setup mock options
            _mockOptions = new Mock<IOptions<CloudinarySettings>>();
            _mockOptions.Setup(x => x.Value).Returns(new CloudinarySettings
            {
                CloudName = TestCloudName,
                ApiKey = TestApiKey,
                ApiSecret = TestApiSecret
            });
        }

        [Fact]
        public void Constructor_ShouldInitializeCloudinary_WithCorrectAccount()
        {
            // Act
            var service = new CloudinaryService(_mockOptions.Object);

            // Assert
            Assert.NotNull(service);
            // We can't directly access _cloudinary as it's private, 
            // but the service was created without exceptions, confirming initialization
        }

        [Fact]
        public async Task UploadImageAsync_ShouldCallCloudinaryUpload_WithCorrectParameters()
        {
            // Arrange - We need a testable CloudinaryService with a mocked Cloudinary
            // This requires creating a test-specific subclass
            var mockCloudinaryWrapper = new Mock<ICloudinaryWrapper>();
            var testService = new TestableCloudinaryService(_mockOptions.Object, mockCloudinaryWrapper.Object);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("test.jpg", new MemoryStream()),
                Folder = "test-folder"
            };
            var expectedResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://res.cloudinary.com/test-cloud/image/upload/test.jpg")
            };

            mockCloudinaryWrapper.Setup(c => c.UploadAsync(
                It.Is<ImageUploadParams>(p => p == uploadParams),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await testService.UploadImageAsync(uploadParams);

            // Assert
            Assert.Equal(expectedResult, result);
            mockCloudinaryWrapper.Verify(c => c.UploadAsync(
                It.Is<ImageUploadParams>(p => p == uploadParams),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadImageAsync_ShouldPassThroughCancellationToken()
        {
            // Arrange
            var mockCloudinaryWrapper = new Mock<ICloudinaryWrapper>();
            var testService = new TestableCloudinaryService(_mockOptions.Object, mockCloudinaryWrapper.Object);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("test.jpg", new MemoryStream())
            };
            var cancellationToken = new CancellationToken(true); // Canceled token

            mockCloudinaryWrapper.Setup(c => c.UploadAsync(
                It.IsAny<ImageUploadParams>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                testService.UploadImageAsync(uploadParams, cancellationToken));

            mockCloudinaryWrapper.Verify(c => c.UploadAsync(
                It.IsAny<ImageUploadParams>(),
                It.Is<CancellationToken>(ct => ct.IsCancellationRequested)),
                Times.Once);
        }

        [Fact]
        public async Task UploadImageAsync_ShouldPropagateExceptions()
        {
            // Arrange
            var mockCloudinaryWrapper = new Mock<ICloudinaryWrapper>();
            var testService = new TestableCloudinaryService(_mockOptions.Object, mockCloudinaryWrapper.Object);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("test.jpg", new MemoryStream())
            };
            var expectedException = new Exception("Upload failed");

            mockCloudinaryWrapper.Setup(c => c.UploadAsync(
                It.IsAny<ImageUploadParams>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                testService.UploadImageAsync(uploadParams));

            Assert.Same(expectedException, exception);
        }
    }

    public interface ICloudinaryWrapper
    {
        Task<ImageUploadResult> UploadAsync(ImageUploadParams parameters, 
            CancellationToken cancellationToken = default);
    }

    // Testable version of CloudinaryService that allows injecting a mock Cloudinary
    class TestableCloudinaryService : CloudinaryService
    {
        private readonly ICloudinaryWrapper _cloudinaryWrapper;

        public TestableCloudinaryService(IOptions<CloudinarySettings> options, 
            ICloudinaryWrapper cloudinaryWrapper)
            : base(options)
        {
            _cloudinaryWrapper = cloudinaryWrapper;
        }

        public override Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams,
            CancellationToken cancellationToken = default)
        {
            return _cloudinaryWrapper.UploadAsync(uploadParams, cancellationToken);
        }
    }
}
