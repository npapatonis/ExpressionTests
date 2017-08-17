using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public class ObjExpressionVisitor : EntityExpressionVisitor
  {
    public ObjExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
      : base(parameterMap)
    {
      MemberMap = new Dictionary<string, MemberMapInfo[]>()
      {
        { "Name", new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Name0") } },
        { "Desc", new MemberMapInfo[] { new MemberMapInfo(typeof(IDesc), "Desc0") } },
        { "CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(ICategorizable), "CatId") } },
        { "LastName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), "Name0") } },
        { "FirstName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), "Name1") } },
        { "MiddleName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), "Name2") } },
        { "Gender", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), "Enum0") } },
        { "AltId0", new MemberMapInfo[] { new MemberMapInfo(typeof(IAltId), "AltId0") } },
        { "AltId1", new MemberMapInfo[] { new MemberMapInfo(typeof(IAltId), "AltId1") } }
      };

      MemberTranslators = new Dictionary<string, Func<MemberExpression, Expression, MemberInfo>>()
      {
        { "Name", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Name0") }) },
        { "Desc", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Desc0") }) },
        { "CatId", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "CatId") }) },
        { "LastName", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Name0") }) },
        { "FirstName", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Name1") }) },
        { "MiddleName", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Name2") }) },
        { "Gender", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "Enum0") }) },
        { "AltId0", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "AltId0") }) },
        { "AltId1", (me, e) => MemberMapTranslate(me.Expression.Type, e.Type, new MemberMapInfo[] { new MemberMapInfo(typeof(IName), "AltId1") }) }
      };
    }
  }

  public abstract class EntityExpressionVisitor : ExpressionVisitor
  {
    private Dictionary<Expression, Expression> ParameterMap { get; set; }
    protected Dictionary<string, MemberMapInfo[]> MemberMap { get; set; }
    protected Dictionary<string, Func<MemberExpression, Expression, MemberInfo>> MemberTranslators { get; set; }

    protected MemberInfo MemberMapTranslate(Type sourceType, Type translatedType, MemberMapInfo[] memberMapInfos)
    {
      foreach (var memberMapInfo in memberMapInfos)
      {
        if (sourceType == memberMapInfo.SourceType || memberMapInfo.SourceType.IsAssignableFrom(sourceType))
        {
          var memberInfo = translatedType.GetMember(
            memberMapInfo.MappedMemberName,
            BindingFlags.Instance | BindingFlags.Public
          ).SingleOrDefault();
          if (memberInfo != null) return memberInfo;
        }
      }

      return null;
    }

    protected EntityExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
    {
      ParameterMap = parameterMap;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      Expression expression;
      if (!ParameterMap.TryGetValue(node, out expression))
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
          // with a list of mapping information
          MemberMapInfo[] memberMapInfos;
          if (MemberMap.TryGetValue(node.Member.Name, out memberMapInfos))
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
