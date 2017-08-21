using System.Reflection;

namespace ExpressionTests
{
  public static class Utils
  {
    /// <summary>
    /// Gets an hash code for a Type.  If Type A and B are such that A.Equals(B), then 
    /// they will return the same hash code.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static int GetRepeatableTypeHashCode(TypeInfo typeInfo)
    {
      return GetRepeatableHashCode(typeInfo.FullName);
    }

    /// <summary>
    /// Gets a hash code for a string.  If strings A and B are such that A.Equals(B), then 
    /// they will return the same hash code.
    /// This hashcode implementation returns, for the most part, the same result as the .NET 4.0 64 bit implementation.
    /// </summary>
    /// <param name="txt"></param>
    /// <returns></returns>
    public static int GetRepeatableHashCode(string txt)
    {
      if (txt == null)
      {
        return 0;
      }

      int hash1 = 5381;
      int hash2 = hash1;

      int i = 0;
      while (i < txt.Length)
      {
        hash1 = ((hash1 << 5) + hash1) ^ (int)txt[i++];

        if (i >= txt.Length)
          break;
        hash2 = ((hash2 << 5) + hash2) ^ (int)txt[i++];
      }

      return hash1 + (hash2 * 1566083941);
    }
  }
}
