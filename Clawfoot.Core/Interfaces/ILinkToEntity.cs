using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Interfaces
{
    /// <summary>
    /// Compile-time marker interface, no need for methods
    /// This lets the services know which entity class to map the DTO to
    /// </summary>
    public interface ILinkToEntity<TEntity> : ILinkToEntity where TEntity : IEntity {}

    //TODO: Make this generic just like IEntity

    /// <summary>
    /// 
    /// </summary>
    public interface ILinkToEntity : IModel
    {
        int Id { get; set; }
    }
}
