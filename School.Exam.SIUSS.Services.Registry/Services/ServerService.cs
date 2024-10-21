using Mapster;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Entities;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Repositories;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Exam.SIUSS.Services.Registry.Services;

public interface IServerService
{
    public Task<ServerDTO> Create(ServerRegistrationRequest request);
    public Task<List<ServerDTO>> GetAll();
    public Task<ServerDTO> Get(Guid id);
    public void DeleteAll();
    public void Delete(Guid id);
}

public class ServerService(IRegistryRepository repository, ILogger<ServerService> logger) : IServerService
{
    public async Task<ServerDTO> Create(ServerRegistrationRequest request)
    {
        var entity = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = request.Address,
            Port = request.Port,
            ListenAddress = request.ListenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        await repository.AddServer(entity);
        logger.LogInformation("Successfully registered Server with information {information}", entity.ToString());

        return entity.Adapt<ServerDTO>();
    }

    public async Task<List<ServerDTO>> GetAll()
    {
        var servers = await repository.GetAllServers();
        var dto = servers.Adapt<List<ServerDTO>>();

        return dto;
    }

    public Task<ServerDTO> Get(Guid id)
    {
        var entity = repository.GetServer(id);
        if (entity is null)
        {
            logger.LogError("Failed to find server with id {id}", id);
            throw new NotFoundException($"Failed to find server with id {id}");
        }

        var dto = entity.Adapt<ServerDTO>();

        return Task.FromResult(dto);
    }

    public void DeleteAll()
    {
        repository.DeleteAllServers();
    }

    public void Delete(Guid id)
    {
        repository.DeleteServer(id);
    }
}