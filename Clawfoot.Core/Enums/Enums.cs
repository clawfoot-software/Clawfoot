using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Enums
{
    /// <summary>
    /// The configuration type for an automapper config
    /// </summary>
    public enum AutomapperConfigType
    {
        /// <summary>
        /// The defualt configuration type for the model
        /// </summary>
        Default,

        /// <summary>
        /// A config type that will not expand entity relations when mapping
        /// </summary>
        NoExpandedRelations
    }

    /// <summary>
    /// Collection expression type used in ExpressionBuilders
    /// </summary>
    public enum CollectionExpressionType
    {
        /// <summary>
        /// A Contains() expression
        /// </summary>
        Contains = 0
    }

    /// <summary>
    /// Service types provided by Clawfoot projects
    /// Intended for use when registering with a <see cref="IServiceCollection "/> DI container
    /// </summary>
    public enum ServiceTypes
    {
        DefaultAutoMapperProvider = 1,
        ForeignKeyPropertyCache = 2,
        CrudService = 3
    }

    /// <summary>
    /// The relative Left or Right hand side
    /// </summary>
    public enum RelativeHandedSide
    {
        Undefined = 0,
        Left = 1,
        Right = 2
    }
}
