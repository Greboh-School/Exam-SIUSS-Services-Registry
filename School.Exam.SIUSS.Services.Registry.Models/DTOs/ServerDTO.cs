using School.Exam.SIUSS.Services.Registry.Models.Entities;

namespace School.Exam.SIUSS.Services.Registry.Models.DTOs;

public sealed record ServerDTO(Guid Id, string Address, ushort Port, ServerProperties Properties);