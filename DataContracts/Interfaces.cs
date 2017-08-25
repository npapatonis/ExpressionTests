using System;
using System.Collections.Generic;
using System.Reflection;

namespace G1T.Dc
{
  public interface ISrvTime
  {
    DateTime SrvTime { get; set; }
  }

  public interface ICreatedTime
  {
    DateTime CreatedTime { get; set; }
  }

  public interface ICreatedAndModifiedTime : ISrvTime, ICreatedTime
  {
  }

  public interface IIdentifiableObject : ISrvTime
  {
    string Id { get; set; }
  }

  public interface IName
  {
    string Name { get; }
  }

  public interface IDesc
  {
    string Desc { get; set; }
  }

  public interface ICategorizable
  {
    string CatId { get; set; }
  }

  public interface IAltId
  {
    string AltId0 { get; set; }
    string AltId1 { get; set; }
  }

  public interface IZone : IName, IDesc, ICategorizable, IAltId
  {
  }

  public enum Gender
  {
    Undefined = 0,
    Male = 1,
    Female = 2
  }

  public interface IPerson : ICategorizable, IAltId, IIdentifiableObject
  {
    string FirstName { get; set; }
    string LastName { get; set; }
    string MiddleName { get; set; }
    DateTime? DateOfBirth { get; set; }
    Gender Gender { get; set; }
    string Account { get; set; }
  }

  public interface IMedia : IName, IDesc, ICategorizable, IIdentifiableObject
  {
    string ClientTag { get; set; }
  }

  public interface IDbEntity : IDbEntry
  {
    int Type { get; }
  }

  public interface IDbEntry : IDbId<Guid>
  {
  }

  public interface IDbId<TId>
  {
    TId Id { get; }
  }

  public interface IModelTypeInfo
  {
    TypeInfo TypeInfo { get; }

    int TypeDbId { get; }

    IReadOnlyList<int> NotAbstractTypeDbIds { get; }
  }

  public interface IRef : IEntity
  {
    DateTimeOffset Time { get; set; }

    IdTypeCatId LId { get; set; }

    IdTypeCatId RId { get; set; }
  }
}
