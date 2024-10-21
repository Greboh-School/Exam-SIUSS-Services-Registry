using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using School.Exam.SIUSS.Services.Registry.Repositories;
using Serilog;
using Xunit;

namespace School.Exam.SIUSS.Services.Registry.IntegrationTests.Setup;

[Collection("mysql")]
public class TestBase : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    protected const string PLAYER_KEY = "PLAYERS";
    protected const string SERVER_KEY = "SERVERS";
    protected HttpClient Client { get; }
    protected TestServer Server { get; }

    protected readonly IMemoryCache Cache;
    protected readonly IRegistryRepository Repository;
    protected readonly IServiceScope Scope;
    
    public TestBase(ApiWebApplicationFactory webApplicationFactory)
    {
        Client = webApplicationFactory.CreateClient();
        Server = webApplicationFactory.Server;
        Scope = webApplicationFactory.Services.CreateScope();
        
        Cache = Scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        Repository = Scope.ServiceProvider.GetRequiredService<IRegistryRepository>();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Log.CloseAndFlush();
        (Cache as MemoryCache)!.Compact(1.0);
        Scope.Dispose();
    }
}