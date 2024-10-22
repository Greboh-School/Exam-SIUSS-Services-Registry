using System.Text.Json; 
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Options;
using School.Exam.SIUSS.Services.Registry.Repositories;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Exam.SIUSS.Services.Registry.Services;

public interface IBrokerService
{
    public void Create(MessageDTO dto);
    public void CreateServerQueue(Guid userId);
}

public class BrokerService : IBrokerService
{
    private readonly IRegistryRepository _repository;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<BrokerService> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public BrokerService(IRegistryRepository repository, IOptions<RabbitMQOptions> options, ILogger<BrokerService> logger)
    {
        _repository = repository;
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            UserName = _options.User,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    
    public void Create(MessageDTO dto)
    {
        var type = Enum.GetName(dto.Type);

        dto.Sender ??= "ADMIN";
        
        var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(dto);
        switch (dto.Type)
        {
            case MessageType.Private:
            {
                var player = _repository.GetPlayerByUserName(dto.Recipient!);
                
                if (player is null)
                {
                    _logger.LogWarning("Sender: {sender} tried to send a message to {recipient} but the recipient does not have a session!", dto.Sender, dto.Recipient);    
                    throw new NotFoundException("Recipient does not have a session!");
                }

                var routingKey = $"server.{player!.ServerId}.user.{player.UserName}";
                
                _channel.BasicPublish(type, routingKey, null, serializedMessage);
                break;
            }
            case MessageType.Public:
            {
                _channel.BasicPublish(type, "", null, serializedMessage);
                break;
            }
        }
    }

    public void CreateServerQueue(Guid serverId)
    {
        var queueName = $"server.{serverId}";
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: queueName, exchange: Enum.GetName(MessageType.Private), routingKey: $"{queueName}.user.#");
        
        _channel.QueueDeclare(queue: $"{queueName}.public", durable: false, exclusive: false, autoDelete: true);
        _channel.QueueBind(queue: $"{queueName}.public", exchange: Enum.GetName(MessageType.Public), routingKey: "");
    }
}