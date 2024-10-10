using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace PeakHub.Utilities {
    public class AWS_SecretsManager {
        public readonly IAmazonSecretsManager _manager;
        public AWS_SecretsManager() {
            string aKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            string sKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");

            var credentials = new BasicAWSCredentials(aKey, sKey);
            _manager = new AmazonSecretsManagerClient(credentials, Amazon.RegionEndpoint.USEast1);
        }

        public async Task<AWSCredentials> GetCreds() {
            try {
                var request = new GetSecretValueRequest{ SecretId = "peakhub-aws-creds" };
                var response = await _manager.GetSecretValueAsync(request);

                if (response.SecretString == null) throw new Exception("Secret String was NULL");

                var secretJson = 
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(response.SecretString);

                return new BasicAWSCredentials(secretJson["aws_access_key_id"], secretJson["aws_secret_access_key"]);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error retrieving secret: {ex.Message}");
                throw;
            }
        }
    }
}
