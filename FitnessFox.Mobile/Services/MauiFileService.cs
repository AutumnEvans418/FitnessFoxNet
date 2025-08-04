using FitnessFox.Components.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessFox.Mobile.Services
{
    internal class MauiFileService : IFileService
    {
        public async Task<Stream> GetLocalFileAsync(string file)
        {
            return await FileSystem.OpenAppPackageFileAsync(file);
        }
    }
}
