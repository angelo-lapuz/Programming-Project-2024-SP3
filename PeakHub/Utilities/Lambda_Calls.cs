using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.AspNetCore.Mvc;

namespace PeakHub.Utilities {
    public class Lambda_Calls {
        private readonly IAmazonLambda _client;
        private readonly ILogger<Lambda_Calls> _logger;

        public Lambda_Calls(IAmazonLambda client, ILogger<Lambda_Calls> logger) {
            _client = client;
            _logger = logger;
        }

        public async Task<string> UploadToS3(string bucket, string ID, byte[] fileContent, string mimeType) {
            try {
                var payload = new {
                    Bucket = bucket,
                    ID = ID,
                    FileContent = fileContent,
                    MimeType = mimeType
                };


                var requst = new InvokeRequest {
                    FunctionName = "peakhub-upload-content",
                    Payload = System.Text.Json.JsonSerializer.Serialize(payload),
                };

                var response = await _client.InvokeAsync(requst);
                var responseString = System.Text.Json.JsonSerializer.Deserialize<string>(response.Payload);

                if (responseString == null) throw new Exception("Error Occured!");

                else if (responseString.Contains("[Error]")) throw new Exception(responseString);

                _logger.LogInformation($"Success! A Brilliant Success! Link: [{responseString}]");
                return responseString;
            }
            catch (Exception ex) {
                _logger.LogError($"[{ex.Message}]");
                return null;
            }
        }

        public async Task<List<string>> GetPeakPics(string mountName) {
            
            try {
                var payload = new { MountName = mountName };

                var request = new InvokeRequest {
                    FunctionName = "peakhub-get-peaks",
                    Payload = System.Text.Json.JsonSerializer.Serialize(payload)
                };

                var response = await _client.InvokeAsync(request);
                var responseList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response.Payload);

                if (responseList == null) throw new Exception("Error Occured!");
                else if (responseList[0].Contains("[Error]")) throw new Exception(responseList[0]);

                foreach (string responseString in responseList) {
                    _logger.LogInformation($"Success! A Brilliant Success! Link: [{responseString}]");
                }
                return responseList;
            }
            catch (Exception ex) {
                _logger.LogError($"[{ex.Message}]");
                return null;
            }
        }
    }
}
