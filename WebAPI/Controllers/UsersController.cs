using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;
using WebAPI.ViewModels;
using WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager _repo;
    private readonly ILogger<UsersController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly EmailSender _emailSender;

    public UsersController( UserManager repo, ILogger<UsersController> logger, UserManager<User> userManager, EmailSender emailSender, SignInManager<User> signInManager)
    {

        _repo = repo;
        _logger = logger;
        _userManager = userManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
    }

    // PUT api/users
    [HttpPut]
    public void Put([FromBody] User user) { 
        _repo.Update(user.Id, user);

    }
  
    [HttpPost("UpdateUser")]
    public async Task<IActionResult> Update([FromBody] User user)
    {
       var currentUser = _repo.Get(user.Id);

        currentUser.Email = user.Email;
        currentUser.UserName = user.UserName;
        currentUser.FirstName = user.FirstName;
        currentUser.LastName = user.LastName;
        currentUser.PhoneNumber = user.PhoneNumber;
        currentUser.ProfileIMG = user.ProfileIMG;
        currentUser.Address = user.Address;
        currentUser.Routes = user.Routes;   
     
        _repo.Update(currentUser.Id, currentUser);
        return Ok("User updated successfully");
    }



    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {

        if (model == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid user registration data.");
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest("User not found.");
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://localhost:7103/Login/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        var emailBody = $@"<html>
            <body>
            <p> reset your password by clicking the link below:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            </body>
            </html>";

       

        await _emailSender.SendEmailAsync(user.Email, "Reset your Password", emailBody);

        return Ok("Password reset link sent successfully.");
    }


    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {
      
        var user = await _userManager.FindByIdAsync(model.UserId);

        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return Ok("Password reset successfully.");
        }

        return BadRequest("Failed to reset password.");
    }


    // add a new user
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] RegisterViewModel model) 
    {
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid user registration data.");
        }
        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)

        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://localhost:7103/SignUp/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
            <html>
            <body>
            <p>Please confirm your email by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Confirm Email</a></p>
            </body>
            </html>";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your Email", emailBody);

            return Ok("User created successfully. Please check your email for confirmation.");
        }

        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel login)
    {
        // Validate input model
        if (login == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid user registration data.");
        }
        // Use SignInManager to check the username, password, and email confirmation in one call
        var signInResult = await _signInManager.PasswordSignInAsync(login.Username, login.Password, isPersistent: false, lockoutOnFailure: true);

        if (signInResult.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(login.Username);

            return Ok(user);
        }
        // If email confirmation is required but hasn't been confirmed
        else if (signInResult.IsNotAllowed)
        {
            return Unauthorized("Please confirm your email to continue.");
        }
        // If two-factor authentication (2FA) is required
        else if (signInResult.RequiresTwoFactor)
        {
            return Unauthorized("Two-factor authentication is required. Please complete the 2FA process.");
        }
        // Invalid username or password
        else
        {
            return BadRequest("Invalid username or password.");
        }
    }
    /// <summary>
    /// ConfirmEmail is used in the signup process to confirm the users email address using .net Identity
    /// 
    /// </summary>
    /// <returns></returns>

    // GET api/users/confirmemail
    [HttpGet("ConfirmEmail/{userId}/{token}")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
      
        var user = await _userManager.FindByIdAsync(userId); 
        // if user hasn't been found 
        if (user == null) 
        {
            return NotFound($"User with ID '{userId}' not found.");
        }
        // check if email has been verified / confirmed
        var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(token));
        if (result.Succeeded) 
        {
            return Ok("Email confirmed successfully.");
        }
        return BadRequest("Email confirmation failed");
    }

    // when to check if a username or email exists in the database
    // GET api/users/verify/{username}/{email}
    [HttpGet("verify/{username}/{email}")]
    public async Task<IActionResult> Verify(string username, string email)
    {
        var usernameExists = await _userManager.FindByNameAsync(username) != null;
        var emailExists = await _userManager.FindByEmailAsync(email) != null;

        return Ok(new { UsernameExists = usernameExists, EmailExists = emailExists });
    }

    // POST api/users/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok();
    }


    [Authorize]
    [HttpGet("GetUser")]
    public async Task<IActionResult> GetLoggedInUser()
    {
        var currentUser = await _userManager.GetUserAsync(User);
           
        
        if (currentUser == null)
        {
            return Unauthorized();
        }

        return Ok(currentUser);
    }

    //// used when logging in
    //[HttpGet("{username}/{password}")]
    //public User Login(string username, string password) {
    //    return _repo.VerifyLogin(username, password);
    //}

    // used when logging in
    /* [HttpGet("{username}/{password}")]
     public User Login(string username, string password) {
         //return _repo.VerifyLogin(username, password);
     }
 }*/

}
