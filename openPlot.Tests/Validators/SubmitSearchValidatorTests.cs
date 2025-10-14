using FluentAssertions;
using openPlot.Application.Validation;
using openPlot.Contracts.Requests;
using Xunit;

namespace openPlot.Tests.Validators;

public class SubmitSearchValidatorTests
{
    // Fábrica: sempre cria um NOVO request já com tudo definido
    private static SubmitSearchRequest MakeReq(
        string? agg = "100ms",
        int? rate = null,
        string username = "renan",
        string configVersion = "SIN_prod_2025_09",
        IReadOnlyList<string>? terminais = null)
    {
        return new SubmitSearchRequest
        {
            Username = username,
            ConfigVersion = configVersion,
            Terminais = terminais ?? new[] { "RJTRIO_PL1" },
            Resolucao = new ResolutionDto { Agg = agg, SelectRate = rate }
        };
    }

    [Fact]
    public void Should_accept_agg_100ms()
    {
        var req = MakeReq(agg: "100ms", rate: null);
        var (ok, err, norm) = SubmitSearchValidator.Validate(req, maxTerminais: 64);

        ok.Should().BeTrue();
        err.Should().BeNull();
        norm.mode.Should().Be("agg");
        norm.agg.Should().Be("100ms");
        norm.rate.Should().BeNull();
    }

    [Fact]
    public void Should_accept_rate_60()
    {
        var req = MakeReq(agg: null, rate: 60);
        var (ok, err, norm) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeTrue();
        norm.mode.Should().Be("rate");
        norm.rate.Should().Be(60);
        norm.agg.Should().BeNull();
    }

    [Fact]
    public void Should_reject_both_agg_and_rate()
    {
        var req = MakeReq(agg: "1s", rate: 60);
        var (ok, err, _) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeFalse();
        // opção A: contem qualquer um dos termos
        err!.ToLower().Should().ContainAny(new[] { "apenas", "use" });

        // opção B: regex simples
        err!.ToLower().Should().MatchRegex("(apenas|use)");

    }

    [Fact]
    public void Should_reject_none_agg_nor_rate()
    {
        var req = MakeReq(agg: null, rate: null);
        var (ok, err, _) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeFalse();
        err!.ToLower().Should().Contain("informe");
    }

    [Fact]
    public void Should_reject_invalid_agg()
    {
        var req = MakeReq(agg: "banana", rate: null);
        var (ok, err, _) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeFalse();
        err!.ToLower().Should().Contain("agg inválido");
    }

    [Fact]
    public void Should_reject_too_many_terminais()
    {
        var req = MakeReq(agg: "100ms", terminais: Enumerable.Repeat("X", 65).ToArray());
        var (ok, err, _) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeFalse();
        err!.ToLower().Should().Contain("máximo");
    }

    [Fact]
    public void Should_reject_missing_username()
    {
        var req = MakeReq(agg: "100ms", username: "");
        var (ok, err, _) = SubmitSearchValidator.Validate(req, 64);

        ok.Should().BeFalse();
        err!.ToLower().Should().Contain("username");
    }
}
