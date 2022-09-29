using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.jwks;

public class CfAuthEventsHandler: JwtBearerEvents
{
    internal CfAuthEventsHandler()
    {
        OnMessageReceived = MessageReceivedHandler;
    }

    private Task MessageReceivedHandler(MessageReceivedContext context)
    {
        if (!context.Request.Headers.TryGetValue("Cf-Access-Jwt-Assertion", out var headerValue))
        {
            context.NoResult();
            return Task.CompletedTask;
        }

        context.Token = headerValue;
        
        return Task.CompletedTask;
    }

}