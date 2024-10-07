﻿using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeakHub.ViewModels;
using PeakHub.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Text;

namespace PeakHub.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient _httpClient => _clientFactory.CreateClient("api");

        public ProfileController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            // gets user object from db
            var response = await _httpClient.GetAsync($"api/users/GetUser");
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            ViewBag.UserName = user.UserName;
            ViewBag.Peaks = user.Peaks;
            ViewBag.Awards = user.Awards;

            return View();
        }

        public async Task<IActionResult> Edit(EditViewModel model)
        {
            // gets user object from db
            var response = await _httpClient.GetAsync($"api/users/GetUser");
            var user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            user.FirstName = model.FirstName;

            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var updatedUser = await _httpClient.PutAsync($"api/users/", content);
            
            return View();
        }
    }
}
