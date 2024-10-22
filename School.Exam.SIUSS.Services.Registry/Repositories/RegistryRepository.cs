using Microsoft.Extensions.Caching.Memory;
using School.Exam.SIUSS.Services.Registry.Models.Entities;

namespace School.Exam.SIUSS.Services.Registry.Repositories;

public interface IRegistryRepository
{
    #region Server

    public Task AddServer(ServerEntity entity);
    public ServerEntity? GetFirstAvailableServer();
    public Task<List<ServerEntity>> GetAllServers();
    public ServerEntity? GetServer(Guid serverId);
    public Task DeleteServer(Guid id);
    public void DeleteAllServers();

    #endregion

    #region Player

    public Task<List<PlayerEntity>> GetAllPlayers();
    public Task AddPlayer(PlayerEntity entity);
    public PlayerEntity? GetPlayer(Guid userId);
    public Task DeletePlayer(Guid id);
    public PlayerEntity? GetPlayerByUserName(string username);

    #endregion

}

public class RegistryRepository(IMemoryCache cache) : IRegistryRepository
{
    #region Server

    private const string _SERVER_KEY = "SERVERS";
    private async Task<List<ServerEntity>> GetOrCreateServers()
    {
        
        var servers = (await cache.GetOrCreateAsync(_SERVER_KEY, _ => Task.FromResult(new List<ServerEntity>())))!;
        return servers;
    }

    public async Task AddServer(ServerEntity entity)
    {
        var servers = await GetOrCreateServers();
        servers.Add(entity);

        cache.Set(_SERVER_KEY, servers);
    }

    /// <summary>
    /// Get the first available server (QuickPlay)
    /// </summary>
    /// <returns>First available server or null if none is available</returns>
    /// <remarks>Adheres to server's properties</remarks>
    public ServerEntity? GetFirstAvailableServer()
    {
        var result = cache.TryGetValue(_SERVER_KEY, out List<ServerEntity>? servers);

        if (!result)
        {
            return null;
        }

        var server = servers!.FirstOrDefault(x => !x.Properties.IsFull);

        if (server is null)
        {
            return null;
        }

        // Add to the current player count
        server.Properties.PlayerCount++;

        // Set the new cache value
        cache.Set(_SERVER_KEY, servers);

        return server;
    }

    public async Task<List<ServerEntity>> GetAllServers() => await GetOrCreateServers();

    public ServerEntity? GetServer(Guid serverId)
    {
        var result = cache.TryGetValue(_SERVER_KEY, out List<ServerEntity>? servers);
        if (!result)
        {
            return null;
        }

        var server = servers!.FirstOrDefault(x => x.Id == serverId);

        return server;
    }

    public void DeleteAllServers()
    {
        cache.Set(_SERVER_KEY, new List<ServerEntity>());
    }

    public Task DeleteServer(Guid id)
    {
        var result = cache.TryGetValue(_SERVER_KEY, out List<ServerEntity>? servers);
        if (!result)
        {
            return Task.CompletedTask;
        }

        var entity = servers!.FirstOrDefault(x => x.Id == id);
        if (entity is null)
        {
            return Task.CompletedTask;
        }

        servers!.Remove(entity);

        cache.Set(_SERVER_KEY, servers);

        cache.TryGetValue(_PLAYER_KEY, out List<PlayerEntity>? players);

        if (players is null)
        {
            return Task.CompletedTask;
        }

        players.RemoveAll(x => x.ServerId == id);
        cache.Set(_PLAYER_KEY, players);

        return Task.CompletedTask;
    }

    #endregion

    #region Player

    private const string _PLAYER_KEY = "PLAYERS";

    /// <summary>
    /// Gets the player cache or creates a new one if it doesn't exist
    /// </summary>
    /// <returns>All players if the key exists otherwise a new list</returns>
    private async Task<List<PlayerEntity>> GetOrCreatePlayers()
    {
        var players = (await cache.GetOrCreateAsync(_PLAYER_KEY, _ => Task.FromResult(new List<PlayerEntity>())))!;
        return players;
    }

    public async Task AddPlayer(PlayerEntity entity)
    {
        var players = await GetOrCreatePlayers();
        players.Add(entity);

        cache.Set(_PLAYER_KEY, players);
    }

    public async Task<List<PlayerEntity>> GetAllPlayers() => await GetOrCreatePlayers();

    public PlayerEntity? GetPlayer(Guid userId)
    {
        var entity = cache.Get<List<PlayerEntity>>(_PLAYER_KEY)?.FirstOrDefault(x => x.UserId == userId);

        return entity;
    }

    public async Task DeletePlayer(Guid id)
    {
        var result = cache.TryGetValue(_PLAYER_KEY, out List<PlayerEntity>? players);
        if (!result)
        {
            return; // Doesn't exist
        }

        var entity = players!.FirstOrDefault(x => x.UserId == id);
        if (entity is null)
        {
            return; // Doesn't exist
        }

        players!.Remove(entity);

        var servers = await GetOrCreateServers();
        servers.First(x => x.Id == entity.ServerId).Properties.PlayerCount--;

        cache.Set(_PLAYER_KEY, players);
        cache.Set(_SERVER_KEY, servers);
    }

    public PlayerEntity? GetPlayerByUserName(string username)
    {
        var entity = cache.Get<List<PlayerEntity>>(_PLAYER_KEY)?.FirstOrDefault(x => x.UserName == username);

        return entity;
    }

    #endregion
}