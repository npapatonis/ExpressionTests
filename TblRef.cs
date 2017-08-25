using G1T.Dc;
using System;

namespace ExpressionTests
{
  public class TblRef : IDbEntity
  {
    public int AutoId { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime SrvTime { get; set; }
    public DateTimeOffset Time { get; set; }
    public int Type { get; set; }
    public string V { get; set; }
    public string LExId { get; set; }
    public string RExId { get; set; }
    public Guid LId { get; set; }
    public int LType { get; set; }
    public string LCatId { get; set; }
    public Guid RId { get; set; }
    public int RType { get; set; }
    public string RCatId { get; set; }
  }
}
