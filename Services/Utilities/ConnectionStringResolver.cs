using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ReservaCanchita.Services.Utilities;

public interface IConnectionStringResolver
{
    string GetConnectionString();
}

public class ConnectionStringResolver : IConnectionStringResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ConnectionStringResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetConnectionString()
    {
        var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
        var subdomain = host?.Split('.')[0].ToLower();

        if (string.IsNullOrEmpty(subdomain))
            subdomain = "DefaultConnection";

        var connString = _configuration.GetConnectionString(subdomain);

        if (string.IsNullOrEmpty(connString))
            connString = _configuration.GetConnectionString("DefaultConnection");

        return connString;
    }
}
