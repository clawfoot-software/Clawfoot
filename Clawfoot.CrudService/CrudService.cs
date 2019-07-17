using AutoMapper;
using Clawfoot.Core.Enums;
using Clawfoot.Core.Interfaces;
using Clawfoot.CrudService.Models;
using Clawfoot.Utilities.AutoMapper;
using Clawfoot.Utilities.Caches;
using Clawfoot.Utilities.Status;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clawfoot.CrudService
{
    // This service and it's utilities are based on https://github.com/JonPSmith/EfCore.GenericServices

    /// <summary>
    /// A generic service that provides CRUD actions for entities or DTOs that implement <see cref="ILinkToEntity{TEntity}"/>
    /// This should not be manually newed up, and should be setup as Transient in your DI container
    /// </summary>
    public class CrudService : CrudService<DbContext>
    {
        public CrudService(DbContext context, ForeignKeyPropertyCache propertyCache, AutoMapperConfigProvider<AutomapperConfigType> autoMapperConfigProvider)
            : base(context, propertyCache, autoMapperConfigProvider) { }
    }

    /// <summary>
    /// A generic service that provides CRUD actions for entities or DTOs that implement <see cref="ILinkToEntity{TEntity}"/>
    /// This should not be manually newed up, and should be setup as Transient in your DI container
    /// </summary>
    /// <typeparam name="TContext">The Entity Framework DbContext type</typeparam>
    public class CrudService<TContext> : GenericStatus, ICrudService<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ForeignKeyPropertyCache _propertyCache;
        private readonly AutoMapperConfigProvider<AutomapperConfigType> _autoMapperConfigProvider;

        public CrudService(TContext context, ForeignKeyPropertyCache propertyCache, AutoMapperConfigProvider<AutomapperConfigType> autoMapperConfigProvider)
        {
            _context = context ?? throw new ArgumentNullException("The DbContext class is null. Please ensure it is registered with the DI container");
            _propertyCache = propertyCache ?? throw new ArgumentNullException("The ForeignKeyPropertyCache is null. Please ensure it is registered with the DI container");
            _autoMapperConfigProvider = autoMapperConfigProvider ?? throw new ArgumentNullException("The AutoMapperConfigProvider is null. Please ensure it is registered with the DI container");
        }

        //TODO
        // Add some sort of ingrained error handler into this that catches exceptions that are thrown during validations
        // Like how GenericLibraries does

        //TODO
        // Expand relations doesn't work with non DTO's on the Read methods

        /// <inheritdoc/>
        public bool EntityExists(object key, Type entityOrDto)
        {
            Type entityType = _context.GetEntityType(entityOrDto);

            object found = _context.Find(entityType, key);

            return !(found is null);
        }

        /// <inheritdoc/>
        public ICollection<T> ReadMany<T>(bool expandRelations = true) where T : class, IModel
        {
            ICollection<T> result;
            Type entityType = _context.GetEntityType(typeof(T));

            if (entityType == typeof(T))
            {
                result = _context.Set<T>().ToList();
            }
            else
            {
                AutomapperConfigType mapperType = expandRelations ? AutomapperConfigType.Default : AutomapperConfigType.NoExpandedRelations;
                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityType, _autoMapperConfigProvider.GetMapper(mapperType));
                result = ((IQueryable<T>)mapper.Accessor.GetDtoQueryable()).ToList();
            }
            return result;
        }

        /// <inheritdoc/>
        public T ReadSingle<T>(object key, bool expandRelations = true) where T : class, IModel
        {
            T result;
            Type entityType = _context.GetEntityType(typeof(T));

            //This is an entity
            if (entityType == typeof(T))
            {
                result = _context.Set<T>().Find(key);
            }
            else //it's a DTO
            {
                AutomapperConfigType mapperType = expandRelations ? AutomapperConfigType.Default : AutomapperConfigType.NoExpandedRelations;

                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityType, _autoMapperConfigProvider.GetMapper(mapperType));
                result = ((IQueryable<T>)mapper.Accessor.GetDtoQueryableByEntityKey(key)).SingleOrDefault();
            }

            if (!(result is null))
            {
                return result;
            }

            return null;
        }

        /// <inheritdoc/>
        public T ReadSingle<T>(Expression<Func<T, bool>> whereExpression, bool expandRelations = true) where T : class, IModel
        {
            T result;
            Type entityType = _context.GetEntityType(typeof(T));

            //This is an entity
            if (entityType == typeof(T))
            {
                result = _context.Set<T>().Where(whereExpression).SingleOrDefault();
            }
            else //it's a DTO
            {
                AutomapperConfigType mapperType = expandRelations ? AutomapperConfigType.Default : AutomapperConfigType.NoExpandedRelations;

                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityType, _autoMapperConfigProvider.GetMapper(mapperType));
                result = ((DbSet<T>)mapper.Accessor.GetDbSet()).Where(whereExpression).SingleOrDefault();
            }

            if (!(result is null))
            {
                return result;
            }

            return null;
        }

        /// <inheritdoc/>
        public T CreateAndSave<T>(T entityOrDto) where T : class, IModel
        {
            EntityTypeDescription entityDescription = _context.GetEntityDescription(typeof(T));

            if (!ValidateModelForeignKeys<T>(entityOrDto))
            {
                return entityOrDto;
            }

            return CreateAndSave<T>(entityOrDto, entityDescription);
        }


        private T CreateAndSave<T>(T entityOrDto, EntityTypeDescription entityDescription) where T : class
        {
            //This is an entity
            if (entityDescription.EntityType == typeof(T))
            {
                _context.Add(entityOrDto);
                _context.SaveChanges();
            }
            else
            {
                object entityInstance = Activator.CreateInstance(entityDescription.EntityType);
                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityDescription.EntityType, _autoMapperConfigProvider.GetMapper(AutomapperConfigType.Default));
                object entity = mapper.Accessor.GetEntityFromDto(entityOrDto, entityInstance);
                _context.Add(entity);
                _context.SaveChanges();

                entity.CopyBackKeysFromEntityToDto<T>(entityOrDto, entityDescription);
            }

            return entityOrDto;
        }

        /// <inheritdoc/>
        public void UpdateAndSave<T>(T entityOrDto) where T : class, IModel
        {
            EntityTypeDescription entityDescription = _context.GetEntityDescription(typeof(T));

            UpdateAndSave<T>(entityOrDto, entityDescription);
        }

        private void UpdateAndSave<T>(T entityOrDto, EntityTypeDescription entityDescription) where T : class
        {
            if (entityDescription.EntityType == typeof(T))
            {
                if (!_context.Entry(entityOrDto).IsKeySet)
                {
                    throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(T).Name}. For an update the key(s) must be set");
                }

                if (_context.Entry(entityOrDto).State == EntityState.Detached)
                {
                    _context.Update(entityOrDto);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("An Unexpected Error Occured");
                }
            }
            else
            {
                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityDescription.EntityType, _autoMapperConfigProvider.GetMapper(AutomapperConfigType.Default));
                PropertyInfo key = (PropertyInfo)mapper.Accessor.EntityKey;
                PropertyInfo dtoKey = typeof(T).GetProperties().ToList().FindBestMatch(key);

                object keyValue = dtoKey.GetValue(entityOrDto);
                var entity = mapper.Accessor.GetEntityByKey(keyValue);

                UpdateAndSave(entityOrDto, entityDescription, entity);
            }
        }

        private void UpdateAndSave<T>(T entityOrDto, EntityTypeDescription entityDescription, dynamic entity) where T : class
        {
            if (entity == null)
            {
                throw new Exception($"The primary key was not set on the entity class {entityDescription.GetType().Name}. For an update the key(s) must be set");
            }

            //HACK: Becuase the getter for this tracks the entity, we must detach it before updating. This probably has a perforance hit...
            if (_context.Entry(entity).State != EntityState.Detached)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }

            IMapper mapper = _autoMapperConfigProvider.GetMapper(AutomapperConfigType.Default);
            var toUpdate = mapper.Map(entityOrDto, typeof(T), entityDescription.EntityType);

            if (_context.Entry(toUpdate).State == EntityState.Detached)
            {
                _context.Update(toUpdate);
                _context.SaveChanges();
            }

        }

        /// <inheritdoc/>
        public T CreateOrUpdateAndSave<T>(T entityOrDto, out bool updated) where T : class, IModel
        {
            //throw new NotImplementedException();
            EntityTypeDescription entityDescription = _context.GetEntityDescription(typeof(T));

            if (!ValidateModelForeignKeys<T>(entityOrDto))
            {
                updated = false;
                return entityOrDto;
            }

            bool isEntity = entityDescription.EntityType == typeof(T);
            bool isKeySet = isEntity ? _context.Entry(entityOrDto).IsKeySet : false;

            if (isEntity && isKeySet)
            {
                UpdateAndSave<T>(entityOrDto, entityDescription);
            }
            else if (isEntity && !isKeySet)
            {
                updated = false;
                return CreateAndSave<T>(entityOrDto, entityDescription);
            }
            else
            {
                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityDescription.EntityType, _autoMapperConfigProvider.GetMapper(AutomapperConfigType.Default));
                PropertyInfo key = (PropertyInfo)mapper.Accessor.EntityKey;
                PropertyInfo dtoKey = typeof(T).GetProperties().ToList().FindBestMatch(key);

                object keyValue = dtoKey.GetValue(entityOrDto);
                var entity = mapper.Accessor.GetEntityByKey(keyValue);

                if (entity == null) //no entity matches
                {
                    updated = false;
                    return CreateAndSave<T>(entityOrDto, entityDescription);
                }
                else
                {
                    UpdateAndSave(entityOrDto, entityDescription, entity);
                }
            }
            updated = true;
            return entityOrDto;
        }

        /// <inheritdoc/>
        public void DeleteAndSave<T>(object key) where T : class, IModel
        {
            EntityTypeDescription entityDescription = _context.GetEntityDescription(typeof(T));

            dynamic entity;

            if (entityDescription.EntityType == typeof(T))
            {
                entity = _context.Set<T>().Find(key);
            }
            else
            {
                GenericMapperMaker mapper = new GenericMapperMaker(_context, typeof(T), entityDescription.EntityType, _autoMapperConfigProvider.GetMapper(AutomapperConfigType.Default));
                entity = mapper.Accessor.GetEntityByKey(key);
            }

            if (entity == null)
            {
                AddError(
                    $"Unable to find the {entityDescription.EntityType.Name} you wanted to delete.",
                    $"The item you wish to delete does not exist"
                );
                return;
            }

            _context.Remove(entity);
            _context.SaveChanges();
        }


        private bool ValidateModelForeignKeys<T>(T model)
        {
            Type modelType = typeof(T);

            ModelForeignKeyProperties foreignKeyProperties = _propertyCache.GetOrAdd(modelType);

            foreach (ForeignKeyProperty fkPropertyInfo in foreignKeyProperties.GetList())
            {
                PropertyInfo property = fkPropertyInfo.Property; //Property with attribute
                PropertyInfo referenceProperty = fkPropertyInfo.ReferenceProperty; //Property referenced in attribute

                int? idValue = (int?)referenceProperty.GetValue(model);

                if (!idValue.HasValue)
                {
                    continue; //It's an optional foreign key with no value
                }

                // If the entity does not exist than the FK Property is invalid
                if (!EntityExists(idValue.Value, property.PropertyType))
                {
                    AddError(
                        $"Value '{idValue.Value}' for key {referenceProperty.Name} is invalid",
                        $"Provided value '{idValue.Value}' for {referenceProperty.Name} is invalid"
                    );
                    return false;
                }
            }

            return true;
        }
    }
}
