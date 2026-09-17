using FluxoCaixaApp.Api.Data.Entities;
using FluxoCaixaApp.Api.Data.Repositories;
using FluxoCaixaApp.Api.Models;

namespace FluxoCaixaApp.Api.Services;

public class LancamentoService : ILancamentoService
{
    private readonly ILancamentoRepository _repository;

    public LancamentoService(ILancamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Lancamento> RegistrarAsync(LancamentoRequest request, CancellationToken cancellationToken = default)
    {
        ValidarRequest(request);

        var lancamento = new Lancamento
        {
            Id = Guid.NewGuid(),
            Data = request.Data.Date,
            Tipo = request.Tipo,
            Descricao = request.Descricao.Trim(),
            Categoria = request.Categoria.Trim(),
            Valor = request.Valor
        };

        await _repository.AddAsync(lancamento, cancellationToken);
        return lancamento;
    }

    public async Task<IReadOnlyCollection<Lancamento>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<RelatorioDiario> ObterSaldoDiarioAsync(DateTime data, CancellationToken cancellationToken = default)
    {
        var lancamentos = await _repository.GetByDateAsync(data, cancellationToken);

        var totalCreditos = lancamentos
            .Where(x => x.Tipo == TipoLancamento.Credito)
            .Sum(x => x.Valor);

        var totalDebitos = lancamentos
            .Where(x => x.Tipo == TipoLancamento.Debito)
            .Sum(x => x.Valor);

        return new RelatorioDiario
        {
            Data = data.Date,
            TotalCreditos = totalCreditos,
            TotalDebitos = totalDebitos,
            SaldoConsolidado = totalCreditos - totalDebitos,
            QuantidadeLancamentos = lancamentos.Count
        };
    }

    private static void ValidarRequest(LancamentoRequest request)
    {
        if (request.Valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(request.Descricao))
            throw new ArgumentException("A descrição é obrigatória.");

        if (string.IsNullOrWhiteSpace(request.Categoria))
            throw new ArgumentException("A categoria é obrigatória.");
    }
}
