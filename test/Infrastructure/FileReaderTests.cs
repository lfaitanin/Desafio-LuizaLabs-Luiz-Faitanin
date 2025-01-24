using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using WebAPI.Infrastructure.Helpers;
using Xunit;

namespace Tests.Infrastructure
{
    public class FileReaderTests
    {
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly FileReader _fileReader;

        public FileReaderTests()
        {
            _cacheMock = new Mock<IMemoryCache>();
            _fileReader = new FileReader(_cacheMock.Object);
        }

        [Fact]
        public async Task ReadAllLinesAsync_ShouldReturnCachedData_WhenDataIsInCache()
        {
            var fileMock = new Mock<IFormFile>();
            var cacheKey = "test.txt";
            fileMock.Setup(f => f.FileName).Returns(cacheKey);

            var cachedData = new List<string> { "Line1", "Line2" };
            object cacheEntry = cachedData;

            _cacheMock.Setup(c => c.TryGetValue(cacheKey, out cacheEntry)).Returns(true);

            var result = await _fileReader.ReadAllLinesAsync(fileMock.Object);

            Assert.Equal(cachedData, result);
            _cacheMock.Verify(c => c.TryGetValue(cacheKey, out cacheEntry), Times.Once);
        }

        [Fact]
        public async Task ReadAllLinesAsync_ShouldReadFileAndCacheData_WhenDataIsNotInCache()
        {
            var fileMock = new Mock<IFormFile>();
            var cacheKey = "test.txt";
            fileMock.Setup(f => f.FileName).Returns(cacheKey);

            var fileContent = "Line1\nLine2\nLine3";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            _cacheMock.Setup(c => c.TryGetValue(cacheKey, out It.Ref<object>.IsAny)).Returns(false);
            _cacheMock.Setup(c => c.CreateEntry(cacheKey)).Returns(Mock.Of<ICacheEntry>);

            var result = await _fileReader.ReadAllLinesAsync(fileMock.Object);

            Assert.Equal(3, result.Count);
            Assert.Contains("Line1", result);
            Assert.Contains("Line2", result);
            Assert.Contains("Line3", result);
        }
    }
}
