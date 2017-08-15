using System;

namespace ExpressionTests
{
  public interface IEntity : IIdentifiableObject, ICreatedAndModifiedTime
  {
  }

  public abstract class EntityBase : IEntity
  {
    public DateTime CreatedTime { get; set; }

    public string Id { get; set; }

    public DateTime SrvTime { get; set; }
  }
}
