using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public class TestExpressionVisitor : ExpressionVisitor
  {
    private readonly Dictionary<Expression, Expression> parameterMap;
    private readonly Dictionary<string, Tuple<Type, string>> memberMap;

    public TestExpressionVisitor(
      Dictionary<Expression, Expression> parameterMap,
      Dictionary<string, Tuple<Type, string>> memberMap)
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
      Expression expression;
      if (!parameterMap.TryGetValue(node, out expression))
        expression = base.VisitParameter(node);

      return expression;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      // re-perform any member-binding
      var expression = Visit(node.Expression);
      if (expression.Type != node.Expression.Type)
      {
        // See if new type has public member with the same name
        MemberInfo memberInfo = expression.Type.GetMember(
          node.Member.Name,
          BindingFlags.Instance | BindingFlags.Public
        ).SingleOrDefault();

        if (memberInfo == null)
        {
          // See if new type is an interface, or implements an interface,
          // with a mapped member
          Tuple<Type, string> map;
          if (memberMap.TryGetValue(node.Member.Name, out map))
          {
            if (node.Expression.Type == map.Item1 || map.Item1.IsAssignableFrom(node.Expression.Type))
            {
              memberInfo = expression.Type.GetMember(
                map.Item2,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
              ).SingleOrDefault();
            }
          }
        }

        return Expression.MakeMemberAccess(expression, memberInfo);
      }
      return base.VisitMember(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
      if (node.NodeType == ExpressionType.TypeAs)
      {
        Expression expression = this.Visit(node.Operand);
        if (expression != node.Operand)
        {
          return expression;
        }
      }
      return node;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      var expression = Visit(node.Expression);
      if (expression.Type != node.Expression.Type)
      {
        HashSet<int> hashSet = null;
        if (node.TypeOperand == typeof(InternationalCustomer))
        {
          hashSet = new HashSet<int>() { 300 };
        }
        else if (node.TypeOperand == typeof(Zone))
        {
          hashSet = new HashSet<int>() { 200 };
        }
        else if (node.TypeOperand == typeof(Obj))
        {
          hashSet = new HashSet<int>() { 100, 200, 300 };
        }

        Expression typePropAccess = Expression.MakeMemberAccess(
          expression,
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
