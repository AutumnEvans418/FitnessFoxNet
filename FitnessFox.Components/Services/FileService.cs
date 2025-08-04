namespace FitnessFox.Components.Services
{
    public class FileService : IFileService
    {
        public async Task<Stream> GetLocalFileAsync(string file) => File.OpenRead(file);
    }
}
