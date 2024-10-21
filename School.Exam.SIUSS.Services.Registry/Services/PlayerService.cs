using Mapster;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Entities;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Repositories;
using School.Shared.Core.Abstractions.Exceptions;

namespace School.Exam.SIUSS.Services.Registry.Services;

public interface IPlayerService
{
    public Task<PlayerDTO> Create(PlayerConnectionRequest request);
    public Task<PlayerDTO> CreateWithServerId(Guid serverId, PlayerConnectionRequest request);
    public PlayerDTO Get(Guid id);
    public Task Delete(Guid id);
}

public class PlayerService(IRegistryRepository repository, ILogger<PlayerService> logger) : IPlayerService
{
    public async Task<PlayerDTO> Create(PlayerConnectionRequest request)
    {
        var server = repository.GetFirstAvailableServer();
        if (server is null)
        {
            logger.LogError("Player tried to connect to a server but found none!");
            throw new NotFoundException("Failed to find any servers!");
        }

        var entity = new PlayerEntity
        {
            UserId = request.UserId,
            UserName = request.UserName,
            ServerId = server.Id,
            ServerAddress = server.Address,
            ServerPort = server.Port
        };

        await repository.AddPlayer(entity);

        var dto = entity.Adapt<PlayerDTO>();
        logger.LogInformation("Player registered {username} with id: {playerId} to server with ip: {serverIp}", entity.UserName, entity.UserId,
            entity.ServerId);

        return dto;
    }

    public async Task<PlayerDTO> CreateWithServerId(Guid serverId, PlayerConnectionRequest request)
    {
        var server = repository.GetServer(serverId);
        if (server is null)
        {
            logger.LogError("{userId} tried to connect to server with id: {serverId} but it doesn't exist!", request.UserId,
                serverId);
            throw new NotFoundException($"Failed to find server with provided id: {serverId}");
        }

        if (server.Properties.IsFull)
        {
            logger.LogError("{userId} tried to connect to server with id: {serverId} but it is at full capacity!", request.UserId,
                serverId);
            throw new BadRequestException($"Failed to connect to server because it's at full capacity with provided id: {serverId}");
        }

        var entity = new PlayerEntity
        {
            UserId = request.UserId,
            UserName = request.UserName,
            ServerId = server.Id,
            ServerAddress = server.Address,
            ServerPort = server.Port
        };

        await repository.AddPlayer(entity);

        var dto = entity.Adapt<PlayerDTO>();
        logger.LogInformation("Player registered {username} with id: {playerId} to server with ip: {serverIp}", entity.UserName, entity.UserId,
            entity.ServerId);

        return dto;
    }

    public PlayerDTO Get(Guid id)
    {
        var entity = repository.GetPlayer(id);
        if (entity is null)
        {
            logger.LogError("Failed to find user with id {id}", id);
            throw new NotFoundException($"Failed to find user with id {id}");
        }

        var dto = entity.Adapt<PlayerDTO>();

        return dto;
    }

    public Task Delete(Guid id)
    {
        return repository.DeletePlayer(id);
    }
}