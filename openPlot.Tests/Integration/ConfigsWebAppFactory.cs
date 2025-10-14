using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace openPlot.Tests.Integration;

public class ConfigsWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _tempXmlDir;

    public ConfigsWebAppFactory()
    {
        _tempXmlDir = Path.Combine(Path.GetTempPath(), "openplot-tests-xml-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempXmlDir);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 🔧 ContentRoot fixo no projeto Web
        builder.UseContentRoot(TestPathHelper.GetProjectPath("openPlot"));

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string?>("Auth:UserStorePath", "users.json"),
                new KeyValuePair<string,string?>("Session:CookieName", ".openplot.sid.tests"),
                new KeyValuePair<string,string?>("Session:CookieSameSite", "None"),
                new KeyValuePair<string,string?>("Session:CookieSecurePolicy", "None"),
                new KeyValuePair<string,string?>("Configs:XmlDirectory", _tempXmlDir)
            });
        });
    }

    public void ClearTempXmlDir()
    {
        if (!Directory.Exists(_tempXmlDir)) return;
        foreach (var f in Directory.EnumerateFiles(_tempXmlDir)) File.Delete(f);
    }

    public void SeedXmlFiles(params string[] names)
    {
        foreach (var n in names)
        {
            var path = Path.Combine(_tempXmlDir, n);
            File.WriteAllText(path, "<root/>");
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try { if (Directory.Exists(_tempXmlDir)) Directory.Delete(_tempXmlDir, true); } catch { /* ignore */ }
    }
}
