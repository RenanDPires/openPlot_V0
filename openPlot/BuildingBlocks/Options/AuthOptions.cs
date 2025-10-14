namespace openPlot.BuildingBlocks.Options;

public sealed class AuthOptions
{
    public string UserStorePath { get; init; } = "users.json";
    public string SessionKey { get; init; } = "openplot:user";
}
