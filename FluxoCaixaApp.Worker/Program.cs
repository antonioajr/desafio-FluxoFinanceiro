using System.Text;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<LancamentoConsumerService>();

var host = builder.Build();
await host.RunAsync();

public sealed class LancamentoConsumerService : BackgroundService
{
	private readonly ILogger<LancamentoConsumerService> _logger;
	private readonly IConfiguration _configuration;

	public LancamentoConsumerService(ILogger<LancamentoConsumerService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
		var userName = _configuration["RabbitMQ:UserName"] ?? "guest";
		var password = _configuration["RabbitMQ:Password"] ?? "guest";
		var queueName = _configuration["RabbitMQ:QueueName"] ?? "fluxocaixa.lancamentos";

		var factory = new ConnectionFactory
		{
			HostName = hostName,
			UserName = userName,
			Password = password
		};

		dynamic connection = await factory.CreateConnectionAsync(stoppingToken);
		dynamic channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
		await channel.QueueDeclareAsync(
			queue: queueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null,
			cancellationToken: stoppingToken);

		var consumer = new AsyncEventingBasicConsumer(channel);
		consumer.ReceivedAsync += async (_, eventArgs) =>
		{
			var payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
			_logger.LogInformation("Mensagem recebida do RabbitMQ: {Payload}", payload);
			await Task.CompletedTask;
		};

		await channel.BasicConsumeAsync(
			queue: queueName,
			autoAck: true,
			consumer: consumer,
			cancellationToken: stoppingToken);

		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(1000, stoppingToken);
		}
	}

}
