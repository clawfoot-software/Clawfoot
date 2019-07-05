using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Utilities.Caches
{
    public interface IForeignKeyPropertyCache
    {
        ModelForeignKeyProperties GetOrAdd(Type type);
        bool Contains(Type type);
    }
}
