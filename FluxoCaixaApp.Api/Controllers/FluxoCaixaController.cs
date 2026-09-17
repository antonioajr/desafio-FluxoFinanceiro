using FluxoCaixaApp.Api.Data;
using FluxoCaixaApp.Api.Data.Entities;
using FluxoCaixaApp.Api.Messages;
using FluxoCaixaApp.Api.Models;
using FluxoCaixaApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FluxoCaixaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FluxoCaixaController : ControllerBase
{
    private readonly ILancamentoService _service;
    private readonly AppDbContext _dbContext;

    public FluxoCaixaController(ILancamentoService service, AppDbContext dbContext)
    {
        _service = service;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Lancamento>>> ObterTodosAsync(CancellationToken cancellationToken)
    {
        var result = await _service.ObterTodosAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Lancamento>> RegistrarAsync([FromBody] LancamentoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var lancamento = await _service.RegistrarAsync(request, cancellationToken);

            var evento = new LancamentoRegistradoEvent
            {
                LancamentoId = lancamento.Id,
                Data = lancamento.Data,
                Tipo = lancamento.Tipo.ToString(),
                Valor = lancamento.Valor
            };

            var outbox = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Tipo = nameof(LancamentoRegistradoEvent),
                Payload = JsonSerializer.Serialize(evento),
                Processado = false
            };

            _dbContext.OutboxMessages.Add(outbox);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(ObterTodosAsync), new { id = lancamento.Id }, lancamento);
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpGet("saldo-diario/{data:datetime}")]
    public async Task<ActionResult<RelatorioDiario>> ObterSaldoDiarioAsync(DateTime data, CancellationToken cancellationToken)
    {
        var relatorio = await _service.ObterSaldoDiarioAsync(data, cancellationToken);
        return Ok(relatorio);
    }
}
