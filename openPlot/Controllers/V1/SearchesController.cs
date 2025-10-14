using Microsoft.AspNetCore.Mvc;
using openPlot.Application.Abstractions;
using openPlot.Application.Commands;
using openPlot.Application.Validation;
using openPlot.Contracts.Requests;
using openPlot.Contracts.Responses;

namespace openPlot.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/searches")]
public sealed class SearchesController : ControllerBase
{
    private readonly ICommandHandler<SubmitSearchCommand, SubmitSearchResponse> _handler;
    private readonly IConfiguration _cfg;

    public SearchesController(
        ICommandHandler<SubmitSearchCommand, SubmitSearchResponse> handler,
        IConfiguration cfg)
    {
        _handler = handler;
        _cfg = cfg;
    }

    [HttpPost("submit")]
    [ProducesResponseType(typeof(SubmitSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit([FromBody] SubmitSearchRequest request, CancellationToken ct)
    {
        var maxTerminais = _cfg.GetValue<int>("Policies:MaxTerminais", 64);
        var (ok, err, norm) = SubmitSearchValidator.Validate(request, maxTerminais);
        if (!ok) return Problem(statusCode: 400, title: "Parâmetros inválidos", detail: err);

        var cmd = new SubmitSearchCommand
        {
            RequestId = Guid.NewGuid().ToString("N"),
            Payload = request,
            Mode = norm.mode,
            Agg = norm.agg,
            SelectRate = norm.rate
        };

        var resp = await _handler.Handle(cmd, ct);
        return Ok(resp);
    }
}
