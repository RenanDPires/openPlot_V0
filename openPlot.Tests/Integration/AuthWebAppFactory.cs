using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

public class AuthWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 🔹 Define o diretório raiz da aplicação (a pasta do projeto openPlot)
        builder.UseSolutionRelativeContentRoot("openPlot");

        // 🔹 Injeta as configurações para rodar os testes sem HTTPS
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("Auth:UserStorePath", "users.json"),        // usa o mock oficial
                new KeyValuePair<string, string?>("Session:CookieName", ".openplot.sid.tests"),
                new KeyValuePair<string, string?>("Session:CookieSameSite", "None"),
                new KeyValuePair<string, string?>("Session:CookieSecurePolicy", "None")      // HTTPS desabilitado no TestServer
            });
        });
    }
}
