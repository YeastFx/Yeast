using System;
using System.Linq;
using System.Linq.Expressions;

namespace Yeast.Data
{
    /// <summary>
    /// Helper class to combine predicates
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// True predicate
        /// </summary>
        /// <typeparam name="T">Predicate's target type</typeparam>
        /// <returns>Always true predicate</returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// False predicate
        /// </summary>
        /// <typeparam name="T">Predicate's target type</typeparam>
        /// <returns>Always false predicate</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// Combines two predicates with a logical OR operator
        /// </summary>
        /// <typeparam name="T">Predicate's target type</typeparam>
        /// <param name="expr1">First predicate</param>
        /// <param name="expr2">Second predicate</param>
        /// <returns>Combined predicate</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Combines two predicates with a logical AND operator
        /// </summary>
        /// <typeparam name="T">Predicate's target type</typeparam>
        /// <param name="expr1">First predicate</param>
        /// <param name="expr2">Second predicate</param>
        /// <returns>Combined predicate</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
