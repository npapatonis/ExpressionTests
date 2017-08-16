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

      Expression<Func<Obj, bool>> whereExp = o => (o.Id == "");
      //Expression<Func<Obj, bool>> whereExp = o => (o is Person && (o as Person).Gender == Gender.Female);
      //Expression<Func<Obj, bool>> whereExp = (o => (o is Person && (o as Person).AltId0 == "6001"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as Zone).Name.StartsWith("Pod") || (o is Person && (o as Person).LastName.StartsWith("M")));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as IName).Name.StartsWith("Pod"));
      //Expression<Func<Zone, bool>> whereExp = (z => z.Name.StartsWith("Pod"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as ICategorizable).CatId.StartsWith("Zone.Out"));


      // rebuild the lambda
      var newExp = EntityExpressionVisitor.TransformExpression(whereExp);

      var results = customers.AsQueryable().Where(newExp as Func<TblObj, bool>);

      foreach (var result in results)
      {
        Console.WriteLine($"{result.Id} {result.Name0} {result.Desc0} {result.CatId} {result.Type} {result.SrvTime}");
      }

      Console.ReadLine();
    }

    private static Dictionary<string, MemberMapInfo[]> CreateMemberMap()
    {
      var memberMap = new Dictionary<string, MemberMapInfo[]>()
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

      return memberMap;
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
        new TblObj { Id = new Guid("DA888F1F-66FB-4167-8333-57AD8F1F61F1"), AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:04.9889168"), Desc0 = "Control", Enum0 = 0, ExId = "", Name0 = "Master Control", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:04.9889168"), Time = DateTime.Parse("2017-08-07 17:35:04.9889168 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("0AA00ABD-0AF8-4B9D-B2B2-AB14EEE54A2C"), AltId0 = "", AltId1 = "", CatId = "Zone.HousingUnit", CreatedTime = DateTime.Parse("2017-08-07 21:35:05.4445864"), Desc0 = "B1 - Men", Enum0 = 0, ExId = "", Name0 = "Pod B1", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:05.4445864"), Time = DateTime.Parse("2017-08-07 17:35:05.4445864 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("527481D1-2A39-4896-A7F5-D27741FBCCAB"), AltId0 = "", AltId1 = "", CatId = "Zone.HousingUnit", CreatedTime = DateTime.Parse("2017-08-07 21:35:05.9793136"), Desc0 = "B2 - Men", Enum0 = 0, ExId = "", Name0 = "Pod B2", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:05.9793136"), Time = DateTime.Parse("2017-08-07 17:35:05.9793136 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("06DE2295-32F6-421C-AD41-262053972B01"), AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:06.4760322"), Desc0 = "", Enum0 = 0, ExId = "", Name0 = "Kitchen", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:06.4760322"), Time = DateTime.Parse("2017-08-07 17:35:06.4760322 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("D5CF3988-FF4A-4DC3-A83B-FA13F15BFE0E"), AltId0 = "", AltId1 = "", CatId = "Zone.", CreatedTime = DateTime.Parse("2017-08-07 21:35:06.9937259"), Desc0 = "", Enum0 = 0, ExId = "", Name0 = "Visitation", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:06.9937259"), Time = DateTime.Parse("2017-08-07 17:35:06.9937259 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("741CD634-7B24-4F55-B4D6-791E678D928A"), AltId0 = "", AltId1 = "", CatId = "Zone.OutZone", CreatedTime = DateTime.Parse("2017-08-07 21:35:07.4944230"), Desc0 = "Out of facility", Enum0 = 0, ExId = "", Name0 = "Van", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:07.4944230"), Time = DateTime.Parse("2017-08-07 17:35:07.4944230 -04:00"), Type = 200 },
        new TblObj { Id = new Guid("AD399659-E578-4C10-9710-FB844E33FA8C"), AltId0 = "", AltId1 = "", CatId = "Zone.OutZone", CreatedTime = DateTime.Parse("2017-08-07 21:35:07.9941714"), Desc0 = "Out  of facility", Enum0 = 0, ExId = "", Name0 = "Courthouse", Name1 = "", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:07.9941714"), Time = DateTime.Parse("2017-08-07 17:35:07.9941714 -04:00"), Type = 200 },
        // Officers
        new TblObj { Id = new Guid("DE0C19C6-B1DF-4DBF-B920-3ACD84E87270"), AltId0 = "6001", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:39.9369303"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "McSorley", Name1 = "James", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:39.9369303"), Time = DateTime.Parse("2017-08-07 17:35:39.9369303 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("781AAAEE-1B89-4E81-B05F-0524EF0F0E18"), AltId0 = "6002", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:40.9174039"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Lopes", Name1 = "Missy", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:40.9174039"), Time = DateTime.Parse("2017-08-07 17:35:40.9174039 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("EB143E37-FADE-4BC1-BB9B-C453901BF236"), AltId0 = "6003", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:41.9621909"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Lippincott", Name1 = "Jannie", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:41.9621909"), Time = DateTime.Parse("2017-08-07 17:35:41.9621909 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("ADC1517D-1180-42B2-ABC0-8B55E65353A8"), AltId0 = "6004", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:43.0541236"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Goolsby", Name1 = "Robyn", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:43.0541236"), Time = DateTime.Parse("2017-08-07 17:35:43.0541236 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("12532BB3-C94D-4EFB-A557-EAFB36F88107"), AltId0 = "6005", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:44.0577327"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Fils", Name1 = "Shawnee", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:44.0577327"), Time = DateTime.Parse("2017-08-07 17:35:44.0577327 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("E8773BEA-CE82-4C31-A477-4FD520DB709B"), AltId0 = "6006", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:45.1401890"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "Blaser", Name1 = "Lyle", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:45.1401890"), Time = DateTime.Parse("2017-08-07 17:35:45.1401890 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("1DB45A5E-47BD-4C66-8044-C9270D1D8513"), AltId0 = "6007", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:46.1817763"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Kemper", Name1 = "Paulita", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:46.1817763"), Time = DateTime.Parse("2017-08-07 17:35:46.1817763 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("B41E3A09-4B08-4776-885A-DDD1760FA8C1"), AltId0 = "6008", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:47.2721309"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Mcneel", Name1 = "Karren", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:47.2721309"), Time = DateTime.Parse("2017-08-07 17:35:47.2721309 -04:00"), Type = 320 },
        new TblObj { Id = new Guid("1DE1C26E-CFD9-4D80-8D9D-B551C162ED25"), AltId0 = "6009", AltId1 = "", CatId = "Person.Staff", CreatedTime = DateTime.Parse("2017-08-07 21:35:48.2555232"), Desc0 = "", Enum0 = 2, ExId = "", Name0 = "Orlando", Name1 = "Jeanice", Name2 = "", SrvTime = DateTime.Parse("2017-08-07 21:35:48.2555232"), Time = DateTime.Parse("2017-08-07 17:35:48.2555232 -04:00"), Type = 320 },
      };
    }
  }
}
