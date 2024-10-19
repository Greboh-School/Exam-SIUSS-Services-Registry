using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using School.Exam.SIUSS.Services.Registry.Repositories;
using Serilog;

namespace School.Exam.SIUSS.Services.Registry.UnitTests.Setup;

public class TestBase : IDisposable
{
    protected const string PLAYER_KEY = "PLAYERS";
    protected const string SERVER_KEY = "SERVERS";
    protected IMemoryCache Cache { get;  } = new MemoryCache(new MemoryCacheOptions());
    protected readonly RegistryRepository Repository;

    protected TestBase()
    {
        Repository = new(Cache);
    }
    
    
    public void Dispose()
    {
        Cache.Dispose();
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}