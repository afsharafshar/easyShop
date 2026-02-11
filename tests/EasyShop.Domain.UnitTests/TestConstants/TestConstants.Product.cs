namespace EasyShop.Domain.UnitTests.TestConstants;

public static partial class TestConstants
{
    public static class Product
    {
        public static readonly Guid Id = Guid.NewGuid();
        public static readonly int Qty = 5;
        public static readonly decimal Price = 50;
    }
}