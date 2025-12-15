using System;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;

namespace Shopping_Cart_System.Tests.Properties
{
    public class CartProperties
    {
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Add_NewProduct_ShouldIncreaseItemsCount(ShoppingCart cart, Product product, int quantity)
        {
            bool isNew = !cart.Items.Any(i => i.Product.Id == product.Id);

            return isNew.Implies(() =>
            {
                int initialCount = cart.Items.Count;
                cart.Add(product, quantity);
                return cart.Items.Count == initialCount + 1;
            });
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Add_ExistingProduct_ShouldUpdateQuantity(Product product, int qty1, int qty2)
        {
            var cart = new ShoppingCart();
            cart.Add(product, qty1);

            int initialItemsCount = cart.Items.Count; 
            cart.Add(product, qty2);

            bool countUnchanged = cart.Items.Count == initialItemsCount;
            bool quantityUpdated = cart.Items.First().Quantity == qty1 + qty2;

            return (countUnchanged && quantityUpdated).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Remove_ExistingProduct_ShouldSucceed(Product product, int quantity)
        {
            var cart = new ShoppingCart();
            cart.Add(product, quantity);

            cart.Remove(product.Id);

            return (!cart.Items.Any()).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Remove_NonExistingProduct_ShouldThrow(ShoppingCart cart, string productId)
        {
            bool notInCart = !cart.Items.Any(i => i.Product.Id == productId);

            return notInCart.Implies(() =>
            {
                try
                {
                    cart.Remove(productId);
                    return false; 
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
            });
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property TotalPrice_ShouldMatchSumOfItems(ShoppingCart cart)
        {
            decimal expectedTotal = cart.Items.Sum(i => i.Product.Price * i.Quantity);
            return (cart.TotalPrice() == expectedTotal).ToProperty();
        }
    }
}