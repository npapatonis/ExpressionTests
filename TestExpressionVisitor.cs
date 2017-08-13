using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public class TestExpressionVisitor : ExpressionVisitor
  {
    private readonly Dictionary<Expression, Expression> parameterMap;

    public TestExpressionVisitor(
        Dictionary<Expression, Expression> parameterMap)
    {
      this.parameterMap = parameterMap;
    }

    public Expression TransformExpression(Expression expression)
    {
      return Visit(expression);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      // re-map the parameter
      Expression found;
      if (!parameterMap.TryGetValue(node, out found))
        found = base.VisitParameter(node);
      return found;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      // re-perform any member-binding
      var expr = Visit(node.Expression);
      if (expr.Type != node.Type)
      {
        MemberInfo newMember = expr.Type.GetMember(node.Member.Name)
                                   .Single();
        return Expression.MakeMemberAccess(expr, newMember);
      }
      return base.VisitMember(node);
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      HashSet<int> hashSet = null;
      if (node.TypeOperand == typeof(InternationalCustomer))
      {
        hashSet = new HashSet<int>() { 4000 };
      }
      else if (node.TypeOperand == typeof(DomesticCustomer))
      {
        hashSet = new HashSet<int>() { 3000 };
      }
      else if (node.TypeOperand == typeof(Customer))
      {
        hashSet = new HashSet<int>() { 2000, 3000, 4000 };
      }

      Expression typePropAccess = Expression.MakeMemberAccess(
        node.Expression,
        typeof(Customer).GetProperty("Type", BindingFlags.Instance | BindingFlags.NonPublic));

      return Expression.Call(
        Expression.Constant(hashSet),
        hashSet.GetType().GetMethod("Contains"),
        typePropAccess);
    }
  }
}
