using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace openPlot.Tests.Integration;

public class AuthWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 🔧 Define explicitamente o ContentRoot como a pasta do projeto Web
        builder.UseContentRoot(TestPathHelper.GetProjectPath("openPlot"));

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string?>("Auth:UserStorePath", "users.json"),
                new KeyValuePair<string,string?>("Session:CookieName", ".openplot.sid.tests"),
                new KeyValuePair<string,string?>("Session:CookieSameSite", "None"),
                new KeyValuePair<string,string?>("Session:CookieSecurePolicy", "None")
            });
        });
    }
}
