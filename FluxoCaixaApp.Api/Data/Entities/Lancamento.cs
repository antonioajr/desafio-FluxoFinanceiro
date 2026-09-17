using System.ComponentModel.DataAnnotations;

namespace FluxoCaixaApp.Api.Data.Entities;

public enum TipoLancamento
{
    Credito = 1,
    Debito = 2
}

public class Lancamento
{
    public Guid Id { get; set; }

    [Required]
    public DateTime Data { get; set; }

    [Required]
    public TipoLancamento Tipo { get; set; }

    [Required]
    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Categoria { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
