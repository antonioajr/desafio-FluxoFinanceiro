using FluxoCaixaApp.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FluxoCaixaApp.Api.Data.Repositories;

public class LancamentoRepository : ILancamentoRepository
{
    private readonly AppDbContext _context;

    public LancamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Lancamento lancamento, CancellationToken cancellationToken = default)
    {
        await _context.Lancamentos.AddAsync(lancamento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Lancamento>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Lancamentos
            .OrderBy(x => x.Data)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Lancamento>> GetByDateAsync(DateTime data, CancellationToken cancellationToken = default)
    {
        return await _context.Lancamentos
            .Where(x => x.Data.Date == data.Date)
            .AsNoTracking()
            .OrderBy(x => x.Data)
            .ToListAsync(cancellationToken);
    }
}
