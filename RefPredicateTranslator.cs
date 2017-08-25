using System.Collections.Generic;
using System.Linq.Expressions;
using G1T.Dc;

namespace ExpressionTests
{
  public class RefPredicateTranslator : PredicateTranslator<Ref, TblRef>
  {
    protected override EntityExpressionVisitor<TblRef> CreateExpressionVisitor(Dictionary<Expression, Expression> parameterMap)
    {
      return new RefExpressionVisitor(parameterMap);
    }
  }
}
