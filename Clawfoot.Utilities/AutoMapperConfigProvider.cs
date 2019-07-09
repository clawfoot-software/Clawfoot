using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities
{
    public class AutoMapperConfigProvider
    {
        public AutoMapperConfigProvider()
        {
            MapperCache = new Dictionary<AutomapperConfigType, AutoMapperConfigContainer>();
        }

        private Dictionary<AutomapperConfigType, AutoMapperConfigContainer> MapperCache { get; set; }

        /// <summary>
        /// Configures the static Mapper instance and adds provided config to the MapperCache as the default
        /// </summary>
        /// <param name="config"></param>
        public void ConfigureDefaultMapper(MapperConfigurationExpression config)
        {
            Mapper.Initialize(config);

            AddOrReplaceConfiguration(AutomapperConfigType.Default, config);
        }

        public MapperConfiguration AddConfiguration(AutomapperConfigType type, MapperConfigurationExpression config)
        {
            VerifyMapperDoesntExist(type);

            AutoMapperConfigContainer container = new AutoMapperConfigContainer(type, config);
            MapperCache.Add(type, container);
            return container.Config;
        }

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

        public MapperConfiguration AppendToConfiguration(AutomapperConfigType type, params Action<IMapperConfigurationExpression>[] configs)
        {
            VerifyMapperExists(type);

            MapperCache[type].AddMappings(configs);
            return MapperCache[type].Config;
        }

        public MapperConfiguration AddOrReplaceConfiguration(AutomapperConfigType type, MapperConfigurationExpression config)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, config);
        }

        public MapperConfiguration AddOrReplaceConfiguration(AutomapperConfigType type, params Action<IMapperConfigurationExpression>[] configs)
        {
            if (MapperCache.ContainsKey(type))
            {
                MapperCache.Remove(type);
            }

            return AddConfiguration(type, configs);
        }

        public IMapper GetMapper(AutomapperConfigType type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new InvalidOperationException($"No config exists to create Mapper for ConfigType: {type}. Ensure this has been added.");
            }

            return MapperCache[type].Mapper;
        }


        private void VerifyMapperDoesntExist(AutomapperConfigType type)
        {
            if (MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} already exists in mapper cache");
            }
        }

        private void VerifyMapperExists(AutomapperConfigType type)
        {
            if (!MapperCache.ContainsKey(type))
            {
                throw new ArgumentException($"Config Type {type} does not exist in the mapper cache");
            }
        }

    }
}
