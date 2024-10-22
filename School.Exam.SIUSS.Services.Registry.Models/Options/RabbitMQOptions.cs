namespace School.Exam.SIUSS.Services.Registry.Models.Options;

public class RabbitMQOptions
{
    public static string Section => "RabbitMQ";
    
    public string Host { get; set; } = default!;
    public string User { get; set; } = default!;
    public string Password { get; set; } = default!;
}