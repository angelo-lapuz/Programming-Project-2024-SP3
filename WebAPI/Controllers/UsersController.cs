using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Models.DataManager;
using WebAPI.ViewModels;
using WebAPI.Utilities;
using System.Text;



namespace WebAPI.Controllers;

// See here for more information:
// https://learn.microsoft.com/en-au/aspnet/core/web-api/?view=aspnetcore-7.0

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CustomUserManager _repo;
    private readonly ILogger<UsersController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly EmailSender _emailSender;

    public UsersController( CustomUserManager repo, ILogger<UsersController> logger, UserManager<User> userManager, EmailSender emailSender, SignInManager<User> signInManager)
    {

        _repo = repo;
        _logger = logger;
        _userManager = userManager;
        _emailSender = emailSender;
        _signInManager = signInManager;
    }


    // PUT api/users
    // Updates user information
    [HttpPut]
    public void Put([FromBody] User user)
    {
        _repo.Update(user);
    }

    // GET: api/users/{id}
    // Retrieves a specific user by ID
    [HttpGet("{id}")]
    public async Task<User> GetUser(string id)
    {
        return _repo.Get(id);
    }

    // called from the front when a user is attempting to reset their password
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {
        // checking that the model state is valid
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid user registration data.");
        }

        // ensurethat the email is valid
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest("User not found.");
        }

        // generate reset token (.net Identity) andd the reset link for the user
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://taspeaks-fpf6hdaqgyazduge.canadacentral-01.azurewebsites.net/Login/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        // creating basic email body
        var emailBody = $@"<html>
            <body>
            <p> reset your password by clicking the link below:</p>
            <p><a href='{resetLink}'>Reset Password</a></p>
            </body>
            </html>";

        // sending the email to the user, returning a success message
        await _emailSender.SendEmailAsync(user.Email, "Reset your Password", emailBody);
        return Ok("Password reset link sent successfully.");
    }

    // reset password is called by the link generated in the ForgotPassword method above 
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
    {

        // check that the user is valid from the model passed in
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        // reset the password using the token and the new password
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return Ok("Password reset successfully.");
        }

        // can occur if the url is corrupted or the token is invalid
        return BadRequest("Failed to reset password.");
    }


    // called when creating a new user from the front end
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] RegisterViewModel model) 
    {
        // ensuring viewmodel is valid - returning error
        if (model == null || !ModelState.IsValid)
        {
            return BadRequest("Invalid user registration data.");
        }
        // creating a new user object
        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = false
        };

        // creating the user in the database and assigning a user role
        var result = await _userManager.CreateAsync(user, model.Password);
        await _userManager.AddToRoleAsync(user, "USER");

        // creating user should never fail anyway
        if (result.Succeeded)

        {
            // generate the email confirmation token and encode it (for security) and
            // in some instances escape coddes can be contained within the generated token (edge case)
            // which will cause a malformed url
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var base64Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));

            // creating the callback url the user must click to confirm their email
            var confirmationLink = $"https://taspeaks-fpf6hdaqgyazduge.canadacentral-01.azurewebsites.net/SignUp/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(base64Token)}";

            // creating the email
            var emailBody = $@"
            <html>
            <body>
            <p>Please confirm your email by clicking the link below:</p>
            <p><a href='{confirmationLink}' target='_self'>Confirm Email</a></p>
            </body>
            </html>";

            // sending the email to the user returning the confirmation of successful account creation
            await _emailSender.SendEmailAsync(user.Email, "Confirm your Email", emailBody);
            return Ok("User created successfully. Please check your email for confirmation.");
        }   
        return BadRequest(result.Errors);
    }


    // called by the front when the user is logging in
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

        // if the user has successfully logged in get and return the user object
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

    // called by the front when the user is confirming their email
    // GET api/users/confirmemail
    [HttpGet("ConfirmEmail/{userId}/{token}")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        // get the user by the id passed in
        var user = await _userManager.FindByIdAsync(userId); 
        // if user hasn't been found 
        if (user == null) 
        {
            return NotFound($"User with ID '{userId}' not found.");
        }

        // decode the token and confirm the email return the appropriate response
        var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(decodedToken));
        if (result.Succeeded) 
        {
            return Ok("Email confirmed successfully.");
        }
        return BadRequest("Email confirmation failed");
    }

    // when to check if a username or email exists in the database - used when creating an account
    // GET api/users/verify/{username}/{email}
    [HttpGet("verify/{username}/{email}")]
    public async Task<IActionResult> Verify(string username, string email)
    {
        // check if the users email or username already exists in the database - return results
        var usernameExists = await _userManager.FindByNameAsync(username) != null;
        var emailExists = await _userManager.FindByEmailAsync(email) != null;

        return Ok(new { UsernameExists = usernameExists, EmailExists = emailExists });   
    }


    // check if the password entered by the user is correct - used when loggin in and changing password
    [HttpGet("VerifyPassword/{password}")]
    public async Task<IActionResult> VerifyPassword(string password)
    {
        // get the user from the current user logged in - check the password entered by the user return result
        var user = await _userManager.GetUserAsync(User);
        var checkPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!checkPassword)
        {
            return null;
        }
        return Ok();
    }

    // called when the user is changing their password
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        // get the user from the model passed in - change the password and return the result
        var user =  await GetUser(model.ID);
        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

        if (result.Succeeded)
        {
            return Ok("Successfully changed password");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    // DELETE api/Users/1
    // Deletes a specific User by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        // get the user from the ID passed in - delete the user and return the result
        var user = await GetUser(id);
        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok("User deleted successfully");
        }
        else
        {
            return BadRequest(result.Errors);
        }
    }

    // Deletes multiple posts based on a list of IDs
    // DELETE api/Users/DeleteUsers
    [HttpPost("DeleteUsers")]
    public void DeleteUsers(List<string> ids)
    {
         _repo.DeleteAll(ids);
    }

    // POST api/users/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // log the user out and return ok
        await _signInManager.SignOutAsync();
        return Ok();
    }


}
