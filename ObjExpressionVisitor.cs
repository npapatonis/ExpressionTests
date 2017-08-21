using G1T.Dc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTests
{
  public class ObjExpressionVisitor : EntityExpressionVisitor<TblObj>
  {
    #region =====[ ctor ]==========================================================================================

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
        { "AltId1", new MemberMapInfo[] { new MemberMapInfo(typeof(IAltId), "AltId1") } },
        { "ParentId.Value.Id", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), "ParentId") } },
        { "ParentId.Value.Type", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), "ParentType") } },
        { "ParentId.Value.CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), "ParentCatId") } }
      };
    }

    #endregion

    #region =====[ Protected Methods ]=============================================================================

    protected override bool IsIdMember(string memberName)
    {
      return memberName == nameof(TblObj.ParentId);
    }

    protected override bool IsTypeMember(string memberName)
    {
      return memberName == nameof(TblObj.ParentType);
    }

    #endregion
  }
}
