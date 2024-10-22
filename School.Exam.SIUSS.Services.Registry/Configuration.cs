using School.Exam.SIUSS.Services.Registry.Configurations;
using School.Exam.SIUSS.Services.Registry.Models.Options;
using School.Exam.SIUSS.Services.Registry.Repositories;
using School.Exam.SIUSS.Services.Registry.Services;
using School.Shared.Core.Abstractions;

namespace School.Exam.SIUSS.Services.Registry;

public class Configuration : ServiceConfiguration
{
    public override void InjectMiddleware(IApplicationBuilder builder)
    {
    }

    public override void InjectServiceRegistrations(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.Configure<RabbitMQOptions>(Configuration.GetRequiredSection(RabbitMQOptions.Section));

        if (!IsRunningInTestMode())
        {
            RabbitMQConfig.Configure(Configuration);
        }

        services.AddTransient<IRegistryRepository, RegistryRepository>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IServerService, ServerService>();
        services.AddTransient<IBrokerService, BrokerService>();
    }

    private static bool IsRunningInTestMode()
    {
        // Check if running in test environment
        var testAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName != null && (
                a.FullName.Contains("TestHost") ||
                a.FullName.Contains(".Tests") ||
                a.FullName.Contains("xunit")));
        return testAssembly;
    }
}