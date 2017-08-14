namespace ExpressionTests
{
  internal abstract class Customer
  {
    protected Customer()
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

    internal string FedId { get; set; }
  }

  internal class InternationalCustomer : Customer
  {
    internal InternationalCustomer() : base() { }

    internal string IMFId { get; set; }
  }
}
