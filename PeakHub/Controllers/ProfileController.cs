using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using System.Text;
using PeakHub.Utilities;
using Amazon.S3;
using Amazon.S3.Model;

namespace PeakHub.Controllers {
    public class ProfileController : Controller {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProfileController> _logger;
        private readonly Tools _tools;
        private readonly IAmazonS3 _s3;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");
        private string userID => HttpContext.Session.GetString("UserId");

        public ProfileController(IHttpClientFactory clientFactory, IAmazonS3 s3, ILogger<ProfileController> logger, Tools tools)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _tools =  tools;
            _s3 = s3;
        }

        // Get Presigned URL [Profile IMG Edit]
        [HttpPost]
        public IActionResult GetPresignedURL([FromBody] PresignedURLDataModel data) {
            string imgID = data.ID, fileType = data.FileType;

            if (string.IsNullOrEmpty(imgID)) return Json(new { success = false, message = "ImageID is Null" });
            if (string.IsNullOrEmpty(userID)) return Json(new { success = false, message = "UserID is Null" });
            if (string.IsNullOrEmpty(fileType)) return Json(new { success = false, message = "File Type is Null" });

            try {
                string fileExtension = _tools.GetFileExtension(fileType);

                string key = $"{userID}/profile/{imgID}";
                if (!string.IsNullOrEmpty(fileExtension)) { key += $"{fileExtension}"; }

                var request = new GetPreSignedUrlRequest {
                    BucketName = "peakhub-user-content",
                    Key = key,
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    Verb = HttpVerb.PUT,
                    ContentType = fileType
                };

                string presignedURL = _s3.GetPreSignedURL(request);
                Console.WriteLine($"Key = [{key}] \nURL = [{presignedURL}]");
                return Json(new { success = true, url = presignedURL, key = key });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = $"Error [{ex.Message}]" });
            }
        }

        // Index page for user profile - displays user details, peaks and awards
        public async Task<IActionResult> Index()
        {
            // ensures that the user is currently logged in
            if (userID == null) return RedirectToAction("Login", "Login");

            // get the current user
            User user = await _tools.GetUser(userID);

            // get peaks
            var getPeakResponse = await _httpClient.GetAsync("api/peaks");
            var peakData = await getPeakResponse.Content.ReadAsStringAsync();
            var peaks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Peak>>(peakData);

            // set the image for the user profile
            string defaultImg = "https://peakhub-user-content.s3.amazonaws.com/default.jpg";

            // put the user details, peaks and awards into the viewbag
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.Peaks = user.UserPeaks.Select(up => up.Peak).ToList();
            ViewBag.Awards = user.UserAwards.Select(ua => ua.Award).ToList();
            ViewBag.ProfileImg = string.IsNullOrEmpty(user.ProfileIMG) ? defaultImg : user.ProfileIMG;
            ViewBag.totalCompleted = user.UserPeaks.Count;
            ViewBag.AllPeaks = peaks;

            return View("Index");
        }

        // returns the EditDetails page for the user - allows the user to edit their details
        [HttpGet]
        public async Task<IActionResult> EditDetails() => View();


        // called when the user is attempting to edit their details from the front
        [HttpPost]
        public async Task<IActionResult> EditDetails(EditDetailsViewModel model) {
            // get the current user
            User user = await _tools.GetUser(userID);

            // update the user details with the new details - call api to update the user
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.ProfileIMG = model.ProfileIMG;

            var result = await _httpClient.PutAsJsonAsync("api/users", user);

            // if the user was updated successfully, return the view else return an error
            if (result.IsSuccessStatusCode) {
                TempData["SuccessMessage"] = "Your details have been successfully updated.";
                return await Index();
            } else {
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }

        // returns the ChangePassword page for the user - allows the user to change their password
        [HttpGet]
        public async Task<IActionResult> ChangePassword() {
            ViewBag.PasswordChangeStatus = null;
            return View();
        }

        // called when the user is attempting to change their password from the front
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model) {
            if (ModelState.IsValid) {
                model.ID = userID;

                // convert viewmodel to json to be sent to api/users/ChangePassword
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/users/ChangePassword", content);

                // if password failed to change display error message
                if (!response.IsSuccessStatusCode) {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(errorContent);

                    foreach (var error in errors) {
                        foreach (var errorMessage in error.Value) {
                            ModelState.AddModelError(error.Key, errorMessage);
                        }
                    }
                }
                else {
                    TempData["SuccessMessage"] = "Password Changed Successfully.";
                }
            }

            return View(model);
        }
    }
}
