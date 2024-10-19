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

public class ServerControllerTests(ApiWebApplicationFactory webApplicationFactory) : TestBase(webApplicationFactory)
{
    private const string _baseAddress = "api/v1/servers";

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAndCreatesServer()
    {
        // Arrange
        const string address = "127.0.0.1";
        const ushort port = 8888;
        const string listenAddress = "0000";
        
        var request = new ServerRegistrationRequest(address, port, listenAddress);

        var expectation = new ServerDTO
        (
            Id: Guid.NewGuid(),
            Address: address,
            Port: port,
            Properties: new()
            {
                PlayerCount = 0,
                MaxPlayerCount = 2
            }
        );

        // Act
        var response = await Client.PostAsJsonAsync(_baseAddress, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        Cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers).Should().BeTrue();
        servers.Should().ContainSingle();

        var result = await response.ReadAsAsync<ServerDTO>();
        result.Should().BeEquivalentTo(expectation, opt => opt.Excluding(X => X.Id));
    }

    [Fact]
    public async Task GetAll_ServersExist_ReturnsOkWithServers()
    {
        // Arrange
        var server1 = new ServerEntity
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

        var server2 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.2",
            Port = 8889,
            ListenAddress = "0001",
            Properties = new()
            {
                PlayerCount = 1,
                MaxPlayerCount = 4
            }
        };

        await Repository.AddServer(server1);
        await Repository.AddServer(server2);

        // Act
        var response = await Client.GetAsync(_baseAddress);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsAsync<List<ServerDTO>>();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_ValidId_ReturnsOkWithServer()
    {
        // Arrange
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

        await Repository.AddServer(server);

        var expectation = new ServerDTO
        (
            Id: server.Id,
            Address: server.Address,
            Port: server.Port,
            Properties: server.Properties
        );

        // Act
        var response = await Client.GetAsync($"{_baseAddress}/{server.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsAsync<ServerDTO>();
        result.Should().BeEquivalentTo(expectation);
    }

    [Fact]
    public async Task Get_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{_baseAddress}/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAll_ServersExist_ReturnsOkAndDeletesAllServers()
    {
        // Arrange
        var server1 = new ServerEntity
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

        var server2 = new ServerEntity
        {
            Id = Guid.NewGuid(),
            Address = "127.0.0.2",
            Port = 8889,
            ListenAddress = "0001",
            Properties = new()
            {
                PlayerCount = 1,
                MaxPlayerCount = 4
            }
        };

        await Repository.AddServer(server1);
        await Repository.AddServer(server2);

        // Act
        var response = await Client.DeleteAsync(_baseAddress);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers).Should().BeTrue();
        servers.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsOkAndDeletesServer()
    {
        // Arrange
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

        await Repository.AddServer(server);

        // Act
        var response = await Client.DeleteAsync($"{_baseAddress}/{server.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Cache.TryGetValue(SERVER_KEY, out List<ServerEntity>? servers).Should().BeTrue();
        servers.Should().BeEmpty();
    }
}