using FluxoCaixaApp.Api.Data.Entities;

namespace FluxoCaixaApp.Api.Data.Repositories;

public interface ILancamentoRepository
{
    Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lancamento>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lancamento>> GetByDateAsync(DateTime data, CancellationToken cancellationToken = default);
}
