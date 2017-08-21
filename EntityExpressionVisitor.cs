using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public abstract class EntityExpressionVisitor<TDbEntity> : ExpressionVisitor
    where TDbEntity : IDbEntity
  {
    #region =====[ ctor ]==========================================================================================

    protected EntityExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
    {
      ParameterMap = parameterMap;
    }

    #endregion

    #region =====[ Private Properties ]============================================================================

    private Dictionary<Expression, Expression> ParameterMap { get; set; }

    #endregion

    #region =====[ Protected Properties ]==========================================================================

    protected Dictionary<string, MemberMapInfo[]> MemberMap { get; set; }

    #endregion

    #region =====[ Protected Methods ]=============================================================================

    protected virtual bool IsIdMember(string memberName) { return false; }

    protected override Expression VisitBinary(BinaryExpression node)
    {
      var leftExpression = Visit(node.Left);

      if (leftExpression.NodeType == ExpressionType.MemberAccess)
      {
        var memberExpression = (leftExpression as MemberExpression);

        if ((memberExpression.Expression.NodeType == ExpressionType.Parameter) &&
            (memberExpression.Type == typeof(Guid) || memberExpression.Type == typeof(Guid?)) &&
            (memberExpression.Member.Name == nameof(EntityBase.Id) || IsIdMember(memberExpression.Member.Name)))
        {
          // This is a binary comparison of an Id property to some constant or variable.
          // Let VisitMember translate the left side (e.g., the o.Id).  Then create a new
          // expression for the right side that calls DataAccessBase.GetDbId with the string form
          // of the Id to convert it to a Guid.
          Expression rightExpression = Expression.Call(
            typeof(DataAccessBase).GetMethod(nameof(DataAccessBase.GetDbId)),
            node.Right);

          if (rightExpression.Type != leftExpression.Type)
          {
            rightExpression = Expression.Convert(rightExpression, leftExpression.Type);
          }

          return Expression.MakeBinary(node.NodeType, leftExpression, rightExpression);
        }
      }

      return base.VisitBinary(node);
    }

    private string m_fullMemberName = string.Empty;
    protected override Expression VisitMember(MemberExpression node)
    {
      if (node.Expression != null)
      {
        m_fullMemberName = node.Member.Name + (m_fullMemberName.Length > 0 ? "." : "") + m_fullMemberName;

        var expression = Visit(node.Expression);

        if (node.Expression.NodeType == ExpressionType.MemberAccess) return expression;

        if (expression.Type != node.Expression.Type)
        {
          MemberInfo memberInfo = null;

          // See if there is a member mapping
          MemberMapInfo[] memberMapInfos;
          if (MemberMap.TryGetValue(m_fullMemberName, out memberMapInfos))
          {
            foreach (var memberMapInfo in memberMapInfos)
            {
              if (node.Expression.Type == memberMapInfo.SourceType || memberMapInfo.SourceType.IsAssignableFrom(node.Expression.Type))
              {
                memberInfo = expression.Type.GetMember(
                  memberMapInfo.MappedMemberName,
                  BindingFlags.Instance | BindingFlags.Public
                ).SingleOrDefault();

                if (memberInfo != null) break;
              }
            }
          }
          m_fullMemberName = string.Empty;

          if (memberInfo == null)
          {
            // No mapping, see if new type has public member with the same name
            memberInfo = expression.Type.GetMember(
              node.Member.Name,
              BindingFlags.Instance | BindingFlags.Public
            ).SingleOrDefault();
          }

          return Expression.MakeMemberAccess(expression, memberInfo);
        }
      }

      return base.VisitMember(node);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      Expression expression;
      if (!ParameterMap.TryGetValue(node, out expression))
        expression = base.VisitParameter(node);

      return expression;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      var expression = Visit(node.Expression);
      if (expression.Type != node.Expression.Type)
      {
        HashSet<int> hashSet = null;
        if (node.TypeOperand == typeof(Person))
        {
          hashSet = new HashSet<int>() { 300, 310, 320 };
        }
        else if (node.TypeOperand == typeof(Inmate))
        {
          hashSet = new HashSet<int>() { 310 };
        }
        else if (node.TypeOperand == typeof(Officer))
        {
          hashSet = new HashSet<int>() { 320 };
        }
        else if (node.TypeOperand == typeof(Zone))
        {
          hashSet = new HashSet<int>() { 200 };
        }
        else if (node.TypeOperand == typeof(Media))
        {
          hashSet = new HashSet<int>() { 400 };
        }
        else if (node.TypeOperand == typeof(Obj))
        {
          hashSet = new HashSet<int>() { 100, 200, 300, 310, 320, 400 };
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

    #endregion
  }
}
