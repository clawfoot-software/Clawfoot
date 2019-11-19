using AutoMapper;
using Clawfoot.Status.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Extensions.Automapper
{
    public static class StatusExtensions
    {
        public static IStatus<TToResult> MapResultTo<TFromResult, TToResult>(this IStatus<TFromResult> status, IMapper mapper)
        {
            TToResult result = mapper.Map<TToResult>(status.Result);
            return status.ConvertTo<TToResult>(result);
        }
    }
}
