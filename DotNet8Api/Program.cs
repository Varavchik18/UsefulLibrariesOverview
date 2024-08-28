using Microsoft.Extensions.Options;
using Refit;
using Serilog;
using Serilog.Events;
using Polly;
using System.Net;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SerilogSettings>(builder.Configuration.GetSection("Serilog"));
builder.Services.Configure<GitHubSettings>(builder.Configuration.GetSection("GitHubSettings"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRefitClient<IGitHubApi>()
    .ConfigureHttpClient((sp, client) =>
    {
        var settings = sp.GetRequiredService<IOptions<GitHubSettings>>().Value;
        client.BaseAddress = new Uri(settings.BaseAddress);
        client.DefaultRequestHeaders.Add("Authorization", settings.AccessToken);
        client.DefaultRequestHeaders.Add("User-Agent", settings.UserAgent);
    })
    .AddPolicyHandler(PollyPoliciesExtensions.GetCombinedPolicy());

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<GitHubUserValidator>());


var serilogSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<SerilogSettings>>().Value;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(new Serilog.Core.LoggingLevelSwitch(
        Enum.Parse<LogEventLevel>(serilogSettings.MinimumLevel.Default, true)))
    .WriteTo.File(
        serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "File")?.Args.path,
        rollingInterval: Enum.Parse<RollingInterval>(serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "File")?.Args.rollingInterval, true),
        outputTemplate: serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "File")?.Args.outputTemplate)
    .WriteTo.Console(outputTemplate: serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "Console")?.Args.outputTemplate)
    .WriteTo.MSSqlServer(
        serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "MSSqlServer")?.Args.connectionString,
        serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "MSSqlServer")?.Args.tableName,
        schemaName: serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "MSSqlServer")?.Args.schemaName,
        autoCreateSqlTable: serilogSettings.WriteTo.FirstOrDefault(w => w.Name == "MSSqlServer")?.Args.autoCreateSqlTable ?? true) 
    .CreateLogger();


builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();