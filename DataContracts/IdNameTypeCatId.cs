using System;

namespace G1T.Dc
{
  public struct IdNameTypeCatId : IEquatable<IdNameTypeCatId>
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="exId"></param>
    /// <param name="name"></param>
    /// <param name="catId"></param>
    /// <param name="typeName"></param>
    public IdNameTypeCatId(string id, string exId, string name, string typeName, string catId) :
      this()
    {
      Id = id;
      ExId = exId;
      Name = name;
      Type = typeName;
      CatId = catId;
    }

    public string Id { get; private set; }

    public string ExId { get; private set; }

    public string CatId { get; private set; }

    public string Name { get; private set; }

    public string Type { get; private set; }

    /// <summary>
    /// Returns true if equal.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IdNameTypeCatId other)
    {
      return other.Id == this.Id &&
             other.ExId == this.ExId &&
             other.Name == this.Name &&
             other.CatId == this.CatId &&
             other.Type == this.Type;
    }

    /// <summary>
    /// Returns true if equal.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if ((obj is IdNameTypeCatId))
      {
        return this.Equals((IdNameTypeCatId)obj);
      }
      return false;
    }

    /// <summary>
    /// Returns this object's hash code.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return (Id == null) ? 0 : Id.GetHashCode();
    }

    /// <summary>
    /// Returns the string representation of this object.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Format("{0} - {1} ({2})", Name, Id, Type);
    }

    /// <summary>
    /// Creates an IdAndType having the same Id an Type of this object.
    /// </summary>
    /// <returns></returns>
    public IdTypeCatId ToIdTypeCatId()
    {
      return new IdTypeCatId(Id, ExId, Type, CatId);
    }

    #region Operator Overloads

    /// <summary>
    /// Implements == operator.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(IdNameTypeCatId a, IdNameTypeCatId b)
    {
      return a.Equals(b);
    }

    /// <summary>
    /// Implements != operator.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(IdNameTypeCatId a, IdNameTypeCatId b)
    {
      return !a.Equals(b);
    }

    #endregion
  }
}
