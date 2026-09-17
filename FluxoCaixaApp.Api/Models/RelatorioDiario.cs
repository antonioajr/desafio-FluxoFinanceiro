namespace FluxoCaixaApp.Api.Models;

public class RelatorioDiario
{
    public DateTime Data { get; set; }
    public decimal TotalCreditos { get; set; }
    public decimal TotalDebitos { get; set; }
    public decimal SaldoConsolidado { get; set; }
    public int QuantidadeLancamentos { get; set; }
}
