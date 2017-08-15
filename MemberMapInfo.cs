using System;

namespace ExpressionTests
{
  public class MemberMapInfo
  {
    public MemberMapInfo(Type sourceType, string mappedMemberName)
    {
      SourceType = sourceType;
      MappedMemberName = mappedMemberName;
    }

    public Type SourceType { get; set; }
    public string MappedMemberName { get; set; }
  }
}
