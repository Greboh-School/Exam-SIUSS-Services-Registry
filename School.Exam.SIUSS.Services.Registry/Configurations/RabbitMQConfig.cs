using RabbitMQ.Client;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Options;

namespace School.Exam.SIUSS.Services.Registry.Configurations;

public static class RabbitMQConfig
{
    public static void Configure(IConfiguration configuration)
    {
        var options = configuration.GetRequiredSection(RabbitMQOptions.Section).Get<RabbitMQOptions>();
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            UserName = options.User,
            Password = options.Password
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(Enum.GetName(MessageType.Private), ExchangeType.Topic);
        channel.ExchangeDeclare(Enum.GetName(MessageType.Public), ExchangeType.Fanout);
    }
}