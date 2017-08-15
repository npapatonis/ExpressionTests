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
        // Try to get a public member by the same name
        MemberInfo memberInfo = expression.Type.GetMember(
          node.Member.Name,
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        ).SingleOrDefault();

        if (memberInfo == null)
        {
          // Try to get a non-public member by the same name
          memberInfo = expression.Type.GetMember(
            node.Member.Name,
            BindingFlags.Instance | BindingFlags.NonPublic
          ).SingleOrDefault();
        }

        Func<string, string, MemberInfo> lookupMemberInfo = (sourceTypeName, sourceMemberName) =>
        {
          Dictionary<string, string> map;
          if (memberMap.TryGetValue(sourceTypeName, out map))
          {
            string memberName;
            if (map.TryGetValue(sourceMemberName, out memberName))
            {
              return expression.Type.GetMember(
                memberName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
              ).SingleOrDefault();
            }
          }

          return null;
        };

        if (memberInfo == null)
        {
          memberInfo = lookupMemberInfo(node.Expression.Type.Name, node.Member.Name);
        }

        if (memberInfo == null)
        {
          if (node.Expression.Type.GetInterface(nameof(IName)) != null)
          {
            memberInfo = lookupMemberInfo(nameof(IName), node.Member.Name);
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
