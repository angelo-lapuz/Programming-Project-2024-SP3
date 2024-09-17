using WebAPI.Data;
using WebAPI.Models.DataManager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7141);  // Listen on port 7141 for all IPs
});


// Add services to the container.
builder.Services.AddDbContext<PeakHubContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("PeakDBD"),
    new MySqlServerVersion(new Version(8, 0, 25))));

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<TaskManager>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<LikeManager>();
builder.Services.AddScoped<BoardManager>();
builder.Services.AddScoped<AwardManager>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
