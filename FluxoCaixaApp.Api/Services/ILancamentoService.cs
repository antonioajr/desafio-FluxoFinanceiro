using FluxoCaixaApp.Api.Data.Entities;
using FluxoCaixaApp.Api.Models;

namespace FluxoCaixaApp.Api.Services;

public interface ILancamentoService
{
    Task<Lancamento> RegistrarAsync(LancamentoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lancamento>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<RelatorioDiario> ObterSaldoDiarioAsync(DateTime data, CancellationToken cancellationToken = default);
}
