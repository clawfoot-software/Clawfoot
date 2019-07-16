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
}
