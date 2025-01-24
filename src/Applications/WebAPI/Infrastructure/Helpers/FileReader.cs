using WebAPI.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Infrastructure.Helpers
{
    public class FileReader : IFileReader
    {
        private readonly IMemoryCache _cache;
        const string cacheKey = "FileDataCache";

        public FileReader(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<List<string>> ReadAllLinesAsync()
        {

            if (_cache.TryGetValue(cacheKey, out List<string> cachedData))
            {
                return cachedData;
            }

            var data1Path = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Helpers", "data_1.txt");
            var data2Path = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Helpers", "data_2.txt");

            if (!File.Exists(data1Path) || !File.Exists(data2Path))
            {
                throw new FileNotFoundException("Arquivos de dados não encontrados.");
            }

            var data1 = await File.ReadAllLinesAsync(data1Path);
            var data2 = await File.ReadAllLinesAsync(data2Path);

            var allData = data1.Concat(data2)
            .Where(line => !string.IsNullOrWhiteSpace(line) && line.Length >= 95)
            .ToList();

            _cache.Set(cacheKey, allData, TimeSpan.FromHours(24));
            return allData;
        }
    }
}
