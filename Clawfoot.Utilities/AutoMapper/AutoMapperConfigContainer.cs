using AutoMapper;
using AutoMapper.Configuration;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    /// <summary>
    /// The container that holds the automapper mappings for a specific Configuration Type
    /// </summary>
    internal class AutoMapperConfigContainer<TMapperConfigTypes> : IAutoMapperConfigContainer<TMapperConfigTypes>
    {
        internal AutoMapperConfigContainer(TMapperConfigTypes configType, MapperConfigurationExpression configExpression)
        {
            ConfigType = configType;
            ConfigExpression = configExpression;

            ReCompile();
        }

        private MapperConfigurationExpression ConfigExpression { get; set; }

        /// <summary>
        /// The mapper configuration type
        /// </summary>
        public TMapperConfigTypes ConfigType { get; private set; }

        /// <summary>
        /// The Mapper configuration itself, can be used to create a new mapper
        /// </summary>
        public MapperConfiguration Config { get; private set; }

        /// <summary>
        /// The Mapper instance for this Config Container
        /// </summary>
        public IMapper Mapper { get; private set; }

        /// <summary>
        /// Regenerates the Mapper Configuration. Used during instantiation and when new mappings are added
        /// </summary>
        private void ReCompile()
        {
            Config = new MapperConfiguration(ConfigExpression);
            Mapper = Config.CreateMapper();
        }

        /// <summary>
        /// Adds the provided mappings to the Configuration
        /// </summary>
        /// <param name="maps"></param>
        public void AddMappings(params Action<IMapperConfigurationExpression>[] maps)
        {
            foreach (var map in maps)
            {
                map.Invoke(ConfigExpression);
            }

            ReCompile();
        }
    }
}
