using Amazon.Lambda;
using PeakHub.Utilities;
using System.Net.Http.Headers;
using System.Net.Mime;
using PeakHub.Models;
using Microsoft.AspNetCore.Identity;
using WebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure API client
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://localhost:5164");
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
});

// Store session into Web-Server memory
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Tools>();

// Configure the database context
builder.Services.AddDbContext<PeakHubContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services for user authentication
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<PeakHubContext>()
.AddDefaultTokenProviders();

// Add distributed memory cache (to store sessions)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

// [NEW] Add AWS S3
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
// Add AWS Lambda
builder.Services.AddAWSService<IAmazonLambda>(new AWSOptions {
    Credentials = new BasicAWSCredentials(
        "AKIA47CRV67K66X4TPO5",
        "/Ytm1pyEEjdAnvDPttBJ8rCuwpRG13laV0jXTe7q"
    ),
    Region = Amazon.RegionEndpoint.USEast1
});
builder.Services.AddScoped<Lambda_Calls>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
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