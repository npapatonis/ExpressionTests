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
    }
  }

  public abstract class EntityExpressionVisitor : ExpressionVisitor
  {
    private Dictionary<Expression, Expression> ParameterMap { get; set; }
    protected Dictionary<string, MemberMapInfo[]> MemberMap { get; set; }

    protected EntityExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
    {
      ParameterMap = parameterMap;
    }

    public static LambdaExpression TransformExpression(LambdaExpression expression)
    {
      Dictionary<Expression, Expression> parameterMap;
      ParameterExpression[] newParams;
      CreateParameterMap(expression, out parameterMap, out newParams);

      var visitor = new ObjExpressionVisitor(parameterMap);
      var body = visitor.Visit(expression.Body);

      return Expression.Lambda(body, newParams);
    }

    private static void CreateParameterMap(LambdaExpression expression,
      out Dictionary<Expression, Expression> parameterMap,
      out ParameterExpression[] newParams)
    {
      parameterMap = new Dictionary<Expression, Expression>();
      newParams = new ParameterExpression[expression.Parameters.Count];
      for (int i = 0; i < newParams.Length; i++)
      {
        if (typeof(Obj).IsAssignableFrom(expression.Parameters[i].Type))
        {
          parameterMap[expression.Parameters[i]] = newParams[i] =
            Expression.Parameter(typeof(TblObj), expression.Parameters[i].Name);
        }
        //else if (typeof(Ref).IsAssignableFrom(expression.Parameters[i].Type))
        //{
        //  parameterMap[expression.Parameters[i]] = newParams[i] =
        //    Expression.Parameter(typeof(TblRef), expression.Parameters[i].Name);
        //}
        else
        {
          newParams[i] = expression.Parameters[i];
        }
      }
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
