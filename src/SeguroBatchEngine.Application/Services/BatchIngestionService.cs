using System.Threading.Channels;
using SeguroBatchEngine.Domain.Entities;
using SeguroBatchEngine.Domain.Interfaces;

namespace SeguroBatchEngine.Application.Services;

public class BatchIngestionService
{
    private readonly IApoliceRepository _repository;
    private readonly IMessageBus _messageBus;
    private const int BatchSize = 1000;

    public BatchIngestionService(IApoliceRepository repository, IMessageBus messageBus)
    {
        _repository = repository;
        _messageBus = messageBus;
    }

    public async Task ProcessarStreamAsync(Stream arquivoStream, Guid batchId, CancellationToken ct = default)
    {
        var channel = Channel.CreateBounded<Apolice>(new BoundedChannelOptions(5000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = false
        });

        var consumerTask = Task.Run(async () =>
        {
            var loteAtual = new List<Apolice>(BatchSize);

            await foreach (var apolice in channel.Reader.ReadAllAsync(ct))
            {
                loteAtual.Add(apolice);

                if (loteAtual.Count >= BatchSize)
                {
                    await _repository.InserirLoteAsync(loteAtual);
                    loteAtual.Clear();
                }
            }

            if (loteAtual.Count > 0)
            {
                await _repository.InserirLoteAsync(loteAtual);
            }
        }, ct);

        // Producer: Lê linha a linha de forma 100% assíncrona até o fim do stream (null)
        using var reader = new StreamReader(arquivoStream);
        int numeroLinha = 0;

        while (await reader.ReadLineAsync(ct) is { } linha)
        {
            numeroLinha++;

            if (string.IsNullOrWhiteSpace(linha)) continue;

            if (ApoliceParser.TryParse(linha, out var apolice) && apolice is not null)
            {
                await channel.Writer.WriteAsync(apolice, ct);
            }
            else
            {
                var evento = new ApoliceFallbackEvent(batchId, numeroLinha, linha, DateTime.UtcNow);
                await _messageBus.PublicarFallbackAsync(evento, ct);
            }
        }

        channel.Writer.Complete();
        await consumerTask;
    }
}