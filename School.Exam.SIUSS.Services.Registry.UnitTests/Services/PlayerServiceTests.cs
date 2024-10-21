using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Entities;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Services;
using School.Exam.SIUSS.Services.Registry.UnitTests.Setup;
using School.Shared.Core.Abstractions.Exceptions;
using Xunit;

namespace School.Exam.SIUSS.Services.Registry.UnitTests.Services;

public class PlayerServiceTests : TestBase
{
    private readonly PlayerService _uut;

    public PlayerServiceTests()
    {
        var logger = NullLoggerFactory.Instance.CreateLogger<PlayerService>();
        _uut = new(Repository, logger);
    }

    [Fact]
    public async Task Create_ValidDTO_CreatesAndReturnsDTO()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();

        var request = new PlayerConnectionRequest(username, id);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "8888",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        await Repository.AddServer(server);

        var expectation = new PlayerDTO(id, username, server.Id, server.Address, server.Port);

        // Act
        var result = await _uut.Create(request);

        // Assert
        result.Should().BeEquivalentTo(expectation);

        Cache.TryGetValue("PLAYERS", out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().ContainSingle();
    }

    [Fact]
    public async Task Create_NoServerAvailable_ThrowsNotFoundException()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();

        var request = new PlayerConnectionRequest(username, id);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "8888",
            Properties = new()
            {
                PlayerCount = 2,
                MaxPlayerCount = 2
            }
        };
        await Repository.AddServer(server);

        // Act
        var result = async () => await _uut.Create(request);

        // Assert
        await result.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Failed to find any servers!");
    }

    [Fact]
    public async Task Create_NoServerExists_ThrowsNotFoundException()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();

        var request = new PlayerConnectionRequest(username, id);

        // Act
        var result = async () => await _uut.Create(request);

        // Assert
        await result.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage("Failed to find any servers!");
    }

    [Fact]
    public async Task CreateWithServerId_ValidDTO_CreatesAndReturnsDTO()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();
        var request = new PlayerConnectionRequest(username, id);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "8888",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        await Repository.AddServer(server);

        var expectation = new PlayerDTO(id, username, server.Id, server.Address, server.Port);

        // Act
        var result = await _uut.CreateWithServerId(server.Id, request);

        // Assert
        result.Should().BeEquivalentTo(expectation);

        Cache.TryGetValue("PLAYERS", out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateWithServerId_NoServerAvailable_ThrowsBadRequestException()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();
        var request = new PlayerConnectionRequest(username, id);

        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "8888",
            Properties = new()
            {
                PlayerCount = 2,
                MaxPlayerCount = 2
            }
        };
        await Repository.AddServer(server);

        // Act
        var result = async () => await _uut.CreateWithServerId(server.Id, request);

        // Assert
        await result.Should()
            .ThrowExactlyAsync<BadRequestException>()
            .WithMessage($"Failed to connect to server because it's at full capacity with provided id: {server.Id}");
    }

    [Fact]
    public async Task CreateWithServerId_NoServerExists_ThrowsNotFoundException()
    {
        // Arrange
        const string username = "Test";
        var id = Guid.NewGuid();

        var request = new PlayerConnectionRequest(username, id);

        // Act
        var result = async () => await _uut.CreateWithServerId(id, request);

        // Assert
        await result.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Failed to find server with provided id: {id}");
    }

    [Fact]
    public async Task Get_EntityWithIdExists_ReturnsDTO()
    {
        // Arrange
        var entity = new PlayerEntity
        {
            UserId = Guid.NewGuid(),
            UserName = "Test",
            ServerId = Guid.NewGuid(),
            ServerAddress = "127.0.0.1",
            ServerPort = 8888
        };

        var expectation = new PlayerDTO
        (
            UserId: entity.UserId,
            UserName: entity.UserName,
            ServerId: entity.ServerId,
            ServerAddress: entity.ServerAddress,
            ServerPort: entity.ServerPort
        );

        await Repository.AddPlayer(entity);

        // Act
        var result = _uut.Get(entity.UserId);

        // Assert
        result.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public void Get_EntityWithIdDoesNotExists_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Act
        var result = () => _uut.Get(id);

        // Assert
        result.Should()
            .ThrowExactly<NotFoundException>()
            .WithMessage($"Failed to find user with id {id}");
    }

    [Fact]
    public async Task Delete_EntityExists_RemovesEntity()
    {
        // Arrange
        var server = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.1",
            Port = 8888,
            ListenAddress = "8888",
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };

        var entity = new PlayerEntity
        {
            UserId = Guid.NewGuid(),
            UserName = "Test",
            ServerId = server.Id,
            ServerAddress = server.Address,
            ServerPort = server.Port
        };

        await Repository.AddServer(server);
        await Repository.AddPlayer(entity);
        
        // Act
        await _uut.Delete(entity.UserId);
        
        // Assert
        Cache.TryGetValue(PLAYER_KEY, out List<PlayerEntity>? players).Should().BeTrue();
        players.Should().BeEmpty();
    }
}