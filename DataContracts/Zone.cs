namespace G1T.Dc
{
  public class Zone : CategorizableObjBase, IZone
  {
    new public static readonly string TypeName = typeof(Zone).FullName;

    public string AltId0 { get; set; }

    public string AltId1 { get; set; }

    public string Desc { get; set; }

    public string Name { get; set; }
  }
}
