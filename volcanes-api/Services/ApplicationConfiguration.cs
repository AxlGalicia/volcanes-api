using volcanes_api.Interfaces;

namespace volcanes_api.Services
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string AccessKey { get; init; }
        public string SecretAccessKey { get; init; }
        public string? SessionToken { get; init; }
        public string BucketName { get; init; }
        public string? Region { get; init; }
        public string? ServiceURL { get; init; }
        public string? SignatureVersion { get; init; }

        public ApplicationConfiguration(IConfiguration configuration)
        {
            AccessKey = configuration["AccessKey"];
            SecretAccessKey = configuration["SecretAccessKey"];
            SessionToken = configuration["SessionToken"];
            BucketName = configuration["BucketName"];
            Region = configuration["Region"];
            ServiceURL = configuration["ServiceURL"];
            SignatureVersion = configuration["SignatureVersion"];
            
            // AccessKey = Environment.GetEnvironmentVariable("AccessKey");
            // SecretAccessKey = Environment.GetEnvironmentVariable("SecretAccessKey");
            // SessionToken = Environment.GetEnvironmentVariable("SessionToken");
            // BucketName = Environment.GetEnvironmentVariable("BucketName");
            // Region = Environment.GetEnvironmentVariable("Region");
            // ServiceURL = Environment.GetEnvironmentVariable("ServiceURL");
            // SignatureVersion = Environment.GetEnvironmentVariable("SignatureVersion");
        }
    }
}
