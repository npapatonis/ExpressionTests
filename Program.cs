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
      var customers = CreateTestData();

      Expression<Func<Media, bool>> whereExp = m => (m.ParentId.Value.CatId == "Obj?id=a8e02957149444e185185bd3957a860b");
      //Expression<Func<Media, bool>> whereExp = m => (m.ParentId.Value.Id == "Obj?id=a8e02957149444e185185bd3957a860b");
      //Expression<Func<Obj, bool>> whereExp = o => (o.Id == "Obj?id=12532bb3c94d4efba557eafb36f88107");
      //Expression<Func<Obj, bool>> whereExp = o => (o is Person && (o as Person).Gender == Gender.Male);
      //Expression<Func<Obj, bool>> whereExp = (o => (o is Person && (o as Person).AltId0 == "6001"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as Zone).Name.StartsWith("Pod") || (o is Person && (o as Person).LastName.StartsWith("M")));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as IName).Name.StartsWith("Pod"));
      //Expression<Func<Zone, bool>> whereExp = (z => z.Name.StartsWith("Pod"));
      //Expression<Func<Obj, bool>> whereExp = (o => (o as ICategorizable).CatId.StartsWith("Zone.Out"));

      //Expression<Func<TblObj, bool>> e = (o => o.ParentId == new Guid("A8E02957-1494-44E1-8518-5BD3957A860B"));


      // rebuild the lambda
      var newExp = new ObjPredicateTranslator().Translate(whereExp);

      var results = customers.AsQueryable().Where(newExp);

      foreach (var result in results)
      {
        Console.WriteLine($"{result.Id} {result.Name0} {result.Desc0} {result.CatId} {result.Type} {result.SrvTime}");
      }

      Console.ReadLine();
    }

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
        // Inmates
        new TblObj { Id = new Guid("A8E02957-1494-44E1-8518-5BD3957A860B"), AltId0 = "15-0000", AltId1 = "11000", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:26.7939538"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "JAMES", Name1 = "LUKE", Name2 = "BRIAN", SrvTime = DateTime.Parse("2017-08-07 21:35:26.7939538"), Time = DateTime.Parse("2017-08-07 17:35:26.7939538 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("66CF5263-DF61-4344-9146-47C71FEF11B8"), AltId0 = "15-0022", AltId1 = "11022", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:28.1576166"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "TOWNSEND", Name1 = "ANDREW", Name2 = "SETH", SrvTime = DateTime.Parse("2017-08-07 21:35:28.1576166"), Time = DateTime.Parse("2017-08-07 17:35:28.1576166 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("59FD89BA-BE53-472B-A76D-4BF09E409749"), AltId0 = "15-0028", AltId1 = "11028", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:29.7842068"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "MCDANIEL", Name1 = "DAVID", Name2 = "COLLIN", SrvTime = DateTime.Parse("2017-08-07 21:35:29.7842068"), Time = DateTime.Parse("2017-08-07 17:35:29.7842068 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("B366E807-7C5B-4AF3-A0CD-53FEDABD97FD"), AltId0 = "15-0008", AltId1 = "11008", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:31.3366502"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "ANDERSON", Name1 = "JEREMIAH", Name2 = "ANTHONY", SrvTime = DateTime.Parse("2017-08-07 21:35:31.3366502"), Time = DateTime.Parse("2017-08-07 17:35:31.3366502 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("7657B2DD-8F9F-42B7-8C45-4CE9F173A4D7"), AltId0 = "15-0014", AltId1 = "11014", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:32.8629230"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "MATTHEWS", Name1 = "REGINALD", Name2 = "MITCHELL", SrvTime = DateTime.Parse("2017-08-07 21:35:32.8629230"), Time = DateTime.Parse("2017-08-07 17:35:32.8629230 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("4649B8EB-8BA3-49DA-893B-4FBFB1A5B1F8"), AltId0 = "15-0015", AltId1 = "11015", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:34.4540365"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "EASON", Name1 = "TAYLOR", Name2 = "RYAN", SrvTime = DateTime.Parse("2017-08-07 21:35:34.4540365"), Time = DateTime.Parse("2017-08-07 17:35:34.4540365 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("8F506777-46CE-48AD-9669-9B0C1B2F9B78"), AltId0 = "15-0007", AltId1 = "11007", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:36.0457096"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "MANN", Name1 = "TODD", Name2 = "ANTONIO", SrvTime = DateTime.Parse("2017-08-07 21:35:36.0457096"), Time = DateTime.Parse("2017-08-07 17:35:36.0457096 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("9A814E36-EC39-4648-BC5F-ECEE53470D44"), AltId0 = "15-0021", AltId1 = "11021", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:37.2976434"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "DANIELS", Name1 = "WALTER", Name2 = "ANTONIO", SrvTime = DateTime.Parse("2017-08-07 21:35:37.2976434"), Time = DateTime.Parse("2017-08-07 17:35:37.2976434 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("88908A10-9FAD-415C-A07B-0180111378AC"), AltId0 = "15-0042", AltId1 = "11042", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:38.3609119"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "CUMMINGS", Name1 = "JOSHUA", Name2 = "VICTOR", SrvTime = DateTime.Parse("2017-08-07 21:35:38.3609119"), Time = DateTime.Parse("2017-08-07 17:35:38.3609119 -04:00"), Type = 310 },
        new TblObj { Id = new Guid("F0469AC4-BBCB-49D8-BDA0-2AD0F1286548"), AltId0 = "15-0035", AltId1 = "11035", CatId = "Person.Resident", CreatedTime = DateTime.Parse("2017-08-07 21:35:38.8559448"), Desc0 = "", Enum0 = 1, ExId = "", Name0 = "SCALES", Name1 = "TONY", Name2 = "STEVEN", SrvTime = DateTime.Parse("2017-08-07 21:35:38.8559448"), Time = DateTime.Parse("2017-08-07 17:35:38.8559448 -04:00"), Type = 310 },
        // Media
        new TblObj { Id = new Guid("9A514E82-D483-49B8-A952-D0FD23B6A1C3"), ParentId = new Guid("A8E02957-1494-44E1-8518-5BD3957A860B"), CreatedTime = DateTime.Parse("2017-08-07 21:35:26.9214603"), SrvTime = DateTime.Parse("2017-08-07 21:35:26.9214603"), Time = DateTime.Parse("2017-08-07 17:35:26.9214603 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("10D60611-5117-4A46-9434-A78D8E1D8B82"), ParentId = new Guid("66CF5263-DF61-4344-9146-47C71FEF11B8"), CreatedTime = DateTime.Parse("2017-08-07 21:35:28.2414058"), SrvTime = DateTime.Parse("2017-08-07 21:35:28.2414058"), Time = DateTime.Parse("2017-08-07 17:35:28.2414058 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("D81C9BDA-022B-424C-A266-9C06C6F79581"), ParentId = new Guid("59FD89BA-BE53-472B-A76D-4BF09E409749"), CreatedTime = DateTime.Parse("2017-08-07 21:35:29.8705221"), SrvTime = DateTime.Parse("2017-08-07 21:35:29.8705221"), Time = DateTime.Parse("2017-08-07 17:35:29.8705221 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("D98627CF-4A0A-4D9B-BEA5-2840C7C156A7"), ParentId = new Guid("B366E807-7C5B-4AF3-A0CD-53FEDABD97FD"), CreatedTime = DateTime.Parse("2017-08-07 21:35:31.4749666"), SrvTime = DateTime.Parse("2017-08-07 21:35:31.4749666"), Time = DateTime.Parse("2017-08-07 17:35:31.4749666 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("C4C5EABC-2AAD-4D82-937C-BB2E1AF650E2"), ParentId = new Guid("7657B2DD-8F9F-42B7-8C45-4CE9F173A4D7"), CreatedTime = DateTime.Parse("2017-08-07 21:35:32.9331945"), SrvTime = DateTime.Parse("2017-08-07 21:35:32.9331945"), Time = DateTime.Parse("2017-08-07 17:35:32.9331945 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("600D291F-C71A-4810-BE16-4432CE7F716A"), ParentId = new Guid("4649B8EB-8BA3-49DA-893B-4FBFB1A5B1F8"), CreatedTime = DateTime.Parse("2017-08-07 21:35:34.6063425"), SrvTime = DateTime.Parse("2017-08-11 19:59:01.5868220"), Time = DateTime.Parse("2017-08-11 15:59:01.5868220 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("A8ED5A54-5B63-457E-86A6-9198604E5CC5"), ParentId = new Guid("8F506777-46CE-48AD-9669-9B0C1B2F9B78"), CreatedTime = DateTime.Parse("2017-08-07 21:35:36.1564850"), SrvTime = DateTime.Parse("2017-08-07 21:35:36.1564850"), Time = DateTime.Parse("2017-08-07 17:35:36.1564850 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("FC81A7EB-EC32-4325-A56A-346E05E0A952"), ParentId = new Guid("9A814E36-EC39-4648-BC5F-ECEE53470D44"), CreatedTime = DateTime.Parse("2017-08-07 21:35:37.3691088"), SrvTime = DateTime.Parse("2017-08-07 21:35:37.3691088"), Time = DateTime.Parse("2017-08-07 17:35:37.3691088 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("D86D8621-2BC2-4DD1-A803-56A48C455380"), ParentId = new Guid("88908A10-9FAD-415C-A07B-0180111378AC"), CreatedTime = DateTime.Parse("2017-08-07 21:35:38.4440386"), SrvTime = DateTime.Parse("2017-08-07 21:35:38.4440386"), Time = DateTime.Parse("2017-08-07 17:35:38.4440386 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
        new TblObj { Id = new Guid("EEA89B7C-4F82-4031-94C3-D60596C5BCEF"), ParentId = new Guid("F0469AC4-BBCB-49D8-BDA0-2AD0F1286548"), CreatedTime = DateTime.Parse("2017-08-07 21:35:38.9428967"), SrvTime = DateTime.Parse("2017-08-07 21:35:38.9428967"), Time = DateTime.Parse("2017-08-07 17:35:38.9428967 -04:00"), ParentType = 310, ParentCatId = "Person.Resident", CatId = "Media.PhotoId", Name0 = "Photo Id", Type = 400 },
      };
    }
  }
}
