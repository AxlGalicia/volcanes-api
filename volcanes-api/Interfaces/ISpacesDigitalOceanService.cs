using volcanes_api.Models.DTOs;

namespace volcanes_api.Interfaces
{
    public interface ISpacesDigitalOceanService
    {

        Task<ArchivoDescargadoDTO> DownloadFileAsync(string file);

        Task<ResponseUpload> UploadFileAsync(IFormFile file);

        Task<bool> DeleteFileAsync(string fileName, string versionId = "");

    }
}
