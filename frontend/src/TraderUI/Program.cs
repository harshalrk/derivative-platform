using Microsoft.AspNetCore.DataProtection;
using TraderUI.Components;
using TraderUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure data protection for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));
}

// Configure HttpClient for API calls
var apiUrl = builder.Configuration["ApiUrl"] ?? "http://localhost:5000";
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
});

// Add session service
builder.Services.AddScoped<SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add simple API endpoint for session creation
app.MapPost("/api/session", async (SessionRequest request, ApiClient apiClient) =>
{
    try 
    {
        var response = await apiClient.SetSessionAsync(request.Name);
        if (response.IsSuccess)
        {
            return Results.Ok(response.Data);
        }
        return Results.BadRequest(response.ErrorMessage);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

public record SessionRequest(string Name);
