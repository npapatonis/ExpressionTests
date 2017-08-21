using G1T.Dc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ExpressionTests
{
  internal class DcTypeBase : IModelTypeInfo
  {
    #region =====[ Private Fields ]================================================================================

    private readonly List<int> _NotAbstractTypeDbIds = new List<int>();
    private readonly int _TypeDbId;
    private int? _BaseTypeDbId;

    #endregion

    #region =====[ ctor ]==========================================================================================

    protected DcTypeBase(TypeInfo typeInfo)
    {
      TypeInfo = typeInfo;
      _TypeDbId = Utils.GetRepeatableTypeHashCode(typeInfo);
    }

    static DcTypeBase()
    {
      Assembly assembly = typeof(EntityBase).GetTypeInfo().Assembly;
      foreach (var t in assembly.DefinedTypes)
      {
        if (typeof(EntityBase).IsAssignableFrom(t.AsType()))
        {
          var modelType = new ModelType(t);
          DictionaryOfModelTypesKeyedOnTypeName.Add(modelType.TypeInfo.FullName, modelType);
          DictionaryOfModelTypesKeyedOnTypeDbId.Add(modelType.TypeDbId, modelType);
        }
      }

      var allModelTypes = DictionaryOfModelTypesKeyedOnTypeName.Values;
      foreach (var modelType in allModelTypes)
      {
        modelType._NotAbstractTypeDbIds.AddRange(allModelTypes.Where(t => !t.TypeInfo.IsAbstract && modelType.TypeInfo.IsAssignableFrom(t.TypeInfo)).Select(t => t.TypeDbId));

        var baseType = modelType.TypeInfo.BaseType;

        while (baseType != null)
        {
          ModelType baseModelType;
          if (baseType.FullName != null &&
              DictionaryOfModelTypesKeyedOnTypeName.TryGetValue(baseType.FullName, out baseModelType))
          {
            modelType._BaseTypeDbId = baseModelType.TypeDbId;
            break;
          }
          baseType = baseType.GetTypeInfo().BaseType;
        }
      }
    }

    #endregion

    #region =====[ Protected Properties ]==========================================================================

    protected static readonly Dictionary<string, ModelType> DictionaryOfModelTypesKeyedOnTypeName = new Dictionary<string, ModelType>();
    protected static readonly Dictionary<int, ModelType> DictionaryOfModelTypesKeyedOnTypeDbId = new Dictionary<int, ModelType>();

    #endregion

    #region =====[ Public Properties ]=============================================================================

    public static List<ModelType> AllModelTypes => DictionaryOfModelTypesKeyedOnTypeName.Values.ToList();
    public TypeInfo TypeInfo { get; }
    public string TypeName => TypeInfo.FullName;
    public int TypeDbId => _TypeDbId;
    public int? BaseTypeDbId => _BaseTypeDbId;
    public IReadOnlyList<int> NotAbstractTypeDbIds => _NotAbstractTypeDbIds;

    #endregion

    #region =====[ Public Methods ]================================================================================

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture,
          "[{0}] {1}, DbId = {2}",
          TypeInfo.FullName,
          TypeInfo.IsAbstract ? "Abstract" : "Concrete",
          TypeDbId);
    }

    #endregion
  }
}
