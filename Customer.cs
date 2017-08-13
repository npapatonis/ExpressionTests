namespace ExpressionTests
{
  internal class Customer
  {
    internal Customer()
    {
    }

    internal int Id { get; set; }
    internal bool Preferred { get; set; }
    internal decimal Discount { get; set; }
    internal decimal AnnualSales { get; set; }
  }

  internal class DomesticCustomer : Customer
  {
    internal DomesticCustomer() : base() { }
  }

  internal class InternationalCustomer : Customer
  {
    internal InternationalCustomer() : base() { }
  }
}
