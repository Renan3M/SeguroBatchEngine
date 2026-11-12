namespace SeguroBatchEngine.Domain.Entities;

public class Apolice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NumeroApolice { get; set; } = string.Empty;
    public string CpfCnpjSegurado { get; set; } = string.Empty;
    public decimal ValorPremio { get; set; }
    public DateTime DataInicioVigencia { get; set; }
    public DateTime DataFimVigencia { get; set; }
    public string MetodoIngestao { get; set; } = "Regex"; // 'Regex' ou 'AI_SemanticKernel'
    public DateTime DataProcessamento { get; set; } = DateTime.UtcNow;
}
