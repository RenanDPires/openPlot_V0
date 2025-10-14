using Microsoft.Extensions.Options;
using openPlot.BuildingBlocks.Options;

namespace openPlot.Application.Config;

public sealed class ConfigManifestService : IConfigManifestService
{
    private readonly IWebHostEnvironment _env;
    private readonly ConfigOptions _opts;

    public ConfigManifestService(IWebHostEnvironment env, IOptions<ConfigOptions> opts)
    {
        _env = env;
        _opts = opts.Value;
    }

    public Task<IReadOnlyList<string>> ListXmlAsync(CancellationToken ct)
    {
        var baseDir = _opts.XmlDirectory;
        var dir = Path.IsPathRooted(baseDir) ? baseDir : Path.Combine(_env.ContentRootPath, baseDir);

        if (!Directory.Exists(dir))
            return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

        var files = Directory.EnumerateFiles(dir, "*.xml", SearchOption.TopDirectoryOnly)
                             .Select(Path.GetFileName)
                             .Where(n => !string.IsNullOrWhiteSpace(n))
                             .Cast<string>()
                             .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                             .ToArray();

        return Task.FromResult<IReadOnlyList<string>>(files);
    }
}
