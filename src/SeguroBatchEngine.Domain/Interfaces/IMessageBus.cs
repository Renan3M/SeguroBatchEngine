namespace SeguroBatchEngine.Domain.Interfaces;

public record ApoliceFallbackEvent(
    Guid BatchId,
    int NumeroLinha,
    string ConteudoBruto,
    DateTime DataOcorrenciaUtc
);

public interface IMessageBus
{
    Task PublicarFallbackAsync(ApoliceFallbackEvent evento, CancellationToken cancellationToken = default);
}