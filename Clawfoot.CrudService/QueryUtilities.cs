using Clawfoot.Core.Enums;
using Clawfoot.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Clawfoot.CrudService
{
    public class QueryUtilities
    {
        /// <summary>
        /// Applies your conditions against an IQueryable
        /// </summary>
        /// <typeparam name="TItem">The type of the data in the data source this will be applied against</typeparam>
        /// <typeparam name="TCondition">The type of the condition</typeparam>
        /// <param name="query">The IQueryable query</param>
        /// <param name="conditions">The map of conditions</param>
        /// <param name="expressionType">The expression type, limited to basic conditions</param>
        /// <returns></returns>
        public static IQueryable<TItem> ApplyConditions<TItem, TCondition>(IQueryable<TItem> query, MultiMap<TCondition> conditions, ExpressionType expressionType)
        {
            if (conditions is null || conditions.Count == 0)
            {
                return query;
            }

            IQueryable<TItem> output = query;
            foreach (KeyValuePair<string, List<TCondition>> condition in conditions)
            {
                foreach (TCondition item in condition.Value)
                {
                    output = output.Where(GetPredicate<TItem, TCondition>(expressionType, condition.Key, item));

                }
            }
            return output;
        }

        /// <summary>
        /// Applies your conditions against an IQueryable with collection conditions (ie. contains)
        /// </summary>
        /// <typeparam name="TItem">The type of the data in the data source this will be applied against</typeparam>
        /// <typeparam name="TCondition">The type of the condition</typeparam>
        /// <param name="query">The IQueryable query</param>
        /// <param name="conditions">The map of conditions</param>
        /// <param name="expressionType">The expression type,</param>
        /// <returns></returns>
        public static IQueryable<TItem> ApplyConditions<TItem, TCondition>(IQueryable<TItem> query, MultiMap<List<TCondition>> conditions, CollectionExpressionType expressionType)
        {
            if (conditions is null || conditions.Count == 0)
            {
                return query;
            }

            IQueryable<TItem> output = query;
            foreach (KeyValuePair<string, List<List<TCondition>>> condition in conditions)
            {
                foreach (List<TCondition> item in condition.Value)
                {
                    output = output.Where(GetPredicate<TItem, TCondition>(expressionType, condition.Key, item));

                }
            }
            return output;
        }

        /// <summary>
        /// Retrieves an expression predicate for a single item comparison
        /// </summary>
        /// <typeparam name="TItem">The type of the data in the data source this will be applied against</typeparam>
        /// <typeparam name="TCondition">The type of the value being compared to the property</typeparam>
        /// <param name="expressionType">See method for valid values</param>
        /// <param name="propertyName">The name of the property being compared</param>
        /// <param name="value">The value being comapred to the property</param>
        /// <returns></returns>
        public static Expression<Func<TItem, bool>> GetPredicate<TItem, TCondition>(ExpressionType expressionType, string propertyName, TCondition value)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TItem), "item");
            Expression property = Expression.Property(parameter, propertyName);
            Expression constant = Expression.Constant(value);
            Expression condition;
            switch (expressionType)
            {
                case ExpressionType.GreaterThan:
                    condition = Expression.GreaterThan(property, constant);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    condition = Expression.GreaterThanOrEqual(property, constant);
                    break;
                case ExpressionType.LessThan:
                    condition = Expression.LessThan(property, constant);
                    break;
                case ExpressionType.LessThanOrEqual:
                    condition = Expression.LessThanOrEqual(property, constant);
                    break;
                case ExpressionType.Equal:
                    condition = Expression.Equal(property, constant);
                    break;
                case ExpressionType.NotEqual:
                    condition = Expression.NotEqual(property, constant);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Expression.Lambda<Func<TItem, bool>>(condition, parameter);
        }

        public static Expression<Func<TItem, bool>> GetPredicate<TItem, TCondition>(CollectionExpressionType expressionType, string propertyName, List<TCondition> values)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TItem), "item");
            Expression property = Expression.Property(parameter, propertyName);
            Expression constant = Expression.Constant(values);

            MethodInfo method;
            switch (expressionType)
            {
                case CollectionExpressionType.Contains:
                    method = typeof(List<TCondition>).GetMethod("Contains", new Type[] { typeof(TCondition) });
                    break;
                default:
                    throw new NotImplementedException();
            }

            Expression finalMethod = Expression.Call(constant, method, property);
            return Expression.Lambda<Func<TItem, bool>>(finalMethod, parameter);
        }
    }
}
