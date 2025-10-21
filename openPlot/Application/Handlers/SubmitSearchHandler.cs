using openPlot.Application.Abstractions;
using openPlot.Application.Commands;
using openPlot.Contracts.Responses;

namespace openPlot.Application.Handlers;

public sealed class SubmitSearchHandler : ICommandHandler<SubmitSearchCommand, SubmitSearchResponse>
{
    public SubmitSearchHandler() { }

    public Task<SubmitSearchResponse> Handle(SubmitSearchCommand cmd, CancellationToken ct)
    {
        // Aqui você poderá: validar existência de config_version, registrar auditoria, etc.
        var r = cmd.Payload;
        var resp = new SubmitSearchResponse
        {
            RequestId = cmd.RequestId,
            Username = r.Username,
            ConfigVersion = r.ConfigVersion,
            Terminais = r.Terminais.ToArray(),
            Mode = cmd.Mode,
            //Agg = cmd.Agg,
            SelectRate = cmd.SelectRate
        };
        return Task.FromResult(resp);
    }
}
