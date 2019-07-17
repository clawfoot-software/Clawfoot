using Clawfoot.Core.Enums;
using Clawfoot.Core.Interfaces;
using Clawfoot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.CrudService
{
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registers the <see cref="ICrudService"/> with the DI container
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultCrudService(this IServiceCollection services)
        {
            services.EnsureServiceRegistered(Utilities.ServiceConfiguration.ServicesCollection[ServiceTypes.DefaultAutoMapperProvider]);
            services.EnsureServiceRegistered(Utilities.ServiceConfiguration.ServicesCollection[ServiceTypes.ForeignKeyPropertyCache]);

            services.TryAddTransient<ICrudService, CrudService>();
            return services;
        }

    }
}
