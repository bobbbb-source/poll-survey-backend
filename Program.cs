using Microsoft.EntityFrameworkCore;
using PollSurveyBuilder.API.Data;
using PollSurveyBuilder.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000",
                "https://localhost:51432"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var maxRetries = 10;
    var retryDelay = TimeSpan.FromSeconds(5);

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Exception) when (attempt < maxRetries)
        {
            Console.WriteLine(
                $"Database not ready. Retrying migration " +
                $"({attempt}/{maxRetries})..."
            );

            Thread.Sleep(retryDelay);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.MapHub<PollHub>("/pollhub");

app.Run();