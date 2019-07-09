using AutoMapper;
using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.AutoMapper
{
    internal interface IAutoMapperConfigContainer<TMapperConfigTypes>
    {
        TMapperConfigTypes ConfigType { get; }
        MapperConfiguration Config { get; }
        IMapper Mapper { get; }
        void AddMappings(params Action<IMapperConfigurationExpression>[] maps);
    }
}
