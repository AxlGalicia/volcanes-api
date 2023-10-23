using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Net;
using volcanes_api.Interfaces;
using volcanes_api.Models.DTOs;

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
        public async Task<ResponseUpload> UploadFileAsync(IFormFile file)
        {
            try
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);
                    var newName = changeFileName(file);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = newName,
                        BucketName = _bucketName,
                        ContentType = file.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(_S3Client);

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return new ResponseUpload(){responseStatus = true,newName = newName};
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Ocurrio un error al intentar cargar archivo: ", e.Message);
                //Console.WriteLine("Ocurrio un error: ", e.Message);
                return new ResponseUpload(){responseStatus = false,newName = ""};
            }
        }
        public async Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            if (!string.IsNullOrEmpty(versionId))
                request.VersionId = versionId;

            if (!IsFileExists(fileName, versionId).Result)
            {
                return false;
            }
            await _S3Client.DeleteObjectAsync(request);
            return true;
        }

        public async Task<ArchivoDescargadoDTO> DownloadFileAsync(string file)
        {
            var archivoDescargadoDto = new ArchivoDescargadoDTO();
            var tipoContenido = "";
            MemoryStream ms = null;
            //MemoryStream stream = new MemoryStream();

            try
            {
                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = file
                };

                using (var response = await _S3Client.GetObjectAsync(getObjectRequest))
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            await response.ResponseStream.CopyToAsync(ms);
                            //await ms.CopyToAsync(stream);
                            archivoDescargadoDto.tipoContenido = response.Headers.ContentType;

                        }
                    }
                }

                    if (ms is null || ms.ToArray().Length < 1)
                    {
                        _logger.LogError(string.Format("The document '{0}' is not found", file));
                        return null;
                    }
                    
                    //throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));
                // MemoryStream stream = new MemoryStream();
                // stream.Write(ms.ToArray());
                //
                // var archivo = new FormFile(stream,0,stream.Length,"Imagen",file)
                // {
                //     Headers = new HeaderDictionary(),
                //     ContentType = tipoContenido
                // };
                //archivo.ContentType = tipoContenido;
                archivoDescargadoDto.contenido = ms.ToArray();
                return archivoDescargadoDto;
            }
            catch (Exception e)
            {
                _logger.LogError("Ocurrio un error en el proceso: ", e.Message);
                return null;
                //throw;
            }
            
        }

        public async Task<bool> IsFileExists(string fileName, string versionId)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    VersionId = !string.IsNullOrEmpty(versionId) ? versionId : null
                };

                var response = await _S3Client.GetObjectMetadataAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
                {
                    if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                        return false;

                    else if (string.Equals(awsEx.ErrorCode, "NotFound"))
                        return false;
                }

                return false;
            }
        }

        private string changeFileName(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var newName = Guid.NewGuid().ToString()+extension;
            return newName;
        }

    }
}
