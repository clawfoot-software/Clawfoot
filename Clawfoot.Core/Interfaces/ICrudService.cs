using Clawfoot.Core.Status;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Clawfoot.Core.Interfaces
{
    public interface ICrudService : IGenericStatus
    {
        /// <summary>
        /// Determines if the entity with the given type and key exists in the context
        /// </summary>
        /// <param name="key">The entity key</param>
        /// <param name="entityOrDto">The type of the entity or DTO that impliments <see cref="ILinkToEntity{TEntity}"/></param>
        /// <returns></returns>
        bool EntityExists(object key, Type entityOrDto);

        /// <summary>
        /// Reads all entites of <typeparamref name="T"/> that exist in the database
        /// Use with discression
        /// </summary>
        /// <typeparam name="T">The entity or Dto type</typeparam>
        /// <param name="expandRelations">Will only expand relations if this is a DTO</param>
        /// <returns></returns>
        ICollection<T> ReadMany<T>(bool expandRelations = true) where T : class, IModel;


        /// <summary>
        /// This reads a single entity or DTO
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="key">The Entities key</param>
        /// <param name="expandRelations">Whether the relations for this entity should be loaded</param>
        /// <returns>The single entity or DTO that was found by the keys. If its an entity class then it is tracked</returns>
        T ReadSingle<T>(object key, bool expandRelations = true) where T : class, IModel;

        /// <summary>
        /// This reads a single entity or DTO using a where clause
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="whereExpression">The where expression should return a single instance </param>
        /// <param name="expandRelations">Whether the relations for this entity should be loaded</param>
        /// <returns>The single entity or DTO that was found by the where clause. If its an entity class then it is tracked</returns>
        T ReadSingle<T>(Expression<Func<T, bool>> whereExpression, bool expandRelations = true) where T : class, IModel;

        /// <summary>
        /// This will create a new entity in the database. If you provide class which is an entity class (i.e. in your EF Core database) then
        /// the method will add, and then call SaveChanges. If the class you provide is a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface
        /// it will use that to create the entity with AutoMapper
        /// </summary>
        /// <typeparam name="T">This type is found from the input instance</typeparam>
        /// <param name="entityOrDto">This should either be an instance of a entity class or a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</param>
        /// <returns></returns>
        T CreateAndSave<T>(T entityOrDto) where T : class, IModel;

        /// <summary>
        /// This will update the entity referred to by the keys in the given class instance.
        /// For a entity class instance it will check the state of the instance. If its detached it will call Update, otherwise it assumes its tracked and calls SaveChanges
        /// For a DTO it will: 
        /// a) load the existing entity class using the primary key(s) in the DTO
        /// b) Try to copy the data over with AutoMapper
        /// c) Call SaveChanges
        /// </summary>
        /// <typeparam name="T">This type is found from the input instance</typeparam>
        /// <param name="entityOrDto">This should either be an instance of a entity class or a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</param>
        void UpdateAndSave<T>(T entityOrDto) where T : class, IModel;


        /// <summary>
        /// This will update the entity referred to by the keys in the given class instance. If the entity doe snot exist it will create it.
        /// For a entity class instance it will check the state of the instance. If its detached it will call Update, otherwise it assumes its tracked and calls SaveChanges
        /// For a DTO it will: 
        /// a) load the existing entity class using the primary key(s) in the DTO
        /// b) Try to copy the data over with AutoMapper
        /// c) Call SaveChanges
        /// </summary>
        /// <typeparam name="T">This type is found from the input instance</typeparam>
        /// <param name="entityOrDto">This should either be an instance of a entity class or a DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</param>
        /// <param name="updated">If the Entity was updated or not</param>
        T CreateOrUpdateAndSave<T>(T entityOrDto, out bool updated) where T : class, IModel;

        /// <summary>
        /// This will delete the entity class with the given primary key
        /// </summary>
        /// <typeparam name="T">The entity or model class you want to delete.</typeparam>
        /// <param name="key"></param>
        void DeleteAndSave<T>(object key) where T : class, IModel;
    }

    public interface ICrudService<TContext> : ICrudService
    {

    }
}
