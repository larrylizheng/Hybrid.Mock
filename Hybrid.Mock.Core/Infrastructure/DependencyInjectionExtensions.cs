using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Hybrid.Mock.Core.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInjectionByConvention(this IServiceCollection services, string prefix, params Assembly[] assemblies)
        {
            var types = new List<Type>();

            //iterate the assemblies and add interfaces/class to list
            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes().Where(x => x.FullName.StartsWith(prefix)));
            }

            //iterate list and inject class to matching interface
            foreach (var @interface in types.Where(y => y.IsInterface))
            {
                //get the interface ignoring the 'I'
                var match = @interface.Name.Substring(1, @interface.Name.Length - 1);

                //get implementation class matching the interface
                var implementation = types.FirstOrDefault(x => x.FullName.StartsWith(prefix) && x.Name == match && x.IsClass);

                if (implementation != default && implementation.GetInterfaces().Contains(@interface))
                {
                    services.AddTransient(@interface, implementation);
                }
            }

            return services;
        }
    }
}
