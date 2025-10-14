using System.Text.RegularExpressions;
using openPlot.Contracts.Requests;

namespace openPlot.Application.Validation;

public static class SubmitSearchValidator
{
    private static readonly Regex AggRegex = new(
        @"^(raw|(\d+ms)|(\d+s))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static (bool ok, string? error, (string mode, string? agg, int? rate) normalized)
        Validate(SubmitSearchRequest r, int maxTerminais)
    {
        if (r is null) return (false, "payload ausente", default);
        if (string.IsNullOrWhiteSpace(r.Username)) return (false, "username obrigatório", default);
        if (string.IsNullOrWhiteSpace(r.ConfigVersion)) return (false, "config_version obrigatória", default);
        if (r.Terminais is null || r.Terminais.Count == 0) return (false, "informe ao menos 1 terminal", default);
        if (r.Terminais.Count > maxTerminais) return (false, $"máximo de {maxTerminais} terminais", default);
        if (r.Resolucao is null) return (false, "resolucao obrigatória", default);

        var hasAgg = !string.IsNullOrWhiteSpace(r.Resolucao.Agg);
        var hasRate = r.Resolucao.SelectRate.HasValue;

        if (hasAgg && hasRate) return (false, "use Agg OU SelectRate (não ambos)", default);
        if (!hasAgg && !hasRate) return (false, "informe Agg (raw/100ms/1s) OU SelectRate (ex.: 60)", default);

        if (hasAgg)
        {
            var agg = r.Resolucao.Agg!.Trim().ToLowerInvariant();
            if (!AggRegex.IsMatch(agg)) return (false, "Agg inválido (use 'raw', 'Xms' ou 'Xs')", default);
            return (true, null, ("agg", agg, null));
        }

        var rate = r.Resolucao.SelectRate!.Value;
        if (rate <= 0 || rate > 1000) return (false, "SelectRate deve estar entre 1 e 1000", default);
        return (true, null, ("rate", null, rate));
    }
}
