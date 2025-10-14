namespace openPlot.Infrastructure.Auth.Models;

public sealed class UserRecord
{
    public required string sub { get; init; }
    public required string username { get; init; }
    public required string password { get; init; }      // para demo; em prod → hash
    public string? preferred_username { get; init; }
    public string? email { get; init; }
    public List<string> roles { get; init; } = new();
    public Dictionary<string, string> claims { get; init; } = new();
}
