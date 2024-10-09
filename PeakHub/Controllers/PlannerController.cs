﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.Utilities;
using PeakHub.Views.Login;
using System.Net.Http;
using PeakHub.Models;
using Azure;


namespace PeakHub.Controllers
{
    public class PlannerController : Controller
    {
        private readonly Tools _tools;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlannerController> _logger;


        public PlannerController(Tools tools, ILogger<PlannerController> logger, IHttpClientFactory httpClientFactory)
        {
            _tools = tools;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("api");
        }

    public async Task<IActionResult> Index()
        {
            var getAllPeaksResponse = await _httpClient.GetAsync("api/Peaks");
            if (getAllPeaksResponse.IsSuccessStatusCode)
            {
                var allpeaks = await getAllPeaksResponse.Content.ReadAsStringAsync();
                var peaks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Peak>>(allpeaks);

                ViewBag.Peaks = Newtonsoft.Json.JsonConvert.SerializeObject(peaks);
            }


            User user =  _tools.GetUser().GetAwaiter().GetResult();

            if (user != null)
            {
                ViewBag.Routes = user.Routes;
                ViewBag.userL = true;
                ViewBag.userPeaks = Newtonsoft.Json.JsonConvert.SerializeObject(user.Peaks);


            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> SaveRoute([FromBody] SaveRouteDTO route)
        {
            var user = await _tools.GetUser();

            if (user == null)
            {
                return Unauthorized();
            }

            ViewBag.userL = true;

            // Deserialize existing routes, or initialize an empty list if none exist
            var routes = string.IsNullOrEmpty(user.Routes)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(user.Routes);

            // Ensure the user has not exceeded the limit of 3 saved routes
            if (routes.Count >= 3)
            {
                return BadRequest(new { error = "You have reached the maximum number of saved routes." });
            }

            // Add the new route (not the entire collection of routes, just the newly drawn route)
            routes.Add(JsonConvert.SerializeObject(route.Route));

            // Save the updated routes back to the user data
            user.Routes = JsonConvert.SerializeObject(routes);

            var result = await _httpClient.PostAsJsonAsync("api/users/UpdateUser", user);

            // Return the appropriate response
            if (result.IsSuccessStatusCode)
            {
                return Ok(new { Ok = true, message = "Route saved successfully" });
            }
            else
            {
                return BadRequest(new { error = "Failed to update user with new route." });
            }
        }



        [HttpPost]
        public async Task<IActionResult> DeleteRoute(int routeIndex)
        {
            var user = _tools.GetUser().GetAwaiter().GetResult();

            if (user == null)
            {
                return Unauthorized();
            }

            var routes = JsonConvert.DeserializeObject<List<string>>(user.Routes);
            
            if(routeIndex < 0 || routeIndex >= routes.Count)
            {
                return BadRequest("Invalid route index");
            }
            routes.RemoveAt(routeIndex);
            user.Routes = JsonConvert.SerializeObject(routes);

            var result = await _httpClient.PutAsJsonAsync("api/users/", user);

            if (!result.IsSuccessStatusCode) {
                return StatusCode(500, "Failed to update user");
            }
            else
            {
                return Ok(new { Ok = true, message = "Route Deleted successfully" });
            }
        }


        public class SaveRouteDTO
        {
            public List<List<LatLng>> Route { get; set; }
        }
        public class LatLng
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }

}
