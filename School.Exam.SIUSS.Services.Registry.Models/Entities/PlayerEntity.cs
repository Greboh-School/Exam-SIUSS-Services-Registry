namespace School.Exam.SIUSS.Services.Registry.Models.Entities;

public sealed class PlayerEntity
{
    public string UserName { get; set; }
    public Guid UserId { get; set; }
    public Guid ServerId { get; set; }
    public string ServerAddress { get; set; }
    public ushort ServerPort { get; set; }
}