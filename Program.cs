using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTests
{
  class Program
  {
    static void Main(string[] args)
    {
      Test03();
    }

    private static void Test03()
    {
      var customers = CreateTestData();

      Expression<Func<Obj, bool>> whereExp = o => (o is Person && (o as Person).Gender == Gender.Female);
      //Expression<Func<Obj, bool>> whereExp = (o => (o is Person && (o as Person).AltId0 == "6001"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as Zone).Name.StartsWith("Pod") || (o is Person && (o as Person).LastName.StartsWith("M")));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as IName).Name.StartsWith("Pod"));
      //Expression<Func<Zone, bool>> whereExp = (z => z.Name.StartsWith("Pod"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as ICategorizable).CatId.StartsWith("Zone.Out"));

      Dictionary<Expression, Expression> parameterMap;
      ParameterExpression[] newParams;
      CreateParameterMap(whereExp, out parameterMap, out newParams);
      var memberMap = CreateMemberMap();

      // rebuild the lambda
      var body = new TestExpressionVisitor(parameterMap, memberMap).Visit(whereExp.Body);
      var newExp = Expression.Lambda<Func<TblObj, bool>>(body, newParams);

      var results = customers.AsQueryable().Where(newExp);

      foreach (var result in results)
      {
        Console.WriteLine($"{result.Id} {result.Name0} {result.Desc0} {result.CatId} {result.Type} {result.SrvTime}");
      }

      Console.ReadLine();
    }

    private static Dictionary<string, MemberMapInfo> CreateMemberMap()
    {
      var memberMap = new Dictionary<string, MemberMapInfo>()
      {
        { "Name", new MemberMapInfo(typeof(IName), "Name0") },
        { "Desc", new MemberMapInfo(typeof(IDesc), "Desc0") },
        { "CatId", new MemberMapInfo(typeof(ICategorizable), "CatId") },
        { "LastName", new MemberMapInfo(typeof(IPerson), "Name0") },
        { "FirstName", new MemberMapInfo(typeof(IPerson), "Name1") },
        { "MiddleName", new MemberMapInfo(typeof(IPerson), "Name2") },
        { "Gender", new MemberMapInfo(typeof(IPerson), "Enum0") },
      };

      return memberMap;
    }

    private static void CreateParameterMap(LambdaExpression expression,
      out Dictionary<Expression, Expression> parameterMap,
      out ParameterExpression[] newParams)
    {
      var sourceTypes = expression.Type.GetGenericArguments();

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

    //private static void CreateParameterMap<TFrom, TTo>(
    //  Expression<Func<TFrom, bool>> whereExp,
    //  out Dictionary<Expression, Expression> parameterMap,
    //  out ParameterExpression[] newParams)
    //{
    //  // figure out which types are different in the function-signature
    //  var fromTypes = whereExp.Type.GetGenericArguments();
    //  var toTypes = typeof(Func<TTo, bool>).GetGenericArguments();

    //  if (fromTypes.Length != toTypes.Length)
    //    throw new NotSupportedException("Incompatible lambda function-type signatures");

    //  Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();
    //  for (int i = 0; i < fromTypes.Length; i++)
    //  {
    //    if (fromTypes[i] != toTypes[i])
    //      typeMap[fromTypes[i]] = toTypes[i];
    //  }

    //  // re-map all parameters that involve different types
    //  parameterMap = new Dictionary<Expression, Expression>();
    //  newParams = new ParameterExpression[whereExp.Parameters.Count];
    //  for (int i = 0; i < newParams.Length; i++)
    //  {
    //    Type newType;
    //    if (typeMap.TryGetValue(whereExp.Parameters[i].Type, out newType))
    //    {
    //      parameterMap[whereExp.Parameters[i]] = newParams[i] =
    //          Expression.Parameter(newType, whereExp.Parameters[i].Name);
    //    }
    //    else
    //    {
    //      newParams[i] = whereExp.Parameters[i];
    //    }
    //  }
    //}

    private static List<TblObj> CreateTestData()
    {
      return new List<TblObj>
      {
        // Zones
        new TblObj { Id = 1, AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:04.9889168"), Desc0 = "Control", Enum0 = 0, ExId = "", Name0 = "Master Control", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:04.9889168"), Time = DateTime.Parse("2017-08-07 17:35:04.9889168 -04:00"), Type = 200 },
        new TblObj { Id = 2, AltId0 = "", AltId1 = "", CatId = "Zone.HousingUnit", CreatedTime = DateTime.Parse("2017-08-07 21:35:05.4445864"), Desc0 = "B1 - Men", Enum0 = 0, ExId = "", Name0 = "Pod B1", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:05.4445864"), Time = DateTime.Parse("2017-08-07 17:35:05.4445864 -04:00"), Type = 200 },
        new TblObj { Id = 3, AltId0 = "", AltId1 = "", CatId = "Zone.HousingUnit", CreatedTime = DateTime.Parse("2017-08-07 21:35:05.9793136"), Desc0 = "B2 - Men", Enum0 = 0, ExId = "", Name0 = "Pod B2", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:05.9793136"), Time = DateTime.Parse("2017-08-07 17:35:05.9793136 -04:00"), Type = 200 },
        new TblObj { Id = 4, AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:06.4760322"), Desc0 = "", Enum0 = 0, ExId = "", Name0 = "Kitchen", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:06.4760322"), Time = DateTime.Parse("2017-08-07 17:35:06.4760322 -04:00"), Type = 200 },
        new TblObj { Id = 5, AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:06.9937259"), Desc0 = "", Enum0 = 0, ExId = "", Name0 = "Visitation", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:06.9937259"), Time = DateTime.Parse("2017-08-07 17:35:06.9937259 -04:00"), Type = 200 },
        new TblObj { Id = 6, AltId0 = "", AltId1 = "", CatId = "Zone.OutZone", CreatedTime = DateTime.Parse("2017-08-07 21:35:07.4944230"), Desc0 = "Out of facility", Enum0 = 0, ExId = "", Name0 = "Van", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:07.4944230"), Time = DateTime.Parse("2017-08-07 17:35:07.4944230 -04:00"), Type = 200 },
        new TblObj { Id = 7, AltId0 = "", AltId1 = "", CatId = "Zone.OutZone", CreatedTime = DateTime.Parse("2017-08-07 21:35:07.9941714"), Desc0 = "Out  of facility", Enum0 = 0, ExId = "", Name0 = "Courthouse", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:07.9941714"), Time = DateTime.Parse("2017-08-07 17:35:07.9941714 -04:00"), Type = 200 },
        // Officers
        new TblObj { Id = 8, AltId0 = "6001", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:39.9369303"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "McSorley", Name1 = "James", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:39.9369303"), Time = DateTime.Parse("2017-08-07 17:35:39.9369303 -04:00"), Type = 320 },
        new TblObj { Id = 9, AltId0 = "6002", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:40.9174039"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Lopes", Name1 = "Missy", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:40.9174039"), Time = DateTime.Parse("2017-08-07 17:35:40.9174039 -04:00"), Type = 320 },
        new TblObj { Id = 10, AltId0 = "6003", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:41.9621909"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Lippincott", Name1 = "Jannie", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:41.9621909"), Time = DateTime.Parse("2017-08-07 17:35:41.9621909 -04:00"), Type = 320 },
        new TblObj { Id = 11, AltId0 = "6004", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:43.0541236"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Goolsby", Name1 = "Robyn", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:43.0541236"), Time = DateTime.Parse("2017-08-07 17:35:43.0541236 -04:00"), Type = 320 },
        new TblObj { Id = 12, AltId0 = "6005", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:44.0577327"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Fils", Name1 = "Shawnee", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:44.0577327"), Time = DateTime.Parse("2017-08-07 17:35:44.0577327 -04:00"), Type = 320 },
        new TblObj { Id = 13, AltId0 = "6006", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:45.1401890"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "Blaser", Name1 = "Lyle", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:45.1401890"), Time = DateTime.Parse("2017-08-07 17:35:45.1401890 -04:00"), Type = 320 },
        new TblObj { Id = 14, AltId0 = "6007", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:46.1817763"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Kemper", Name1 = "Paulita", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:46.1817763"), Time = DateTime.Parse("2017-08-07 17:35:46.1817763 -04:00"), Type = 320 },
        new TblObj { Id = 15, AltId0 = "6008", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:47.2721309"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Mcneel", Name1 = "Karren", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:47.2721309"), Time = DateTime.Parse("2017-08-07 17:35:47.2721309 -04:00"), Type = 320 },
        new TblObj { Id = 16, AltId0 = "6009", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:48.2555232"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Orlando", Name1 = "Jeanice", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:48.2555232"), Time = DateTime.Parse("2017-08-07 17:35:48.2555232 -04:00"), Type = 320 },
      };
    }
  }
}
