namespace WebAPI.Infrastructure.Interfaces.Helpers
{
    public interface IFileReader
    {
        Task<List<string>> ReadAllLinesAsync(IFormFile file);
    }
}
