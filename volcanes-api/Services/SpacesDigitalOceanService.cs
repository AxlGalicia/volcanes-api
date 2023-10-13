using volcanes_api.Interfaces;

namespace volcanes_api.Services
{
    public class SpacesDigitalOceanService : ISpacesDigitalOceanService
    {
        public Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadFileAsync(string file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadFileAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
