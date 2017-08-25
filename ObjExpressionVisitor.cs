using G1T.Dc;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        { "Name", new MemberMapInfo[] { new MemberMapInfo(typeof(IName), nameof(TblObj.Name0)) } },
        { "Desc", new MemberMapInfo[] { new MemberMapInfo(typeof(IDesc), nameof(TblObj.Desc0)) } },
        { "CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(ICategorizable), nameof(TblObj.CatId)) } },
        { "LastName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), nameof(TblObj.Name0)) } },
        { "FirstName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), nameof(TblObj.Name1)) } },
        { "MiddleName", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), nameof(TblObj.Name2)) } },
        { "Gender", new MemberMapInfo[] { new MemberMapInfo(typeof(IPerson), nameof(TblObj.Enum0)) } },
        { "AltId0", new MemberMapInfo[] { new MemberMapInfo(typeof(IAltId), nameof(TblObj.AltId0)) } },
        { "AltId1", new MemberMapInfo[] { new MemberMapInfo(typeof(IAltId), nameof(TblObj.AltId1)) } },
        { "ParentId.Value.Id", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), nameof(TblObj.ParentId)) } },
        { "ParentId.Value.Type", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), nameof(TblObj.ParentType)) } },
        { "ParentId.Value.CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), nameof(TblObj.ParentCatId)) } },
        { "ParentId.Value.ExId", new MemberMapInfo[] { new MemberMapInfo(typeof(IObj), nameof(TblObj.ParentExId)) } }
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
