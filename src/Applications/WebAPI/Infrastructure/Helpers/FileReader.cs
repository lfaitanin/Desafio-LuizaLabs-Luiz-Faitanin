using WebAPI.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Infrastructure.Helpers
{
    public class FileReader : IFileReader
    {
        private readonly IMemoryCache _cache;

        public FileReader(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<List<string>> ReadAllLinesAsync(IFormFile file)
        {
            var cacheKey = file.FileName;

            if (_cache.TryGetValue(cacheKey, out List<string> cachedData))
            {
                return cachedData;
            }

            using var reader = new StreamReader(file.OpenReadStream());
            var content = (await reader.ReadToEndAsync())
                        .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                        .ToList();

            _cache.Set(cacheKey, content, TimeSpan.FromHours(24));
            return content;
        }
    }
}
