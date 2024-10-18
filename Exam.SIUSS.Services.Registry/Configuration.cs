using School.Shared.Core.Abstractions;

namespace Exam_SIUSS_Services_Registry;

public class Configuration : ServiceConfiguration
{
    public override void InjectMiddleware(IApplicationBuilder builder)
    {
        
    }

    public override void InjectServiceRegistrations(IServiceCollection services)
    {
        // Persistence
        services.AddMySQLContext<ApplicationUserContext>("users", Configuration);
        
        // Options
        services.Configure<Models.Options.TokenOptions>(Configuration.GetSection(Models.Options.TokenOptions.Section)); 
        
        // Services
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationUserContext>()
            .AddDefaultTokenProviders();
        
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ITokenService, TokenService>();
    }
    }
}