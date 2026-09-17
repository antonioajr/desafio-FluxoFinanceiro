using System.Text;
using System.Text.Json;
using System.Reflection;
using FluxoCaixaApp.Api.Data;
using FluxoCaixaApp.Api.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace FluxoCaixaApp.Api.Background;

public class OutboxPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly IConfiguration _configuration;

    public OutboxPublisherService(IServiceProvider serviceProvider, ILogger<OutboxPublisherService> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pendentes = await db.OutboxMessages
                    .Where(x => !x.Processado)
                    .Take(50)
                    .ToListAsync(stoppingToken);

                foreach (var mensagem in pendentes)
                {
                    PublishToRabbitMq(mensagem);
                    mensagem.Processado = true;
                }

                if (pendentes.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagens do Outbox.");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private static void PublishToRabbitMq(OutboxMessage mensagem)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        dynamic connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        dynamic channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

        channel.QueueDeclareAsync(
            queue: "fluxocaixa.lancamentos",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();

        var body = Encoding.UTF8.GetBytes(mensagem.Payload);
        channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "fluxocaixa.lancamentos",
            mandatory: false,
            basicProperties: null,
            body: body).GetAwaiter().GetResult();
    }

}
