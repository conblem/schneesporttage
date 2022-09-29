using System.Security.Claims;
using api.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Npgsql;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace api;

public static class DiExtensions
{
    public static IServiceCollection AddRepos(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserRepo, UserRepo>();

        return serviceCollection;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        
        var oltp = configuration.GetSection("Oltp");
        var serviceName = oltp["Service"];
        var oltpEndpoint = oltp["Endpoint"];
        
        serviceCollection.AddSingleton<ITelemetryService>(_=> new TelemetryService(serviceName));
        
        serviceCollection.AddOpenTelemetryTracing(tracingBuilder =>
            tracingBuilder
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(oltpEndpoint);
                })
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddSource(serviceName)
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.CfEnrich();
                })
                .AddNpgsql()
        );

        return serviceCollection;
    }

    private static AspNetCoreInstrumentationOptions CfEnrich(this AspNetCoreInstrumentationOptions options)
    {
        options.Enrich = (activity, eventName, obj) =>
        {
            if (eventName.Equals("OnStartActivity") && obj is HttpRequest httpRequest)
            {
                if (httpRequest.Headers.TryGetValue("Cf-Ray", out var rayId))
                {
                    activity.AddTag(nameof(rayId), rayId);
                }
            }
            if (eventName.Equals("OnStopActivity") && obj is HttpResponse httpResponse)
            {
                var remoteIp = httpResponse.HttpContext.Connection.RemoteIpAddress;
                activity.SetTag($"http.{nameof(remoteIp)}", remoteIp);
                            
                var user = httpResponse.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                if (user is not null)
                {
                    activity.AddTag("user.email", user);
                }
            }
        };

        return options;
    }
    
    public static void SetJwksOptions(this JwtBearerOptions options, string uri)
    {
        var httpClient = new HttpClient(new HttpClientHandler())
        {
            MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB 
        };

        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            uri,
            new JwksRetrieverNew(),
            new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });
    }
}