using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <summary>
    /// The AutoMapper configuration provider. This is used when multiple configurations may be needed per model
    /// </summary>
    public interface IAutoMapperConfigProvider<TMapperConfigTypes> where TMapperConfigTypes : System.Enum
    {
        /// <summary>
        /// Configures the static Mapper instance and adds provided config to the MapperCache as the default
        /// </summary>
        /// <param name="config"></param>
        /// <param name="defaultType"></param>
        void ConfigureDefaultMapper(MapperConfigurationExpression config, TMapperConfigTypes defaultType);

        /// <summary>
        /// Adds a new AutoMapper configuration. Requires that the provided <see cref="TMapperConfigTypes"/> does not yet exist
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config">The expression that represents the mapper configurations</param>
        /// <returns></returns>
        MapperConfiguration AddConfiguration(TMapperConfigTypes type, MapperConfigurationExpression config);

        /// <summary>
        /// Adds a new AutoMapper configuration. Requires that the provided <see cref="TMapperConfigTypes"/> does not yet exist
        /// </summary>
        /// <param name="type"></param>
        /// <param name="configs">The actions that represent the mapper configurations</param>
        /// <returns></returns>
        MapperConfiguration AddConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs);

        /// <summary>
        /// Adds the provided mapper expressions to an existing configuration.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        MapperConfiguration AppendToConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs);

        /// <summary>
        /// Will add, or if it already exists, replace the mapper configuration for this Config Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        MapperConfiguration AddOrReplaceConfiguration(TMapperConfigTypes type, MapperConfigurationExpression config);

        /// <summary>
        /// Will add, or if it already exists, replace the mapper configuration for this Config Type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        MapperConfiguration AddOrReplaceConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs);

        /// <summary>
        /// Gets the mapper instance for the provided config type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IMapper GetMapper(TMapperConfigTypes type);
    }
}
