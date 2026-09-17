using FluxoCaixaApp.Api.Data.Entities;
using FluxoCaixaApp.Api.Models;
using FluxoCaixaApp.Api.Services;

namespace FluxoCaixaApp.Tests;

public class LancamentoServiceTests
{
    [Fact]
    public async Task RegistrarAsync_DeveAceitarLancamentoCreditoValido()
    {
        var service = new LancamentoService(new FakeLancamentoRepository());

        var request = new LancamentoRequest
        {
            Data = new DateTime(2026, 9, 17),
            Tipo = TipoLancamento.Credito,
            Descricao = "Venda",
            Categoria = "Receita",
            Valor = 150.75m
        };

        var result = await service.RegistrarAsync(request);

        Assert.Equal(request.Descricao, result.Descricao);
        Assert.Equal(TipoLancamento.Credito, result.Tipo);
        Assert.Equal(150.75m, result.Valor);
    }

    [Fact]
    public async Task RegistrarAsync_DeveRejeitarValorInvalido()
    {
        var service = new LancamentoService(new FakeLancamentoRepository());

        var request = new LancamentoRequest
        {
            Data = new DateTime(2026, 9, 17),
            Tipo = TipoLancamento.Debito,
            Descricao = "Gasto",
            Categoria = "Despesas",
            Valor = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.RegistrarAsync(request));
    }

    [Fact]
    public async Task ObterSaldoDiarioAsync_DeveCalcularValorConsolidadoCorretamente()
    {
        var repository = new FakeLancamentoRepository(
            new Lancamento { Data = new DateTime(2026, 9, 17), Tipo = TipoLancamento.Credito, Descricao = "Venda", Categoria = "Receita", Valor = 100m },
            new Lancamento { Data = new DateTime(2026, 9, 17), Tipo = TipoLancamento.Credito, Descricao = "Venda 2", Categoria = "Receita", Valor = 50m },
            new Lancamento { Data = new DateTime(2026, 9, 17), Tipo = TipoLancamento.Debito, Descricao = "Compra", Categoria = "Despesa", Valor = 20m });

        var service = new LancamentoService(repository);

        var result = await service.ObterSaldoDiarioAsync(new DateTime(2026, 9, 17));

        Assert.Equal(150m, result.TotalCreditos);
        Assert.Equal(20m, result.TotalDebitos);
        Assert.Equal(130m, result.SaldoConsolidado);
        Assert.Equal(3, result.QuantidadeLancamentos);
    }

    private sealed class FakeLancamentoRepository : FluxoCaixaApp.Api.Data.Repositories.ILancamentoRepository
    {
        private readonly List<Lancamento> _lancamentos;

        public FakeLancamentoRepository(params Lancamento[] lancamentos)
        {
            _lancamentos = lancamentos.ToList();
        }

        public Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
        {
            _lancamentos.Add(lancamento);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Lancamento>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Lancamento>>(_lancamentos.AsReadOnly());
        }

        public Task<IReadOnlyCollection<Lancamento>> GetByDateAsync(DateTime data, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Lancamento>>(
                _lancamentos
                    .Where(x => x.Data.Date == data.Date)
                    .ToList()
                    .AsReadOnly());
        }
    }
}
