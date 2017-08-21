using System;

namespace G1T.Dc
{
  public struct IdTypeCatId : IEquatable<IdTypeCatId>
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="exId"></param>
    /// <param name="typeName"></param>
    /// <param name="catId"></param>
    public IdTypeCatId(string id, string exId, string typeName, string catId) :
      this()
    {
      Id = id;
      ExId = exId;
      CatId = catId;
      Type = typeName;
    }

    public string Id { get; private set; }

    public string ExId { get; private set; }

    public string CatId { get; private set; }

    public string Type { get; private set; }

    /// <summary>
    /// Returns true if equal.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IdTypeCatId other)
    {
      return other.Id == this.Id &&
             other.ExId == this.ExId &&
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
      if ((obj is IdTypeCatId))
      {
        return this.Equals((IdTypeCatId)obj);
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
      return string.Format("{0} - {1} ({2})", CatId, Id, Type);
    }

    /// <summary>
    /// Creates an IdNameType having the same Id an Type of this object.
    /// </summary>
    /// <returns></returns>
    public IdNameTypeCatId ToIdNameTypeCatId()
    {
      return new IdNameTypeCatId(Id, ExId, null, Type, CatId);
    }

    /// <summary>
    /// Returns a IdTypeCatId with the specified CatId.
    /// </summary>
    /// <param name="catId"></param>
    /// <returns></returns>
    public static IdTypeCatId FromCatId(string catId)
    {
      return new IdTypeCatId(null, null, null, catId);
    }

    #region Operator Overloads

    /// <summary>
    /// Implements == operator.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(IdTypeCatId a, IdTypeCatId b)
    {
      return a.Equals(b);
    }

    /// <summary>
    /// Implements != operator.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(IdTypeCatId a, IdTypeCatId b)
    {
      return !a.Equals(b);
    }

    #endregion
  }
}
