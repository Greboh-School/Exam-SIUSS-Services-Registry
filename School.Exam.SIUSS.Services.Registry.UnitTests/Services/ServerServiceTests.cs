using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using School.Exam.SIUSS.Services.Registry.Models.DTOs;
using School.Exam.SIUSS.Services.Registry.Models.Entities;
using School.Exam.SIUSS.Services.Registry.Models.Requests;
using School.Exam.SIUSS.Services.Registry.Services;
using School.Exam.SIUSS.Services.Registry.UnitTests.Setup;
using School.Shared.Core.Abstractions.Exceptions;
using Xunit;

namespace School.Exam.SIUSS.Services.Registry.UnitTests.Services;

public class ServerServiceTests : TestBase
{
    private readonly ServerService _uut;

    public ServerServiceTests()
    {
        var logger = NullLoggerFactory.Instance.CreateLogger<ServerService>();
        _uut = new(Repository, logger);
    }

    [Fact]
    public async Task Create_ValidDTO_CreatesAndReturnsDTO()
    {
        // Arrange
        var request = new ServerRegistrationRequest("127.0.0.1", 8888, "8888");

        var expectation = new ServerDTO(
            Guid.NewGuid(),
            request.Address,
            request.Port,
            new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        );
        // Act
        var result = await _uut.Create(request);

        // Assert
        result.Should().BeEquivalentTo(expectation, opt => opt.Excluding(x => x.Id));
    }

    [Fact]
    public async Task GetAll_NoServers_ReturnsEmptyList()
    {
        // Act
        var result = await _uut.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ServersExists_ReturnsListOfServers()
    {
        // Arrange
        const string address = "127.0.0.0.1";
        const string listenAddress = "0000";

        var server1 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };

        var server2 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };

        var expectation = new List<ServerDTO>()
        {
            new
            (
                Id: server1.Id,
                Address: server1.Address,
                Port: server1.Port,
                Properties: server1.Properties
            ),
            new
            (
                Id: server2.Id,
                Address: server2.Address,
                Port: server2.Port,
                Properties: server2.Properties
            )
        };

        await Repository.AddServer(server1);
        await Repository.AddServer(server2);
        
        // Act
        var result = await _uut.GetAll();
        
        // Assert
        result.Should().BeEquivalentTo(expectation);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_ServerExists_ReturnsServerDTO()
    {
        // Arrange
        const string address = "127.0.0.0.1";
        const string listenAddress = "0000";

        var server1 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        var expectation = new ServerDTO
        (
            server1.Id,
            server1.Address,
            server1.Port,
            server1.Properties
        );
        
        await Repository.AddServer(server1);
        
        // Act
        var result = await _uut.Get(server1.Id);
        
        // Assert
        result.Should().BeEquivalentTo(expectation);
    }
    
    [Fact]
    public async Task Get_ServerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var result = async () => await _uut.Get(id);
        
        // Assert
        await result.Should()
            .ThrowExactlyAsync<NotFoundException>()
            .WithMessage($"Failed to find server with id {id}");
    }

    [Fact]
    public async Task DeleteAll_ServersExists_DeletesAllServers()
    {
        // Arrange
        const string address = "127.0.0.0.1";
        const string listenAddress = "0000";

        var server1 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        var server2 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        await Repository.AddServer(server1);
        await Repository.AddServer(server2);
        
        // Act
        _uut.DeleteAll();
        
        // Assert
        Cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers).Should().BeTrue();
        servers.Should().BeEmpty();
    }
    
    [Fact]
    public async Task Delete_ServerExists_DeletesServer()
    {
        // Arrange
        const string address = "127.0.0.0.1";
        const string listenAddress = "0000";

        var server1 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = address,
            Port = 8888,
            ListenAddress = listenAddress,
            Properties = new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        };
        
        await Repository.AddServer(server1);
        
        // Act
        _uut.Delete(server1.Id);
        
        // Assert
        Cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers).Should().BeTrue();
        servers.Should().BeEmpty();
    }
}