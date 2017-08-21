using System;

namespace G1T.Dc
{
  public class Media : CategorizableObjBase, IMedia
  {
    public string Name { get; set; }

    public string Desc { get; set; }

    public DateTime MediaSrvTime { get; set; }

    public string ClientTag { get; set; }
  }
}
