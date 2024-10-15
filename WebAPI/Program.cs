using WebAPI.Data;
using WebAPI.Models.DataManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

using WebAPI.Utilities;
using WebAPI.Models; 
using static WebAPI.Utilities.EmailSender;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PeakHubContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseSqlite(builder.Configuration.GetConnectionString("PeakDBD"));
});

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<SignInManager<User>>();  
builder.Services.AddScoped<PeakManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<LikeManager>();
builder.Services.AddScoped<BoardManager>();
builder.Services.AddScoped<AwardManager>();
builder.Services.AddScoped<EmailSender>();

// Identity configuration with custom User model
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<PeakHubContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCulture = new[] { new System.Globalization.CultureInfo("en-AU") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-AU");
    options.SupportedCultures = supportedCulture;
    options.SupportedUICultures = supportedCulture;
});

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });


// KEEP ME
builder.WebHost.ConfigureKestrel(server =>
{
    server.ListenAnyIP(7141);
    server.ListenAnyIP(5164);
});


var app = builder.Build();

// Seed data.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

// Authentication and Authorization
app.UseAuthentication();  
app.UseAuthorization();
app.MapControllers();
app.Run();
