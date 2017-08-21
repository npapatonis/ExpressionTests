using G1T.Dc;
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
    protected virtual bool IsTypeMember(string memberName) { return false; }

    protected override Expression VisitBinary(BinaryExpression node)
    {
      var leftExpression = Visit(node.Left);

      if (leftExpression.NodeType == ExpressionType.MemberAccess)
      {
        var memberExpression = (leftExpression as MemberExpression);

        if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
        {
          if ((memberExpression.Type == typeof(Guid) || memberExpression.Type == typeof(Guid?)) &&
              (memberExpression.Member.Name == nameof(EntityBase.Id) || IsIdMember(memberExpression.Member.Name)))
          {
            // This is a binary comparison of an Id property to some constant or variable.
            // Create a new expression for the right side that calls DataAccessBase.GetDbId
            // with the string form of the Id to convert it to a Guid.
            Expression rightExpression = Expression.Call(
              typeof(DataAccessBase).GetMethod(nameof(DataAccessBase.GetDbId)),
              node.Right);

            if (rightExpression.Type != leftExpression.Type)
            {
              rightExpression = Expression.Convert(rightExpression, leftExpression.Type);
            }

            return Expression.MakeBinary(node.NodeType, leftExpression, rightExpression);
          }
          else if ((memberExpression.Type == typeof(int) || memberExpression.Type == typeof(int?)) &&
              (IsTypeMember(memberExpression.Member.Name)))
          {
            // This is a binary comparison of a Type property to some constant or variable.
            // Create a new expression for the right side that calls HashSet.Contains.
            HashSet<int> typeDbIds;
            ModelType.TryGetNotAbstractIncludingDerivedTypeDbIds(new string[] { (node.Right as ConstantExpression).Value.ToString() }, out typeDbIds);

            //Expression typePropAccess = Expression.MakeMemberAccess(
            //  leftExpression,
            //  typeof(TDbEntity).GetProperty(nameof(IDbEntity.Type), BindingFlags.Instance | BindingFlags.Public));

            Expression typePropAccess = memberExpression;
            if (typePropAccess.Type == typeof(int?))
            {
              var x = Expression.MakeBinary(ExpressionType.NotEqual, memberExpression, Expression.Constant(null));
              var y = Expression.Call(
                Expression.Constant(typeDbIds),
                typeDbIds.GetType().GetMethod(nameof(HashSet<int>.Contains)),
                Expression.Convert(typePropAccess, typeof(int)));

              var z = Expression.AndAlso(x, y);
              return z;
            }

            return Expression.Call(
              Expression.Constant(typeDbIds),
              typeDbIds.GetType().GetMethod(nameof(HashSet<int>.Contains)),
              typePropAccess);
          }
        }
      }

      return base.VisitBinary(node);
    }

    private Stack<string> m_memberNames = new Stack<string>();
    protected override Expression VisitMember(MemberExpression node)
    {
      if (node.Expression != null)
      {
        m_memberNames.Push(node.Member.Name);

        var expression = Visit(node.Expression);

        if (node.Expression.NodeType == ExpressionType.MemberAccess) return expression;

        if (expression.Type != node.Expression.Type)
        {
          MemberInfo memberInfo = null;
          var fullMemberName = string.Join(".", m_memberNames);

          // See if there is a member mapping
          MemberMapInfo[] memberMapInfos;
          if (MemberMap.TryGetValue(fullMemberName, out memberMapInfos))
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
          m_memberNames.Clear();

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
        HashSet<int> typeDbIds;
        ModelType.TryGetNotAbstractIncludingDerivedTypeDbIds(new string[] { node.TypeOperand.FullName }, out typeDbIds);

        Expression typePropAccess = Expression.MakeMemberAccess(
          expression,
          typeof(TDbEntity).GetProperty(nameof(IDbEntity.Type), BindingFlags.Instance | BindingFlags.Public));

        return Expression.Call(
          Expression.Constant(typeDbIds),
          typeDbIds.GetType().GetMethod(nameof(HashSet<int>.Contains)),
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
