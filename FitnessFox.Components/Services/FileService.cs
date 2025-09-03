namespace FitnessFox.Components.Services
{
    public class FileService : IFileService
    {
        public Task<Stream> GetLocalFileAsync(string file) => Task.FromResult<Stream>(File.OpenRead(file));
    }
}
