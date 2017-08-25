using G1T.Dc;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTests
{
  public class RefExpressionVisitor : EntityExpressionVisitor<TblRef>
  {
    #region =====[ ctor ]==========================================================================================

    public RefExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
      : base(parameterMap)
    {
      MemberMap = new Dictionary<string, MemberMapInfo[]>()
      {
        { "LId.Id", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.LId)) } },
        { "LId.Type", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.LType)) } },
        { "LId.CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.LCatId)) } },
        { "LId.ExId", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.LExId)) } },

        { "RId.Id", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.RId)) } },
        { "RId.Type", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.RType)) } },
        { "RId.CatId", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.RCatId)) } },
        { "RId.ExId", new MemberMapInfo[] { new MemberMapInfo(typeof(IRef), nameof(TblRef.RExId)) } }
      };
    }

    #endregion

    #region =====[ Protected Methods ]=============================================================================

    protected override bool IsIdMember(string memberName)
    {
      return memberName == nameof(TblRef.LId) || memberName == nameof(TblRef.RId);
    }

    protected override bool IsTypeMember(string memberName)
    {
      return memberName == nameof(TblRef.LType) || memberName == nameof(TblRef.RType);
    }

    #endregion
  }
}
