using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using volcanes_api.Interfaces;

namespace volcanes_api.tests.Mocks
{
    internal class MockSpacesDigitalOceanService : ISpacesDigitalOceanService
    {
        public Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            return Task.FromResult(true);
        }

        public Task<byte[]> DownloadFileAsync(string file)
        {
            return Task.FromResult(new byte[] { });
        }

        public Task<bool> UploadFileAsync(IFormFile file)
        {
            return Task.FromResult(true);
        }
    }
}
