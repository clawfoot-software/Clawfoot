using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Clawfoot.CrudService
{
    public class GenericMapper<TDto, TEntity>
        where TDto : class
        where TEntity : class
    {

        private readonly DbContext _context;
        private readonly PropertyInfo _entityKey;
        private readonly IMapper _mapper;

        public GenericMapper(DbContext context, Type entity, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _entityKey = entity.GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(KeyAttribute))).SingleOrDefault();

            _mapper = mapper;
        }

        public PropertyInfo EntityKey
        {
            get => _entityKey;
        }

        //Get and save key property
        //Create equals predicate from key property and supplied key
        //Filter DbSet with that key
        //Project to Dto

        public DbSet<TEntity> GetDbSet()
        {
            return _context.Set<TEntity>();
        }

        public TEntity GetEntityByKey(object key)
        {
            return GetDbSet().Find(key);
        }

        public IQueryable<TEntity> GetEntityQueryableByKey(object key)
        {
            return GetDbSet().Where(CreateFilter(key));
        }

        public IQueryable<TDto> GetDtoQueryableByEntityKey(object key)
        {
            return GetEntityQueryableByKey(key).ProjectTo<TDto>(_mapper.ConfigurationProvider);
        }

        public IQueryable<TEntity> GetEntityQueryable()
        {
            return GetDbSet().AsQueryable();
        }

        public IQueryable<TDto> GetDtoQueryable()
        {
            return GetDbSet().ProjectTo<TDto>(_mapper.ConfigurationProvider);
        }

        public object GetEntityFromDto(TDto dto, object entity)
        {
            return _mapper.Map(dto, entity);
        }

        private Expression<Func<TEntity, bool>> CreateFilter(object key)
        {
            return QueryUtilities.GetPredicate<TEntity, object>(ExpressionType.Equal, _entityKey.Name, key);
        }
    }
}
