
using G1T.Dc;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionTests
{
  public class ObjPredicateTranslator : PredicateTranslator<Obj, TblObj>
  {
    protected override EntityExpressionVisitor<TblObj> CreateExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
    {
      return new ObjExpressionVisitor(parameterMap); ;
    }
  }
}
