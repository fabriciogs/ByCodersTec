using Application.Repositories;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Localization;
using Presentation.WebApi;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .Configure(delegate (RequestLocalizationOptions options)
    {
        var array = new CultureInfo[2] { new("pt-BR"), new("pt") };
        options.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
        options.SupportedCultures = array;
        options.SupportedUICultures = array;
    })
    .AddResponseCaching()
    .AddResponseCompression()
    .AddOptions()
    .AddDistributedMemoryCache()
    .AddHttpContextAccessor()
    .AddRazorPages();

// Services
builder.Services.AddControllersWithViews().AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true);

// Application
var connectionString = "DefaultConnection";
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(builder.Configuration.GetConnectionString(connectionString)));
builder.Services.AddHealthChecks().AddSqlServer(connectionString);

// Dependency Injection
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IWebApiClient, WebApiClient>();

// Add services to the container.
builder.Services.AddHttpClient(); // This line registers IHttpClientFactory

// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseAntiforgery();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/healthz");

app.Run();