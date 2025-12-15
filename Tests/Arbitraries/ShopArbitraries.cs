using FsCheck;
using FsCheck.Fluent;
using Shopping_Cart_System.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopping_Cart_System.Tests.Arbitraries
{
    public static class ShopArbitraries
    {
        public static Arbitrary<Product> Product()
        {
            var productGen =
                from id in NonEmptyAlphaString()
                from name in NonEmptyAlphaString()
                from price in Gen.Choose(1, 10000).Select(x => x / 100m)
                select new Product(id, name, price);

            return productGen.ToArbitrary();
        }

        public static Arbitrary<int> Quantity()
        {
            return Gen.Choose(1, 100).ToArbitrary();
        }

        public static Arbitrary<ShoppingCart> Cart()
        {
            var productsGen = Gen.ListOf(Product().Generator);
            var quantitiesGen = Gen.ListOf(Quantity().Generator);

            return Gen.Zip(productsGen, quantitiesGen)
                      .Select(t => BuildCart(t.Item1.ToList(), t.Item2.ToList()))
                      .ToArbitrary();
        }

        private static ShoppingCart BuildCart(List<Product> products, List<int> quantities)
        {
            var cart = new ShoppingCart();
            var count = Math.Min(products.Count, quantities.Count);

            for (int i = 0; i < count; i++)
            {
                cart.Add(products[i], quantities[i]);
            }

            return cart;
        }
        
        private static Gen<string> NonEmptyAlphaString()
        {
            return Gen.Elements("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray())
                      .NonEmptyListOf()
                      .Select(chars => new string(chars.ToArray()));
        }
    }
}