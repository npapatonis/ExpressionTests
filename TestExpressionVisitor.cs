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
    private readonly Dictionary<string, MemberMapInfo> memberMap;

    public TestExpressionVisitor(
      Dictionary<Expression, Expression> parameterMap,
      Dictionary<string, MemberMapInfo> memberMap)
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
          MemberMapInfo memberMapInfo;
          if (memberMap.TryGetValue(node.Member.Name, out memberMapInfo))
          {
            if (node.Expression.Type == memberMapInfo.SourceType || memberMapInfo.SourceType.IsAssignableFrom(node.Expression.Type))
            {
              memberInfo = expression.Type.GetMember(
                memberMapInfo.MappedMemberName,
                BindingFlags.Instance | BindingFlags.Public
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
      if (node.NodeType == ExpressionType.TypeAs || node.NodeType == ExpressionType.Convert)
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
        if (node.TypeOperand == typeof(Officer))
        {
          hashSet = new HashSet<int>() { 320 };
        }
        if (node.TypeOperand == typeof(Person))
        {
          hashSet = new HashSet<int>() { 300, 310, 320 };
        }
        else if (node.TypeOperand == typeof(Zone))
        {
          hashSet = new HashSet<int>() { 200 };
        }
        else if (node.TypeOperand == typeof(Obj))
        {
          hashSet = new HashSet<int>() { 100, 200, 300, 310, 320 };
        }

        Expression typePropAccess = Expression.MakeMemberAccess(
          expression,
          typeof(TblObj).GetProperty("Type", BindingFlags.Instance | BindingFlags.Public));

        return Expression.Call(
          Expression.Constant(hashSet),
          hashSet.GetType().GetMethod("Contains"),
          typePropAccess);
      }
      return base.VisitTypeBinary(node);
    }
  }
}
