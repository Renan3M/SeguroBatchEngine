using SeguroBatchEngine.Domain.Entities;

namespace SeguroBatchEngine.Domain.Interfaces
{
	public interface IApoliceRepository
	{
		Task InserirLoteAsync(IEnumerable<Apolice> apolices);	
	}
}
