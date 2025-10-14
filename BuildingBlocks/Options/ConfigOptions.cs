namespace openPlot.BuildingBlocks.Options;

public sealed class ConfigOptions
{
    /// <summary>
    /// Diretório onde ficam os XMLs. Pode ser relativo ao ContentRoot (ex: "configs/xml")
    /// ou absoluto (ex: "D:\\data\\configs\\xml").
    /// </summary>
    public string XmlDirectory { get; init; } = "configs/xml";
}
