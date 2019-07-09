using Clawfoot.Core.Enums;
using Clawfoot.Utilities.AutoMapper;
using Clawfoot.Utilities.Caches;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities
{
    /// <summary>
    /// Contains <see cref="IServiceCollection"/> extension methods to register utilities with the DI container
    /// </summary>
    public static class ServiceConfiguration
    {

        /// <summary>
        /// Registers the default <see cref="IAutoMapperConfigProvider"/> with the DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultAutoMapperProvider(this IServiceCollection services)
        {
            return services.AddSingleton<IAutoMapperConfigProvider<AutomapperConfigType>, AutoMapperConfigProvider<AutomapperConfigType>>();
        }


        /// <summary>
        /// Registers a custom <see cref="IAutoMapperConfigProvider"/> with the DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAutoMapperProvider<TConfigType>(this IServiceCollection services)
        {
            return services.AddSingleton<IAutoMapperConfigProvider<TConfigType>, AutoMapperConfigProvider<TConfigType>>();
        }

        /// <summary>
        /// Registers the <see cref="IForeignKeyPropertyCache"/> with the DI containers
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddForeignKeyPropertyCache(this IServiceCollection services)
        {
            return services.AddSingleton<IForeignKeyPropertyCache, ForeignKeyPropertyCache>();
        }
    }
}
