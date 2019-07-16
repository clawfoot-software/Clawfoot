using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clawfoot.CrudService.Models
{
    public class EntityTypeDescription
    {
        public EntityTypeDescription(Type entityType, IEntityType efEntityType)
        {
            EntityType = entityType;
            EfEntityType = efEntityType;
        }

        /// <summary>
        /// The Type of the entity
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// The ENtity Framework type for the entity
        /// </summary>
        public IEntityType EfEntityType { get; private set; }

        /// <summary>
        /// Valid if the type is an entity or a DTO, 
        /// </summary>
        public bool IsValid { get; private set; }
    }
}
