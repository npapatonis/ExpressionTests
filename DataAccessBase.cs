using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTests
{
  public abstract class DataAccessBase
  {
    public static Guid GetDbId(string id)
    {
      string prefix;
      Guid guid;
      UriTemplateItemId.TryParseEntityId(id, out prefix, out guid);
      return guid;
    }

  }
}
