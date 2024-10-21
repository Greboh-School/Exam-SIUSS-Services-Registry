using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using School.Exam.SIUSS.Services.Registry.IntegrationTests.Setup;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Entities;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Shared.Tools.Test.Extensions;
using Xunit;

namespace School.Exam.SIUSS.Services.Registry.IntegrationTests.Controllers;

public class PlayerControllerTests(ApiWebApplicationFactory webApplicationFactory) : TestBase(webApplicationFactory)
{
    private const string _baseAddress = "api/v1/players";

    [Fact]
    public async Task Create_ValidRequest_ReturnsOkCreatedAndCreatesPlayer()
    {
        // Arrange
        const string username = "Tester";
        var userId = Guid.NewGuid();

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "0000",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        var expectation = new PlayerDTO
        (
            UserId: userId,
            UserName: username,
            ServerId: server.Id,
            ServerAddress: server.Address,
            ServerPort: server.Port
        );

        await Repository.AddServer(server);
        
        var request = new PlayerConnectionRequest(username, userId);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().ContainSingle();
        
        var result = await response.ReadAsAsync<PlayerDTO>();
        result.Should().BeEquivalentTo(expectation);
    }
 
    [Fact]
    public async Task Create_ValidRequestNoServersAvailable_ReturnsNotFound()
    {
        // Arrange
        const string username = "Tester";
        var userId = Guid.NewGuid();
        
        var request = new PlayerConnectionRequest(username, userId);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateWithServerId_ValidRequest_ReturnsOkCreatedAndCreatesPlayer()
    {
        // Arrange
        const string username = "Tester";
        var guid = Guid.NewGuid();

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "0000",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        var expectation = new PlayerDTO
        (
            UserId: guid,
            UserName: username,
            ServerId: server.Id,
            ServerAddress: server.Address,
            ServerPort: server.Port
        );

        await Repository.AddServer(server);
        
        var request = new PlayerConnectionRequest(username, guid);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}/{server.Id}", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().ContainSingle();
        
        var result = await response.ReadAsAsync<PlayerDTO>();
        result.Should().BeEquivalentTo(expectation);
    }
    
    [Fact]
    public async Task CreateWithServerId_ValidRequestNoServersFound_ReturnsNotFound()
    {
        // Arrange
        const string username = "Tester";
        var userId = Guid.NewGuid();
        
        var request = new PlayerConnectionRequest(username, userId);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}/{Guid.NewGuid()}", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateWithServerId_ValidRequestNoServersAvailable_ReturnsBadRequest()
    {
        // Arrange
        const string username = "Tester";
        var guid = Guid.NewGuid();

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "0000",
            Properties = new()
            {
                PlayerCount = 2,
                MaxPlayerCount = 2
            }
        };

        await Repository.AddServer(server);
        
        var request = new PlayerConnectionRequest(username, guid);
        
        // Act
        var response = await Client.PostAsJsonAsync($"{_baseAddress}/{server.Id}", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_ValidRequest_ReturnsDTO()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        const string username = "Tester";
        const string serverAddress = "127.0.0.1";
        const ushort serverPort = 8888;

        var player = new PlayerEntity
        {
            UserId = userId,
            UserName = username,
            ServerId = serverId,
            ServerAddress = serverAddress,
            ServerPort = serverPort
        };
        await Repository.AddPlayer(player);

        var expectation = new PlayerDTO
        (
            UserId: userId,
            UserName: username,
            ServerId: serverId,
            ServerAddress: serverAddress,
            ServerPort: serverPort
        );
        
        // Act
        var response = await Client.GetAsync($"{_baseAddress}/{userId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.ReadAsAsync<PlayerDTO>();
        result.Should().BeEquivalentTo(expectation);
    }   
    
    [Fact]
    public async Task Get_ValidRequestPlayerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Act
        var response = await Client.GetAsync($"{_baseAddress}/{userId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
    }

    [Fact]
    public async Task Delete_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var serverId = Guid.NewGuid();
        const string username = "Tester";
        const string serverAddress = "127.0.0.1";
        const ushort serverPort = 8888;
        
        var server = new ServerEntity
        {
            Id = serverId,
            Address = serverAddress,
            Port = serverPort,
            ListenAddress = "0000",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        var player = new PlayerEntity
        {
            UserId = userId,
            UserName = username,
            ServerId = serverId,
            ServerAddress = serverAddress,
            ServerPort = serverPort
        };
        
        await Repository.AddServer(server);
        await Repository.AddPlayer(player);
        
        // Act
        var response = await Client.DeleteAsync($"{_baseAddress}/{userId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().BeEmpty();
    }
    
}