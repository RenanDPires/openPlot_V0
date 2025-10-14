namespace openPlot.Application.Config;

public interface IConfigManifestService
{
    /// <summary>Lista somente os nomes de arquivos .xml no diretório configurado.</summary>
    Task<IReadOnlyList<string>> ListXmlAsync(CancellationToken ct);
}
