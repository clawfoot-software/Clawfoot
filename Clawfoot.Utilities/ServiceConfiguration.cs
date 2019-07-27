using Clawfoot.Core.Enums;
using Clawfoot.Core.Interfaces;
using Clawfoot.Utilities.AutoMapper;
using Clawfoot.Utilities.Caches;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clawfoot.Utilities
{
    /// <summary>
    /// Contains <see cref="IServiceCollection"/> extension methods to register utilities with the DI container
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Dictionary of Services provided by Clawfoot.Utilities
        /// </summary>
        public static IReadOnlyDictionary<ServiceTypes, ServiceDescriptor> ServicesCollection => new Dictionary<ServiceTypes, ServiceDescriptor>()
        {
            {
                ServiceTypes.DefaultAutoMapperProvider,
                new ServiceDescriptor(typeof(IAutoMapperConfigProvider<AutomapperConfigType>), typeof(AutoMapperConfigProvider<AutomapperConfigType>), ServiceLifetime.Singleton)
            },
            {
                ServiceTypes.ForeignKeyPropertyCache,
                new ServiceDescriptor(typeof(IForeignKeyPropertyCache), typeof(ForeignKeyPropertyCache), ServiceLifetime.Singleton)
            }
        };

        /// <summary>
        /// Registers the default <see cref="IAutoMapperConfigProvider"/> with the DI container if it does not already exist
        /// This uses <see cref="AutomapperConfigType"/> as the Config Type.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAutoMapperProvider(this IServiceCollection services)
        {
            bool automapperProviderRegistered = services.Any(x => x.ServiceType.GetGenericTypeDefinition() == typeof(IAutoMapperConfigProvider<>));

            services.TryAddSingleton<IAutoMapperConfigProvider, AutoMapperConfigProvider>();
            services.TryAddSingleton<IAutoMapperConfigProvider<AutomapperConfigType>>(x => x.GetRequiredService<IAutoMapperConfigProvider>());

            return services;
        }

        /// <summary>
        /// Registers the default <see cref="IAutoMapperConfigProvider"/> with the DI container if it does not already exist
        /// This uses <see cref="AutomapperConfigType"/> as the Config Type.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="instance">The instance of the config provider</param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAutoMapperProvider(this IServiceCollection services, IAutoMapperConfigProvider instance)
        {
            bool automapperProviderRegistered = services.Any(x => x.ServiceType.GetGenericTypeDefinition() == typeof(IAutoMapperConfigProvider<>));

            services.TryAddSingleton<IAutoMapperConfigProvider>(instance);
            services.TryAddSingleton<IAutoMapperConfigProvider<AutomapperConfigType>>(x => x.GetRequiredService<IAutoMapperConfigProvider>());

            return services;
        }


        /// <summary>
        /// Registers a custom <see cref="IAutoMapperConfigProvider{TMapperConfigTypes}"/> with the DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAutoMapperProvider<TMapperConfigType>(this IServiceCollection services) where TMapperConfigType : struct, System.Enum
        {
            return services.AddSingleton<IAutoMapperConfigProvider<TMapperConfigType>, AutoMapperConfigProvider<TMapperConfigType>>();
        }

        /// <summary>
        /// Registers the <see cref="IForeignKeyPropertyCache"/> with the DI container if it does not already exist
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddForeignKeyPropertyCache(this IServiceCollection services)
        {
            services.TryAddSingleton<IForeignKeyPropertyCache, ForeignKeyPropertyCache>();
            return services;
        }
    }
}
