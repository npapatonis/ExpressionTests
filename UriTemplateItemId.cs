using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTests
{
  /// <summary>
  /// This object is used to manipulate a URI in the for of "{UriRoutePrefix}/?id={idSuffix}".
  /// E.g.: "Zones/?id=Kitchen". Where {UriRoutePrefix} = "Zones" and {idSuffix} = "Kitchen".
  /// </summary>
  public static class UriTemplateItemId
  {
    private const string GUID_STRFORMAT = "N";
    private const string QS_Id = "?id=";

    /// <summary>
    /// Creates an entity Uri, e.g.: http://41r8sw1.ntks.ezbarcode.com/Tks.G1Track.Server/Obj?id=ceba682b76634a2baccabada08d95da7
    /// </summary>
    /// <param name="serverBaseAddress"></param>
    /// <param name="entityId">E.g.: Obj?id=ceba682b76634a2baccabada08d95da7</param>
    /// <returns></returns>
    public static Uri CreateEntityUri(Uri serverBaseAddress, string entityId)
    {
      string endpointUriTxt = serverBaseAddress.AbsoluteUri + "/" + entityId;
      return new Uri(endpointUriTxt);
    }

    /// <summary>
    /// Creates an entity Id, e.g.: Obj?id=19eb90709f634458a1b6668f3e4342f7
    /// </summary>
    /// <param name="uriSegmentPrefix">E.g.: Obj</param>
    /// <param name="guidId"></param>
    /// <returns></returns>
    public static string CreateEntityId(string uriSegmentPrefix, Guid guidId)
    {
      return uriSegmentPrefix + QS_Id + guidId.ToString(GUID_STRFORMAT);
    }

    /// <summary>
    /// Creates an entity Id, e.g.: EvtAlm?id=streamPos
    /// </summary>
    /// <param name="uriSegmentPrefix">E.g.: Obj</param>
    /// <param name="stringId"></param>
    /// <returns></returns>
    public static string CreateEntityId(string uriSegmentPrefix, string stringId)
    {
      return uriSegmentPrefix + QS_Id + Uri.EscapeDataString(stringId);
    }

    /// <summary>
    /// Creates an entity Id, e.g.: EvtAlm?id=3631
    /// </summary>
    /// <param name="uriSegmentPrefix"></param>
    /// <param name="intId"></param>
    /// <returns></returns>
    public static string CreateEntityId(string uriSegmentPrefix, long intId)
    {
      var evtId = uriSegmentPrefix + QS_Id + intId.ToString();
      return evtId;
    }

    /// <summary>
    /// Parse Entity Guid Id.
    /// </summary>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public static Guid ParseEntityGuidId(string entityId)
    {
      string uriSegmentPrefix;
      Guid guidId;
      if (!TryParseEntityId(entityId, out uriSegmentPrefix, out guidId))
      {
        throw new ArgumentOutOfRangeException(nameof(entityId));
      }
      return guidId;
    }

    /// <summary>
    /// Parses Entity Id.
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="uriSegmentPrefix"></param>
    /// <param name="guidId"></param>
    /// <returns>Returns true if successful.</returns>
    public static bool TryParseEntityId(
      string entityId,
      out string uriSegmentPrefix,
      out Guid guidId)
    {
      string stringId;
      if (TryParseEntityId(entityId, out uriSegmentPrefix, out stringId))
      {
        return Guid.TryParseExact(stringId, GUID_STRFORMAT, out guidId);
      }
      guidId = Guid.Empty;
      return false;
    }

    /// <summary>
    /// Parses Entity Id.
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="uriSegmentPrefix"></param>
    /// <param name="stringId"></param>
    /// <returns>Returns true if successful.</returns>
    public static bool TryParseEntityId(
      string entityId,
      out string uriSegmentPrefix,
      out string stringId)
    {
      if (entityId == null)
      {
        throw new ArgumentNullException(nameof(entityId));
      }

      if (entityId != null)
      {
        int p1 = entityId.IndexOf(QS_Id);
        if (p1 > 0)
        {
          stringId = entityId.Substring(p1 + QS_Id.Length);
          int p2 = stringId.IndexOf('&');
          if (p2 > -1)
          {
            stringId = stringId.Substring(0, p2);
          }
          stringId = Uri.UnescapeDataString(stringId);
          uriSegmentPrefix = entityId.Substring(0, p1);
          return true;
        }
      }
      stringId = null;
      uriSegmentPrefix = null;
      return false;
    }

    /// <summary>
    /// PropId UriRoutePrefix .
    /// </summary>
    public const string PropId_UriRoutePrefix = "PropId";

    /// <summary>
    /// Creates a PropId.
    /// </summary>
    /// <param name="propertyId"></param>
    /// <returns></returns>
    public static string CreatePropId(Guid propertyId)
    {
      return CreateEntityId(PropId_UriRoutePrefix, propertyId);
    }
  }
}
