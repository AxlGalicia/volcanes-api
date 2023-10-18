﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Net;
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

        public async Task<byte[]> DownloadFileAsync(string file)
        {
            MemoryStream ms = null;

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
                        }
                    }
                }

                    if (ms is null || ms.ToArray().Length < 1)
                    {
                        _logger.LogError(string.Format("The document '{0}' is not found", file));
                        return null;
                    }

                    //throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

                return ms.ToArray();
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

    }
}
