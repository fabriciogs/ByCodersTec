using Application.Repositories;
using Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
var logger = LoggerFactory.Create(config =>
{
    config.ClearProviders();
    config.AddDebug();
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
}).CreateLogger("Program");

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application
var connectionString = "DefaultConnection";
logger.LogInformation($"Trying to open connection string '{connectionString}'...");
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(builder.Configuration.GetConnectionString(connectionString)));
builder.Services.AddHealthChecks().AddSqlServer(connectionString);
logger.LogInformation($"Connection string '{connectionString}' found.");

logger.LogInformation("Starting Dependency Injections...");
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<Application.Dtos.TransactionDtoValidator>();

// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

logger.LogInformation("builder.Services => DONE...");


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    logger.LogInformation("Environment is Development.");
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