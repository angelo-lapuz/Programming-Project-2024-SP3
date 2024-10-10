using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace PeakHub.Utilities {
    public class AWS_S3 {
        private IAmazonS3 _s3;
        public AWS_S3(AWSCredentials creds) =>
            _s3 = new AmazonS3Client(creds, Amazon.RegionEndpoint.USEast1);

        public async Task<string> UploadToS3(string bucketType, string ID, string filePath, string mimeType) {
            try {
                string fileExtension = mimeType switch {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/gif" => ".gif",
                    "image/webp" => ".webp",
                    "video/mp4" => ".mp4",
                    "video/webm" => ".webm",
                    _ => throw new Exception("Unsupported MIME type")
                };

                var fileTransferUtility = new TransferUtility(_s3);
                string key = $"{ID}{fileExtension}";

                var uploadRequest = new TransferUtilityUploadRequest {
                    BucketName = $"peakhub-{bucketType}-content",
                    FilePath = filePath,
                    Key = key,
                    ContentType = mimeType
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
                Console.WriteLine($"Success! A Brilliant Success! File has been Uploaded");

                return $"https://peakhub-{bucketType}-content.s3.amazonaws.com/{key}";
            }
            catch (Exception ex) {
                Console.WriteLine($"Error Occured: [{ex.Message}]");
                return null;
            }

        }

        public async Task<List<string>> GetPeakImages(string mountName) {
            try {
                string prefix = $"peaks/{mountName}";
                
                var request = new ListObjectsV2Request { 
                    BucketName = "peakhub",
                    Prefix = prefix,
                };
                var response = await _s3.ListObjectsV2Async(request);
                var imageLinks = new List<string>();

                foreach (var image in response.S3Objects) {
                    imageLinks.Add($"https://peakhub.s3.amazonaws.com/{image.Key}");
                }

                return imageLinks;
            }
            catch (Exception ex) { 
                Console.WriteLine($"Error Occured: [{ex.Message}]"); 
                return null; 
            }
        }
    }
}
