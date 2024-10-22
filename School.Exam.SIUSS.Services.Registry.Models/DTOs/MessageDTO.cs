namespace School.Exam.SIUSS.Services.Registry.Models.DTOs;

public enum MessageType
{
    Unknown = 0,
    Public = 1,
    Private = 2,
}

public class MessageDTO
{
    public MessageType Type { get; set; }
    public string Content { get; set; }
    public string? Sender { get; set; }
    public string? Recipient { get; set; }
}