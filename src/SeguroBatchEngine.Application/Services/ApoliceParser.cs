using System.Text.RegularExpressions;
using SeguroBatchEngine.Domain.Entities;

namespace SeguroBatchEngine.Application.Services;

public static partial class ApoliceParser
{
    // Formato esperado:
    // APOLICE: 123456789 | DOC: 12345678900 | VALOR: 1500.50 | VIGENCIA: 2026-01-01 A 2027-01-01
    [GeneratedRegex(@"APOLICE:\s*(?<num>\d+)\s*\|\s*DOC:\s*(?<doc>\d+)\s*\|\s*VALOR:\s*(?<valor>[\d\.]+)\s*\|\s*VIGENCIA:\s*(?<inicio>\d{4}-\d{2}-\d{2})\s*A\s*(?<fim>\d{4}-\d{2}-\d{2})", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ApoliceRegex();

    public static bool TryParse(string rawLine, out Apolice? apolice)
    {
        apolice = null;
        var match = ApoliceRegex().Match(rawLine);

        if (!match.Success)
            return false;

        try
        {
            apolice = new Apolice
            {
                NumeroApolice = match.Groups["num"].Value,
                CpfCnpjSegurado = match.Groups["doc"].Value,
                ValorPremio = decimal.Parse(match.Groups["valor"].Value, System.Globalization.CultureInfo.InvariantCulture),
                DataInicioVigencia = DateTime.Parse(match.Groups["inicio"].Value),
                DataFimVigencia = DateTime.Parse(match.Groups["fim"].Value),
                MetodoIngestao = "Regex"
            };
            return true;
        }
        catch
        {
            return false;
        }
    }
}