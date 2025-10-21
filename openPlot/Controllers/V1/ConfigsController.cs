using Microsoft.AspNetCore.Mvc;
using openPlot.Application.Config;
using openPlot.Middleware;
using openPlot.Contracts.Responses;

namespace openPlot.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/configs")]
[Produces("application/json")]
public sealed class ConfigsController : ControllerBase
{
    private readonly IConfigManifestService _service;

    public ConfigsController(IConfigManifestService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna SOMENTE um array JSON com os nomes dos arquivos XML (formato esperado pelo consumidor).
    /// Ex.: ["OH2_ONS_BSB.xml","OH2_ONS_BSB_GOLD.xml"]
    /// </summary>
    [HttpGet("manifest")]
    [RequireUser] // se quiser liberar público por enquanto, remova este atributo
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManifest(CancellationToken ct)
    {
        var files = await _service.ListXmlAsync(ct);
        return Ok(files); // array simples de strings
    }

    /// <summary>
    /// Retorna um objeto com metadados (útil pra UI/diagnóstico interno).
    /// </summary>
    [HttpGet("manifest/meta")]
    [RequireUser]
    [ProducesResponseType(typeof(ConfigManifestMetaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetManifestMeta(CancellationToken ct)
    {
        var files = await _service.ListXmlAsync(ct);
        var resp = new ConfigManifestMetaResponse { Count = files.Count, Files = files };
        return Ok(resp);
    }
}

public sealed class ConfigManifestMetaResponse
{
    public int Count { get; init; }
    public IReadOnlyList<string> Files { get; init; } = Array.Empty<string>();
}
