using System;

namespace ExpressionTests
{
  public interface IObj : IEntity
  {
    /// <summary>
    /// Unique Id identifying the object in an external system.
    /// </summary>
    string ExId { get; set; }

    /// <summary>
    /// Time when the Obj was last modified according to the client.
    /// </summary>
    DateTimeOffset Time { get; set; }
  }

  public abstract class Obj : EntityBase, IObj
  {
    public string ExId { get; set; }

    public DateTimeOffset Time { get; set; }

    public IdTypeCatId? ParentId { get; set; }
  }
}
