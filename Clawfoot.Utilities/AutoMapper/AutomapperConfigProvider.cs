using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <inheritdoc/>
    public class AutoMapperConfigProvider<TMapperConfigTypes> : IAutoMapperConfigProvider<TMapperConfigTypes>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperConfigProvider"/>
        /// </summary>
        public AutoMapperConfigProvider()
        {
            MapperCache = new Dictionary<TMapperConfigTypes, AutoMapperConfigContainer<TMapperConfigTypes>>();
        }

        private Dictionary<TMapperConfigTypes, AutoMapperConfigContainer<TMapperConfigTypes>> MapperCache { get; set; }

        /// <inheritdoc/>
        public void ConfigureDefaultMapper(MapperConfigurationExpression config, TMapperConfigTypes defaultType)
        {
            Mapper.Initialize(config);

            AddOrReplaceConfiguration(defaultType, config);
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
