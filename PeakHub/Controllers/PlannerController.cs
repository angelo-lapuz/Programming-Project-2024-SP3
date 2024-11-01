using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Utilities;
using PeakHub.Models;

namespace PeakHub.Controllers {
    public class PlannerController : Controller {
        private readonly Tools _tools;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlannerController> _logger;
        private string userID => HttpContext.Session.GetString("UserId");


        public PlannerController(Tools tools, ILogger<PlannerController> logger, IHttpClientFactory httpClientFactory) {
            _tools = tools;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("api");
        }
    // index displays the planner page - with the map
   
    public async Task<IActionResult> Index() {
            HttpContext.Session.SetString("LastPage", "Planner");

            var getAllPeaksResponse = await _httpClient.GetAsync("api/Peaks");
            if (getAllPeaksResponse.IsSuccessStatusCode) {
                var allpeaks = await getAllPeaksResponse.Content.ReadAsStringAsync();
                var peaks = JsonConvert.DeserializeObject<List<Peak>>(allpeaks);

                ViewBag.Peaks = JsonConvert.SerializeObject(peaks);
            }


            // getthe current user (if logged in) and set the viewbag values accordingly
            User user =  await _tools.GetUser(userID);

            if (user != null) {
                ViewBag.Routes = user.Routes;
                ViewBag.userL = true;
                ViewBag.userPeaks = Newtonsoft.Json.JsonConvert.SerializeObject(user.UserPeaks.Select(up=>up.Peak));
            }
            return View();
        }


        // called from the front end when the user saves a route (maximum of 3) 
        [HttpPost]
        public async Task<IActionResult> SaveRoute([FromBody] SaveRouteDTO route)
        {
            // get the current user ( ensures logged in)
            var user = await _tools.GetUser(userID);
            if (user == null)
            {
                return Unauthorized();
            }
            // ensures the user stays logged in according to javascript
            ViewBag.userL = true;

            // Deserialize existing routes, or initialize an empty list if none exist
            var routes = string.IsNullOrEmpty(user.Routes)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(user.Routes);

            // Ensure the user has not exceeded the limit of 3 saved routes
            if (routes.Count >= 3) {
                return BadRequest(new { error = "You have reached the maximum number of saved routes." });
            }

            // Add the new route (not the entire collection of routes, just the newly drawn route)
            routes.Add(JsonConvert.SerializeObject(route.Route));

            // Save the updated routes back to the user data
            user.Routes = JsonConvert.SerializeObject(routes);

            var result = await _httpClient.PutAsJsonAsync("api/users", user);

            // Return the appropriate response
            if (result.IsSuccessStatusCode) {
                return Ok(new { Ok = true, message = "Route saved successfully" });
            } else {
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }


        // called when the user attempts to delete a route from the front end
        [HttpPost]
        public async Task<IActionResult> DeleteRoute(int routeIndex)
        {
            // ensures the user is logged in
            var user = await _tools.GetUser(userID);
            if (user == null)
            {
                return Unauthorized();
            }

            // get the curretn user's routes
            var routes = JsonConvert.DeserializeObject<List<string>>(user.Routes);

            // Ensure the route index is valid
            if (routeIndex < 0 || routeIndex >= routes.Count)
            {
                return BadRequest("Invalid route index");
            }
            // remove the route from the list - reserialize the list and update the user
            routes.RemoveAt(routeIndex);
            user.Routes = JsonConvert.SerializeObject(routes);

            var result = await _httpClient.PutAsJsonAsync("api/users", user);

            // this should never fail - but just in case
            if (!result.IsSuccessStatusCode) {
                return StatusCode(500, "Failed to update user");
            }
            // alert user of success
            else
            {
                return Ok(new { Ok = true, message = "Route Deleted successfully" });
            }
        }

        // saveRoute DTO used when saving a route - takes in a list of lists of latlngs (multiple coordinates)
        public class SaveRouteDTO
        {
            public List<List<LatLng>> Route { get; set; }
        }

        // LatLng class used to store the lat and lng of a coordinate - used when saving a route
        public class LatLng
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }

}
