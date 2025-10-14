using System.Text.Json;
using openPlot.Infrastructure.Auth.Models;

namespace openPlot.Infrastructure.Auth;

public sealed class JsonUserStore : IUserStore
{
    private readonly string _path;

    public JsonUserStore(IConfiguration cfg, IWebHostEnvironment env)
    {
        var configured = cfg["Auth:UserStorePath"] ?? "users.json";
        _path = Path.IsPathRooted(configured) ? configured : Path.Combine(env.ContentRootPath, configured);
    }

    public async Task<UserRecord?> FindByUsernameAsync(string username, CancellationToken ct)
    {
        if (!File.Exists(_path)) return null;
        await using var fs = File.OpenRead(_path);
        var users = await JsonSerializer.DeserializeAsync<List<UserRecord>>(fs, cancellationToken: ct)
                    ?? new List<UserRecord>();
        return users.FirstOrDefault(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
    }
}
