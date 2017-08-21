using G1T.Dc;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressionTests
{
  internal class ModelType : DcTypeBase
  {
    #region =====[ ctor ]==========================================================================================

    internal ModelType(TypeInfo typeInfo)
      : base(typeInfo)
    {
      IsObj = typeof(Obj).IsAssignableFrom(typeInfo.AsType());
      //IsRef = typeof(Ref).IsAssignableFrom(typeInfo.AsType());
      IsRef = false;
    }

    #endregion

    #region =====[ Public Properties ]=============================================================================

    public readonly bool IsObj;
    public readonly bool IsRef;

    #endregion

    #region =====[ Public Methods ]================================================================================

    public static bool TryGetNotAbstractIncludingDerivedTypeDbIds(
      IEnumerable<string> modelTypeNames,
      out HashSet<int> concreteTypeNames)
    {
      concreteTypeNames = new HashSet<int>();
      foreach (var typeName in modelTypeNames)
      {
        ModelType modelType;
        if (!DictionaryOfModelTypesKeyedOnTypeName.TryGetValue(typeName, out modelType))
        {
          concreteTypeNames = null;
          return false;
        }
        concreteTypeNames.UnionWith(modelType.NotAbstractTypeDbIds);
      }
      return true;
    }

    public static bool TryGetModelType(string typeName, out ModelType modelType)
    {
      return DictionaryOfModelTypesKeyedOnTypeName.TryGetValue(typeName, out modelType);
    }

    public static bool TryGetModelType(int typeDbId, out ModelType modelType)
    {
      return DictionaryOfModelTypesKeyedOnTypeDbId.TryGetValue(typeDbId, out modelType);
    }

    public static ModelType GetModelType(string type)
    {
      ModelType modelType;

      if (!TryGetModelType(type, out modelType))
      {
        throw new ArgumentOutOfRangeException(nameof(type));
      };
      return modelType;
    }

    public static ModelType GetModelType(int typeDbId)
    {
      ModelType modelType;

      if (!TryGetModelType(typeDbId, out modelType))
      {
        throw new ArgumentOutOfRangeException(nameof(typeDbId));
      };
      return modelType;
    }

    #endregion
  }
}
