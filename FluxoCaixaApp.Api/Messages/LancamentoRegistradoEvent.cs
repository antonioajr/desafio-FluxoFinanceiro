namespace FluxoCaixaApp.Api.Messages;

public class LancamentoRegistradoEvent
{
    public Guid LancamentoId { get; set; }
    public DateTime Data { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
