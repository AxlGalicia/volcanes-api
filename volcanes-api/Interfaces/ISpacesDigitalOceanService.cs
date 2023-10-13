namespace volcanes_api.Interfaces
{
    public interface ISpacesDigitalOceanService
    {

        Task<byte[]> DownloadFileAsync(string file);

        Task<bool> UploadFileAsync(IFormFile file);

        Task<bool> DeleteFileAsync(string fileName, string versionId = "");

    }
}
