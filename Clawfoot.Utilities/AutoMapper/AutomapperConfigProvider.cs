using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <inheritdoc/>
    public class AutoMapperConfigProvider : IAutoMapperConfigProvider<AutomapperConfigType>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AutoMapperConfigProvider"/>
        /// </summary>
        public AutoMapperConfigProvider()
        {
            MapperCache = new Dictionary<AutomapperConfigType, AutoMapperConfigContainer>();
        }

        private Dictionary<AutomapperConfigType, AutoMapperConfigContainer> MapperCache { get; set; }

        /// <inheritdoc/>
        public void ConfigureDefaultMapper(MapperConfigurationExpression config)
        {
            Mapper.Initialize(config);

            AddOrReplaceConfiguration(AutomapperConfigType.Default, config);
        }

        /// <inheritdoc/>
        public MapperConfiguration AddConfiguration(AutomapperConfigType type, MapperConfigurationExpression config)
        {
            VerifyMapperDoesntExist(type);

            AutoMapperConfigContainer container = new AutoMapperConfigContainer(type, config);
            MapperCache.Add(type, container);
            return container.Config;
        }

        /// <inheritdoc/>
        public MapperConfiguration AddConfiguration(AutomapperConfigType type, params Action<IMapperConfigurationExpression>[] configs)
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
        public MapperConfiguration AppendToConfiguration(AutomapperConfigType type, params Action<IMapperConfigurationExpression>[] configs)
        {
            VerifyMapperExists(type);

            MapperCache[type].AddMappings(configs);
            return MapperCache[type].Config;
        }

        /// <inheritdoc/>
        public MapperConfiguration AddOrReplaceConfiguration(AutomapperConfigType type, MapperConfigurationExpression config)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, config);
        }

        /// <inheritdoc/>
        public MapperConfiguration AddOrReplaceConfiguration(AutomapperConfigType type, params Action<IMapperConfigurationExpression>[] configs)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, configs);
        }

        /// <inheritdoc/>
        public IMapper GetMapper(AutomapperConfigType type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new InvalidOperationException($"No config exists to create Mapper for ConfigType: {type}. Ensure this has been added.");
            }

            return MapperCache[type].Mapper;
        }

        private void VerifyMapperExists(AutomapperConfigType type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} does not exist in the mapper cache");
            }
        }

        private void VerifyMapperDoesntExist(AutomapperConfigType type)
        {
            if (MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} already exists in mapper cache");
            }
        }



    }
}
