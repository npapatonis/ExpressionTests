using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTests
{
  class Program
  {
    static void Main(string[] args)
    {
      Test03();
    }

    private static void Test03()
    {
      var customers = new List<TblCustomer>
      {
        new TblCustomer { Id = 1, Type = 4000, Name0 = "International ABC", Preferred = false, Discount = 5m, AnnualSales = 10000m, AltId = "IMF 101" },
        new TblCustomer { Id = 2, Type = 3000, Name0 = "Domestic ABC", Preferred = true, Discount = 15m, AnnualSales = 25000m, AltId = "Fed 101" },
        new TblCustomer { Id = 3, Type = 3000, Name0 = "Domestic DEF", Preferred = false, Discount = 5m, AnnualSales = 5000m, AltId = "Fed 102" },
        new TblCustomer { Id = 4, Type = 3000, Name0 = "Domestic XYZ", Preferred = false, Discount = 7.5m, AnnualSales = 8000m, AltId = "Fed 203" },
        new TblCustomer { Id = 5, Type = 4000, Name0 = "International DEF", Preferred = true, Discount = 10m, AnnualSales = 23000m, AltId = "IMF 102" },
        new TblCustomer { Id = 6, Type = 4000, Name0 = "International XYZ", Preferred = false, Discount = 7.5m, AnnualSales = 8000m, AltId = "IMF 202" },
      };

      //Expression<Func<Customer, bool>> whereExp = (c => c is DomesticCustomer && (c as DomesticCustomer).FedId == "Fed 101");
      //Expression<Func<Customer, bool>> whereExp = (c => (c as DomesticCustomer).FedId == "Fed 101");
      Expression<Func<DomesticCustomer, bool>> whereExp = (c => c.Name.StartsWith("Domestic"));
      //Expression<Func<Customer, bool>> whereExp = (c => c is DomesticCustomer && c.Discount < 10m);

      // figure out which types are different in the function-signature
      var fromTypes = whereExp.Type.GetGenericArguments();
      var toTypes = typeof(Func<TblCustomer, bool>).GetGenericArguments();

      if (fromTypes.Length != toTypes.Length)
        throw new NotSupportedException("Incompatible lambda function-type signatures");

      Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();
      for (int i = 0; i < fromTypes.Length; i++)
      {
        if (fromTypes[i] != toTypes[i])
          typeMap[fromTypes[i]] = toTypes[i];
      }

      // re-map all parameters that involve different types
      Dictionary<Expression, Expression> parameterMap = new Dictionary<Expression, Expression>();
      ParameterExpression[] newParams = new ParameterExpression[whereExp.Parameters.Count];
      for (int i = 0; i < newParams.Length; i++)
      {
        Type newType;
        if (typeMap.TryGetValue(whereExp.Parameters[i].Type, out newType))
        {
          parameterMap[whereExp.Parameters[i]] = newParams[i] =
              Expression.Parameter(newType, whereExp.Parameters[i].Name);
        }
        else
        {
          newParams[i] = whereExp.Parameters[i];
        }
      }

      Dictionary<string, Dictionary<string, string>> memberMap = new Dictionary<string, Dictionary<string, string>>()
      {
        { typeof(DomesticCustomer).Name, new Dictionary<string, string> { { "FedId", "AltId" } } },
        { typeof(InternationalCustomer).Name, new Dictionary<string, string> { { "IMFId", "AltId" } } },
        { typeof(IName).Name, new Dictionary<string, string> { { "Name", "Name0" } } }
      };

      // rebuild the lambda
      var body = new TestExpressionVisitor(parameterMap, memberMap).Visit(whereExp.Body);
      var newExp = Expression.Lambda<Func<TblCustomer, bool>>(body, newParams);

      var results = customers.AsQueryable().Where(newExp);

      foreach (var result in results)
      {
        Console.WriteLine($"{result.Id} {result.Preferred} {result.Discount} {result.AnnualSales}");
      }

      Console.ReadLine();
    }

    //private static void Test02()
    //{
    //  TypeInfo typeInfo = typeof(Customer).GetTypeInfo();
    //  var newExpression = Expression.New(typeInfo);
    //  var lambda = Expression.Lambda<Func<object>>(newExpression);
    //  var func = lambda.Compile();
    //  var c = func();
    //}

    //private static void Test01()
    //{
    //  var customers = new List<Customer>
    //  {
    //    new Customer { Id = 1, Preferred = true, Discount = 5m, AnnualSales = 10000m },
    //    new Customer { Id = 2, Preferred = true, Discount = 15m, AnnualSales = 25000m },
    //    new Customer { Id = 3, Preferred = false, Discount = 5m, AnnualSales = 5000m },
    //    new Customer { Id = 4, Preferred = false, Discount = 7.5m, AnnualSales = 8000m },
    //    new Customer { Id = 5, Preferred = true, Discount = 10m, AnnualSales = 23000m },
    //  };

    //  Expression<Func<Customer, bool>> whereExp = (c => c.Preferred == true && c.Discount < 10 && c.AnnualSales > 5000m);
    //  Expression<Func<Customer, int, CustomerReduced>> selectExp = ((c, i) => new CustomerReduced() { Id = c.Id, AnnualSales = c.AnnualSales });

    //  var results = customers.AsQueryable().Where(whereExp).Select(selectExp);

    //  foreach (var result in results)
    //  {
    //    //Console.WriteLine($"{result.Id} {result.Preferred} {result.Discount} {result.AnnualSales}");
    //    Console.WriteLine($"{result.Id} {result.AnnualSales}");
    //  }

    //  Console.ReadLine();
    //}
  }

  internal class CustomerReduced
  {
    internal int Id { get; set; }
    internal decimal AnnualSales { get; set; }
  }

}
