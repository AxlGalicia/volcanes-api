using Amazon.S3;
using Amazon.S3.Transfer;
using volcanes_api.Interfaces;

namespace volcanes_api.Services
{
    public class SpacesDigitalOceanService : ISpacesDigitalOceanService
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _S3Client;
        private readonly AmazonS3Config _S3Config;
        private readonly ILogger<SpacesDigitalOceanService> _logger;

        public SpacesDigitalOceanService(IConfiguration configuration,
                                         ILogger<SpacesDigitalOceanService> logger)
        {
            var appConfiguration = new ApplicationConfiguration(configuration);
            _bucketName = appConfiguration.BucketName;
            _S3Config = new AmazonS3Config()
            {
                ServiceURL = appConfiguration.ServiceURL,
                SignatureVersion = appConfiguration.SignatureVersion
            };
            _S3Client = new AmazonS3Client(appConfiguration.AccessKey,
                                           appConfiguration.SecretAccessKey,
                                           _S3Config);
            _logger = logger;
        }
        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = file.FileName,
                        BucketName = _bucketName,
                        ContentType = file.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(_S3Client);

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Ocurrio un error al intentar cargar archivo: ", e.Message);
                //Console.WriteLine("Ocurrio un error: ", e.Message);
                return false;
            }
        }
        public Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadFileAsync(string file)
        {
            throw new NotImplementedException();
        }


    }
}
