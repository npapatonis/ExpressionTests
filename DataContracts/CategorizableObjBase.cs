namespace G1T.Dc
{
  public abstract class CategorizableObjBase : Obj, ICategorizable
  {
    public static readonly string TypeName = typeof(CategorizableObjBase).FullName;

    public string CatId { get; set; }
  }
}
