using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <summary>
    /// Default Marker/Helper class for <see cref="AutoMapperConfigProvider{TMapperConfigTypes}"/>
    /// implimenting <see cref="AutomapperConfigType"/>
    /// </summary>
    public class AutoMapperConfigProvider : AutoMapperConfigProvider<AutomapperConfigType>, IAutoMapperConfigProvider
    {
        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperConfigProvider"/>
        /// </summary>
        /// <param name="defaultConfiguration"></param>
        /// <param name="defaultType"></param>
        public AutoMapperConfigProvider(Action<IMapperConfigurationExpression> defaultConfiguration, AutomapperConfigType defaultType)
            : base(defaultConfiguration, defaultType) { }
    }

    /// <inheritdoc/>
    public class AutoMapperConfigProvider<TMapperConfigTypes> : IAutoMapperConfigProvider<TMapperConfigTypes> where TMapperConfigTypes : struct, System.Enum
    {
        private Dictionary<TMapperConfigTypes, AutoMapperConfigContainer<TMapperConfigTypes>> MapperCache { get; set; }

        /// <inheritdoc/>
        public TMapperConfigTypes DefaultConfigType { get; private set; }


        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperConfigProvider"/>
        /// </summary>
        public AutoMapperConfigProvider()
        {
            MapperCache = new Dictionary<TMapperConfigTypes, AutoMapperConfigContainer<TMapperConfigTypes>>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperConfigProvider"/>
        /// </summary>
        /// <param name="defaultConfiguration"></param>
        /// <param name="defaultType"></param>
        public AutoMapperConfigProvider(Action<IMapperConfigurationExpression> defaultConfiguration, TMapperConfigTypes defaultType)
            :this()
        {
            ConfigureDefaultMapper(defaultConfiguration, defaultType);
        }        

        /// <inheritdoc/>
        public void ConfigureDefaultMapper(Action<IMapperConfigurationExpression> configExpression, TMapperConfigTypes defaultType)
        {
            AddOrReplaceConfiguration(defaultType, configExpression);
            DefaultConfigType = defaultType;
        }

        /// <inheritdoc/>
        public void ConfigureDefaultMapper(MapperConfigurationExpression config, TMapperConfigTypes defaultType)
        {
            AddOrReplaceConfiguration(defaultType, config);
            DefaultConfigType = defaultType;
        }

        /// <inheritdoc/>
        public MapperConfiguration AddConfiguration(TMapperConfigTypes type, MapperConfigurationExpression config)
        {
            VerifyMapperDoesntExist(type);

            AutoMapperConfigContainer<TMapperConfigTypes> container = new AutoMapperConfigContainer<TMapperConfigTypes>(type, config);
            MapperCache.Add(type, container);
            return container.Config;
        }

        /// <inheritdoc/>
        public MapperConfiguration AddConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs)
        {
            VerifyMapperDoesntExist(type);

            MapperConfigurationExpression configExpression = new MapperConfigurationExpression();
            foreach (var config in configs)
            {
                config.Invoke(configExpression);
            }

            return AddConfiguration(type, configExpression); ;
        }

        /// <inheritdoc/>
        public MapperConfiguration AppendToConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs)
        {
            VerifyMapperExists(type);

            MapperCache[type].AddMappings(configs);
            return MapperCache[type].Config;
        }

        /// <inheritdoc/>
        public MapperConfiguration AddOrReplaceConfiguration(TMapperConfigTypes type, MapperConfigurationExpression config)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, config);
        }

        /// <inheritdoc/>
        public MapperConfiguration AddOrReplaceConfiguration(TMapperConfigTypes type, params Action<IMapperConfigurationExpression>[] configs)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, configs);
        }

        /// <inheritdoc/>
        public IMapper GetMapper(TMapperConfigTypes type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new InvalidOperationException($"No config exists to create Mapper for ConfigType: {type}. Ensure this has been added.");
            }

            return MapperCache[type].Mapper;
        }

        /// <inheritdoc/>
        public IMapper GetDefaultMapper()
        {
            // /Double cast is necessary since the generic enum constraint can't guarantee it's an int See: https://github.com/dotnet/csharplang/issues/1993
            if ((int)(object)DefaultConfigType == (int)(object)default(TMapperConfigTypes))
            {
                throw new InvalidOperationException($"Cannot retrieve default mapper, no default has been set");
            }

            return GetMapper(DefaultConfigType);
        }

        private void VerifyMapperExists(TMapperConfigTypes type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} does not exist in the mapper cache");
            }
        }

        private void VerifyMapperDoesntExist(TMapperConfigTypes type)
        {
            if (MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} already exists in mapper cache");
            }
        }
    }
}
