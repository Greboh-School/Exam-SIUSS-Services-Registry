namespace School.Exam.SIUSS.Services.Registry.Models.Requests;

public sealed record ServerRegistrationRequest(string Address, ushort Port, string ListenAddress);