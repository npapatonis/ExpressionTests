using System;

namespace G1T.Dc
{
  public abstract class Ref : EntityBase, IRef
  {
    public IdTypeCatId LId { get; set; }

    public IdTypeCatId RId { get; set; }

    public DateTimeOffset Time { get; set; }
  }
}
