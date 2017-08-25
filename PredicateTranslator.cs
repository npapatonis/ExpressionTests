using G1T.Dc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTests
{
  public abstract class PredicateTranslator<TEntity, TDbEntity>
    where TEntity : EntityBase
    where TDbEntity : IDbEntity
  {
    #region =====[ Protected Methods ]=============================================================================

    protected abstract EntityExpressionVisitor<TDbEntity> CreateExpressionVisitor(Dictionary<Expression, Expression> parameterMap);

    protected Dictionary<Expression, Expression> CreateParameterMap(LambdaExpression expression)
    {
      Dictionary<Expression, Expression> parameterMap;

      if (expression.Parameters.Count != 1) throw new ArgumentException("Wrong number of expression parameters");
      if (!typeof(TEntity).IsAssignableFrom(expression.Parameters[0].Type))
      {
        throw new ArgumentException("First expression parameter is not the correct type");
      }

      parameterMap = new Dictionary<Expression, Expression>();
      parameterMap[expression.Parameters[0]] = Expression.Parameter(typeof(TDbEntity), expression.Parameters[0].Name);

      return parameterMap;
    }

    #endregion

    #region =====[ Public Methods ]================================================================================

    public Expression<Func<TDbEntity, bool>> Translate(LambdaExpression expression)
    {
      if (expression == null) return null;

      var parameterMap = CreateParameterMap(expression);

      var visitor = CreateExpressionVisitor(parameterMap);

      var body = visitor.Visit(expression.Body);

      var newParams = parameterMap.Values.Cast<ParameterExpression>().ToArray();
      return Expression.Lambda<Func<TDbEntity, bool>>(body, newParams);
    }

    #endregion
  }
}
