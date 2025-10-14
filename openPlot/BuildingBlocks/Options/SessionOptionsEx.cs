namespace openPlot.BuildingBlocks.Options;

public sealed class SessionOptionsEx
{
    public int IdleTimeoutMinutes { get; init; } = 60;
    public string CookieName { get; init; } = ".openplot.sid";
    public string CookieSecurePolicy { get; init; } = "None";
    public string CookieSameSite { get; init; } = "Lax";
}
