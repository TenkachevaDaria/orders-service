using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;

namespace Tests.E2E;

public static class RebusReplacements
{
   public static void ReplaceRebus(
        IServiceCollection services,
        Func<RebusConfigurer, RebusConfigurer> configure)
    {
        foreach (var d in services.Where(s =>
                     s.ServiceType.FullName!.StartsWith("Rebus")).ToList())
        {
            services.Remove(d);
        }

        services.AddRebus(configure);
    }
}