namespace FluxoCaixaApp.Api.Data.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public bool Processado { get; set; }
}
