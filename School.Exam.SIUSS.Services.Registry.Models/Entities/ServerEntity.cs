namespace School.Exam.SIUSS.Services.Registry.Models.Entities;

public sealed class ServerEntity
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public ushort Port { get; set; }
    public string ListenAddress { get; set; }
    public ServerProperties Properties { get; set; }
}

public sealed class ServerProperties
{
    public int PlayerCount { get; set; }
    public int MaxPlayerCount { get; set; }
    
    public bool IsFull => PlayerCount == MaxPlayerCount;

    public override string ToString()
    {
        return $"{MaxPlayerCount}/{PlayerCount}";
    }
}