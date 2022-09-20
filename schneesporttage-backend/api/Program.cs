using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var oltp = builder.Configuration.GetSection("Oltp");
var serviceName = oltp["Service"];
var oltpEndpoint = oltp["Endpoint"];

// Configure important OpenTelemetry settings, the console exporter, and instrumentation library
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri(oltpEndpoint);
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        })
        .AddSource(oltp["Service"])
        .AddProcessor()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Docker is also for local development
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();