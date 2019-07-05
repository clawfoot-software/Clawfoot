using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Interfaces
{
    //TODO: Make IEntity generic, not all keys will be the same type

    /// <summary>
    /// An entity
    /// </summary>
    public interface IEntity : IModel
    {
        int Id { get; set; }
    }
}
