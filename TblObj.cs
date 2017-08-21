using System;

namespace ExpressionTests
{
  public class TblObj : IDbEntity
  {
    public Guid Id { get; set; }
    public string ExId { get; set; }
    public Guid? ParentId { get; set; }
    public int? ParentType { get; set; }
    public string ParentCatId { get; set; }
    public string ParentExId { get; set; }
    public string CatId { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime SrvTime { get; set; }
    public DateTimeOffset Time { get; set; }
    public int Type { get; set; }
    public string V { get; set; }
    public string Name0 { get; set; }
    public string Name1 { get; set; }
    public string Name2 { get; set; }
    public string AltId0 { get; set; }
    public string AltId1 { get; set; }
    public string Desc0 { get; set; }
    public int Enum0 { get; set; }
  }
}
