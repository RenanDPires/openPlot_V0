using openPlot.Contracts.Requests;

namespace openPlot.Application.Commands;

public sealed class SubmitSearchCommand
{
    public required string RequestId { get; init; }
    public required SubmitSearchRequest Payload { get; init; }

    public required string Mode { get; init; }  // "agg" | "rate"
    // public string? Agg { get; init; }
    public int? SelectRate { get; init; }
}
