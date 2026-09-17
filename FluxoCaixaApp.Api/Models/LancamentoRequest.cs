namespace FluxoCaixaApp.Api.Models;

using FluxoCaixaApp.Api.Data.Entities;

public class LancamentoRequest
{
    public DateTime Data { get; set; } = DateTime.Today;
    public TipoLancamento Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
