using Clawfoot.Core.Interfaces;
using Clawfoot.CrudService.Models;
using Clawfoot.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clawfoot.CrudService
{
    public static class HelpersAndExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="EntityTypeDescription"/> for and entity if <paramref name="entityOrDto"/> is a <seealso cref="ILinkToEntity{TEntity}"/>. 
        /// If <paramref name="entityOrDto"/> is an Entity then it retrieves the <see cref="EntityTypeDescription"/> associated with that Entity
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityOrDto">The entity or Dto type</param>
        /// <returns></returns>
        public static EntityTypeDescription GetEntityDescription(this DbContext context, Type entityOrDto)
        {
            Type entityType = GetLinkedEntityFromDto(entityOrDto) ?? entityOrDto;
            IEntityType efEntityType = context.Model.FindEntityType(entityType);
            if (efEntityType is null)
            {
                throw new InvalidOperationException($"The class {entityType.Name}is not registered as an entity class in your DbContext {context.GetType().Name}");
            }
            return new EntityTypeDescription(entityType, efEntityType);
        }

        /// <summary>
        /// Retrieves and validates the Entity type of the <see cref="ILinkToEntity{TEntity}"/> relation.
        /// Or if <paramref name="entityOrDto"/> is already an Entity, validates and returns <paramref name="entityOrDto"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityOrDto"></param>
        /// <returns></returns>
        public static Type GetEntityType(this DbContext context, Type entityOrDto)
        {
            Type entityType = GetLinkedEntityFromDto(entityOrDto) ?? entityOrDto; // Get the entity type, if it's null then this is probably an entity type already
            IEntityType efEntityType = context.Model.FindEntityType(entityType);  // Confirm this is actually an entity type
            if (efEntityType is null)
            {
                throw new InvalidOperationException($"The class {entityType.Name}is not registered as an entity class in your DbContext {context.GetType().Name}");
            }
            return entityType;
        }

        /// <summary>
        /// Gets the linked entity from <paramref name="entityOrDto"/>
        /// If <paramref name="entityOrDto"/> does not impliment <see cref="ILinkToEntity"/>,  returns null
        /// </summary>
        /// <param name="entityOrDto">The Type that is either an <see cref="ILinkToEntity{TEntity}"/> or not</param>
        /// <returns>The type if entityOrDto is an Entity otherwise Null</returns>
        public static Type GetLinkedEntityFromDto(Type entityOrDto)
        {
            Type[] interfaceTypes = entityOrDto.GetInterfaces();
            if (interfaceTypes.Length == 0)
            {
                throw new ArgumentException($"The class {entityOrDto.Name} does not impliment any interfaces. Expected one of IEntity or ILinkToEntity");
            }

            foreach (Type type in interfaceTypes)
            {
                if (type.IsGenericType)
                {
                    Type genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(ILinkToEntity<>))
                    {
                        return type.GetGenericArguments().Single();
                    }
                }
            }
            return null;
        }

        // Inspired by GenericServices
        /// <summary>
        /// Retrieves the best match <see cref="PropertyInfo"/> based on the name of <paramref name="toFind"/>
        /// Assumes properties are in normal PascalCase convention
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public static PropertyInfo FindBestMatch(this ICollection<PropertyInfo> properties, PropertyInfo toFind)
        {
            double bestScore = -1;
            PropertyInfo bestMatch = null;
            foreach (PropertyInfo propertyInfo in properties)
            {
                //This matches camel and Pascal names with some degree of match       
                double score = toFind.Name.SplitPascalCase().Equals(propertyInfo.Name.SplitPascalCase(), StringComparison.InvariantCultureIgnoreCase) ? 0.7 : 0;
                //The name is a higher match, as collection types can differ
                score += toFind.PropertyType == propertyInfo.PropertyType ? 0.3 : 0;

                if (bestScore == -1 || bestScore < score)
                {
                    bestScore = score;
                    bestMatch = propertyInfo;
                }

            }

            return bestMatch;
        }

        //Inpsired by EfCore.GenericServices
        /// <summary>
        /// Copies the keys from the provided entity back to the Dto it's associated with based on common property names
        /// </summary>
        /// <typeparam name="TDto">The Dto Type</typeparam>
        /// <param name="newEntity">The newly created entity</param>
        /// <param name="dto">The DTO instance</param>
        /// <param name="description">The EntityTypeDescription created from <see cref="DbContext.GetEntityDescription()"/></param>
        public static void CopyBackKeysFromEntityToDto<TDto>(this object newEntity, TDto dto, EntityTypeDescription description)
        {
            List<IKey> primaryKeys = description.EfEntityType.GetKeys().Where(x => x.IsPrimaryKey()).ToList();
            List<PropertyInfo> keyProperties = primaryKeys.Single().Properties.Select(x => x.PropertyInfo).ToList();

            PropertyInfo[] dtoKeyProperies = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo entityKey in keyProperties)
            {
                PropertyInfo dtoMatchingProperty =
                    dtoKeyProperies.SingleOrDefault(
                        x => x.Name == entityKey.Name && x.PropertyType == entityKey.PropertyType);
                if (dtoMatchingProperty == null)
                {
                    continue;
                }

                dtoMatchingProperty.SetValue(dto, entityKey.GetValue(newEntity));
            }
        }
    }
}
