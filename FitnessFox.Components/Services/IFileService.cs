
namespace FitnessFox.Components.Services
{
    public interface IFileService
    {
        Task<Stream> GetLocalFileAsync(string file);
    }
}