using System;

namespace G1T.Dc
{
  public class Person : CategorizableObjBase, IPerson
  {
    public static readonly new string TypeName = typeof(Person).FullName;

    public string Account { get; set; }

    public string AltId0 { get; set; }

    public string AltId1 { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string FirstName { get; set; }

    public Gender Gender { get; set; }

    public string LastName { get; set; }

    public string MiddleName { get; set; }
  }
}
