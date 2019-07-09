using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.Core.Enums
{
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
}
