using System.Diagnostics;
using api;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var oltp = builder.Configuration.GetSection("Oltp");
var serviceName = oltp["Service"];
var oltpEndpoint = oltp["Endpoint"];

// Configure important OpenTelemetry settings, the console exporter, and instrumentation library
builder.Services.AddOpenTelemetryTracing(tracingBuilder =>
    tracingBuilder
        .AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(oltpEndpoint);
        })
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddSource(serviceName)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddNpgsql()
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SchneesporttageContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DB"))
);

builder.Services.AddRepos();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Docker is also for local development
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.MapControllers();
});

app.UseHttpMetrics();

app.Run();