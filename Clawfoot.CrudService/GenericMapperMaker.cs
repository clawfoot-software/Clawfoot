using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Clawfoot.CrudService
{
    public class GenericMapperMaker
    {
        /// <summary>
        /// The GenericMapper instance
        /// </summary>
        public dynamic Accessor { get; }

        /// <summary>
        /// Creates a new GenericMapper with the provided dtoType and entityType
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dtoType"></param>
        /// <param name="entityType"></param>
        /// <param name="mapper"></param>
        public GenericMapperMaker(DbContext context, Type dtoType, Type entityType, IMapper mapper)
        {
            /* Process:
             *  - Create Generic mapper type with the provided types
             *  - Get constructor
             *  - Get Func to new it up
             *  - Invoke the Func with the parameters
             */

            Type genericBase = typeof(GenericMapper<,>);
            Type genericType = genericBase.MakeGenericType(dtoType, entityType);
            ConstructorInfo constructor = genericType.GetConstructors().Single();

            Accessor = NewGenericMapper(constructor, entityType).Invoke(context, entityType, mapper);
        }

        private Func<DbContext, Type, IMapper, dynamic> NewGenericMapper(ConstructorInfo constructor, Type entityType)
        {
            /* Process:
             *   - Setup the parameters for new GenericMapper(DbContext context, Type entity, IMapper mapper)
             *   - Create an expression out of the constructor and arguments
             *   - Create the lambda expression out of that (I have no idea what this does anymore)
             *   - Compile it
             *   - Return the Func to instantiate the GenericMapper
             */

            ParameterExpression arg1 = Expression.Parameter(typeof(DbContext), "context");
            ParameterExpression arg2 = Expression.Parameter(typeof(Type), "entity");
            ParameterExpression arg3 = Expression.Parameter(typeof(IMapper), "mapper");

            NewExpression newExpression = Expression.New(constructor, arg1, arg2, arg3);
            LambdaExpression lambda = Expression.Lambda(newExpression, false, arg1, arg2, arg3);
            Delegate result = lambda.Compile();

            return (Func<DbContext, Type, IMapper, dynamic>)result;
        }
    }
}
