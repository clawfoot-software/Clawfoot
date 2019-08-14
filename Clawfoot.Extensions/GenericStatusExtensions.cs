using AutoMapper;
using Clawfoot.Core.Status;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Extensions
{
    public static class GenericStatusExtensions
    {
        public static IGenericStatus<TToResult> MapResultTo<TFromResult, TToResult>(this IGenericStatus<TFromResult> status, IMapper mapper)
        {
            TToResult result = mapper.Map<TToResult>(status.Result);
            return status.ConvertTo<TToResult>(result);
        }
    }
}
