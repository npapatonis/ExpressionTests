using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTests
{
  // Some comment
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
        new TblCustomer { Id = 1, Type = 2000, Preferred = true, Discount = 5m, AnnualSales = 10000m },
        new TblCustomer { Id = 2, Type = 3000, Preferred = true, Discount = 15m, AnnualSales = 25000m },
        new TblCustomer { Id = 3, Type = 3000, Preferred = false, Discount = 5m, AnnualSales = 5000m },
        new TblCustomer { Id = 4, Type = 2000, Preferred = false, Discount = 7.5m, AnnualSales = 8000m },
        new TblCustomer { Id = 5, Type = 4000, Preferred = true, Discount = 10m, AnnualSales = 23000m },
      };

      var hashSet = new HashSet<int> { 3000, 4000 };

      //Expression<Func<Customer, bool>> whereExp = (c => hashSet.Contains(c.Type) && c.Preferred == true && c.Discount < 10 && c.AnnualSales > 5000m);
      Expression<Func<Customer, bool>> whereExp = (c => c is DomesticCustomer);
      //Expression<Func<Customer, bool>> whereExp = (c => hashSet.Contains(c.Type));

      TestExpressionVisitor expVisitor = new TestExpressionVisitor();
      var newExp = (Expression<Func<Customer, bool>>)expVisitor.TransformExpression(whereExp);

      var results = customers.AsQueryable().Where(newExp);

      foreach (var result in results)
      {
        Console.WriteLine($"{result.Id} {result.Preferred} {result.Discount} {result.AnnualSales}");
      }

      Console.ReadLine();
    }

    private static void Test02()
    {
      TypeInfo typeInfo = typeof(Customer).GetTypeInfo();
      var newExpression = Expression.New(typeInfo);
      var lambda = Expression.Lambda<Func<object>>(newExpression);
      var func = lambda.Compile();
      var c = func();
    }

    private static void Test01()
    {
      var customers = new List<Customer>
      {
        new Customer { Id = 1, Preferred = true, Discount = 5m, AnnualSales = 10000m },
        new Customer { Id = 2, Preferred = true, Discount = 15m, AnnualSales = 25000m },
        new Customer { Id = 3, Preferred = false, Discount = 5m, AnnualSales = 5000m },
        new Customer { Id = 4, Preferred = false, Discount = 7.5m, AnnualSales = 8000m },
        new Customer { Id = 5, Preferred = true, Discount = 10m, AnnualSales = 23000m },
      };

      Expression<Func<Customer, bool>> whereExp = (c => c.Preferred == true && c.Discount < 10 && c.AnnualSales > 5000m);
      Expression<Func<Customer, int, CustomerReduced>> selectExp = ((c, i) => new CustomerReduced() { Id = c.Id, AnnualSales = c.AnnualSales });

      var results = customers.AsQueryable().Where(whereExp).Select(selectExp);

      foreach (var result in results)
      {
        //Console.WriteLine($"{result.Id} {result.Preferred} {result.Discount} {result.AnnualSales}");
        Console.WriteLine($"{result.Id} {result.AnnualSales}");
      }

      Console.ReadLine();
    }
  }

  internal class CustomerReduced
  {
    internal int Id { get; set; }
    internal decimal AnnualSales { get; set; }
  }

}
