using Application.Repositories;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

// Swagger
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ByCoders Challenge API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application
var connectionString = "DefaultConnection";
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new SqlServerConnectionFactory(builder.Configuration.GetConnectionString(connectionString)));
builder.Services.AddHealthChecks().AddSqlServer(connectionString);

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAntiforgery();
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();