using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public class TestExpressionVisitor : ExpressionVisitor
  {
    private readonly Dictionary<Expression, Expression> parameterMap;
    private readonly Dictionary<string, Dictionary<string, string>> memberMap;

    public TestExpressionVisitor(
        Dictionary<Expression, Expression> parameterMap,
        Dictionary<string, Dictionary<string, string>> memberMap)
    {
      this.parameterMap = parameterMap;
      this.memberMap = memberMap;
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
      if (expr.Type != node.Expression.Type)
      {
        // Try to get a public member by the same name
        MemberInfo memberInfo = expr.Type.GetMember(
          node.Member.Name,
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        ).SingleOrDefault();

        if (memberInfo == null)
        {
          // Try to get a non-public member by the same name
          memberInfo = expr.Type.GetMember(
            node.Member.Name,
            BindingFlags.Instance | BindingFlags.NonPublic
          ).SingleOrDefault();
        }

        if (memberInfo == null)
        {
          memberInfo = expr.Type.GetMember(
            memberMap[node.Expression.Type.Name][node.Member.Name],
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
          ).SingleOrDefault();
        }

        return Expression.MakeMemberAccess(expr, memberInfo);
      }
      return base.VisitMember(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
      Expression expr = this.Visit(node.Operand);
      if (expr != node.Operand)
      {
        return expr;
      }
      return node;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      var expr = Visit(node.Expression);
      if (expr.Type != node.Expression.Type)
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
          expr,
          typeof(TblCustomer).GetProperty("Type", BindingFlags.Instance | BindingFlags.NonPublic));

        return Expression.Call(
          Expression.Constant(hashSet),
          hashSet.GetType().GetMethod("Contains"),
          typePropAccess);
      }
      return base.VisitTypeBinary(node);
    }
  }
}
