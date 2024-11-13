using Amazon.Lambda;
using PeakHub.Utilities;
using System.Net.Http.Headers;
using System.Net.Mime;
using PeakHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using WebAPI.Models.DataManager;
using WebAPI.Utilities;
using static WebAPI.Utilities.EmailSender;

var builder = WebApplication.CreateBuilder(args);


// Configure API client
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://taspeak-bba5hncge9f0e5c0.australiaeast-01.azurewebsites.net");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
});

// Store session into Web-Server memory - user will stay logged in for 2 hours
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddHttpContextAccessor();

// Configure the database context
builder.Services.AddDbContext<PeakHubContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlServer(builder.Configuration.GetConnectionString("PeakDBD"));
});

// Add Identity services for user authentication
builder.Services.AddIdentity<WebAPI.Models.User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<PeakHubContext>()
.AddDefaultTokenProviders();

// adding scoped Datamanager services
builder.Services.AddScoped<PeakManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<LikeManager>();
builder.Services.AddScoped<BoardManager>();
builder.Services.AddScoped<AwardManager>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<CustomUserManager>();

// adding Transient services ( new version every time its called)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// adding scoped utilities / helpers
builder.Services.AddScoped<Lambda_Calls>();
builder.Services.AddScoped<Tools>();

// Add distributed memory cache (to store sessions)
builder.Services.AddDistributedMemoryCache();


// [NEW] Add AWS S3 // getting the credentials from environment variables
builder.Services.AddSingleton<IAmazonS3>(service => {
    var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
    var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

    if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey)) {
        throw new Exception("AWS credentials are missing in environment variables.");
    }

    var credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
    return new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1); 
});

// [TEMP]
// Add AWS Lambda with credentials
builder.Services.AddAWSService<IAmazonLambda>(new AWSOptions {
    Credentials = new BasicAWSCredentials(
        "AKIA47CRV67K66X4TPO5",
        "/Ytm1pyEEjdAnvDPttBJ8rCuwpRG13laV0jXTe7q"
    ),
    Region = Amazon.RegionEndpoint.USEast1
});

// configuring controllers - ignore reference loops - prevent infinite loops when loading from database
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });


// Add controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS for enhanced security
app.UseHttpsRedirection();

// Serve static files (like images, CSS, JavaScript) from the wwwroot folder
app.UseStaticFiles();

// Enable routing capabilities to match incoming requests to proper endpoints
app.UseRouting();

// Enable session management, allowing data to be stored across requests for a user
app.UseSession();

// Add authentication middleware to check and authenticate users' identity
app.UseAuthentication();

// Add authorization middleware to enforce access control based on authenticated users' permissions
app.UseAuthorization();

// Custom Route - Forums
app.MapControllerRoute(
    name: "forum",
    pattern: "Forum/{boardID:int}",
    defaults: new { controller = "Forum", action = "Index" }
);

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();