using SeguroBatchEngine.Application.Services;
using Xunit;

namespace SeguroBatchEngine.UnitTests;

public class ApoliceParserTests
{
    [Fact]
    public void TryParse_LinhaValida_DeveRetornarSucessoEMetodoRegex()
    {
        var linha = "APOLICE: 998877 | DOC: 11122233344 | VALOR: 2450.00 | VIGENCIA: 2026-01-01 A 2027-01-01";

        var sucesso = ApoliceParser.TryParse(linha, out var apolice);

        Assert.True(sucesso);
        Assert.NotNull(apolice);
        Assert.Equal("998877", apolice.NumeroApolice);
        Assert.Equal("Regex", apolice.MetodoIngestao);
    }

    [Fact]
    public void TryParse_LinhaCorrompida_DeveFalharParaAcionarIA()
    {
        var linhaCorrompida = "Seguro auto contrato n 998877 valor de 2450 reais cliente doc 11122233344";

        var sucesso = ApoliceParser.TryParse(linhaCorrompida, out var apolice);

        Assert.False(sucesso);
        Assert.Null(apolice);
    }
}