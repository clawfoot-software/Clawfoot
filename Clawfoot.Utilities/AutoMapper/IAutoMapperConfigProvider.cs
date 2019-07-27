using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <summary>
    /// Default Marker/Helper interface for <see cref="IAutoMapperConfigContainer{TMapperConfigTypes}"/>
    /// implimenting <see cref="AutomapperConfigType"/>
    /// </summary>
    public interface IAutoMapperConfigProvider : IAutoMapperConfigProvider<AutomapperConfigType>
    {

    }


    /// <summary>
    /// The AutoMapper configuration provider. This is used when multiple configurations may be needed per model
    /// </summary>
    public interface IAutoMapperConfigProvider<TMapperConfigTypes> where TMapperConfigTypes : struct, System.Enum
    {
        /// <summary>
        /// The type assocaited with the default mapper. Set when ConfigureDefaultMapper is called
        /// </summary>
        TMapperConfigTypes DefaultConfigType { get; }

        /// <summary>
        /// Configures the static Mapper instance and adds provided config to the MapperCache as the default
        /// </summary>
        /// <param name="configExpression"></param>
        /// <param name="defaultType"></param>
        void ConfigureDefaultMapper(Action<IMapperConfigurationExpression> configExpression, TMapperConfigTypes defaultType);


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

        /// <summary>
        /// Gets the default mapper that has been configured
        /// </summary>
        /// <returns></returns>
        IMapper GetDefaultMapper();
    }
}
