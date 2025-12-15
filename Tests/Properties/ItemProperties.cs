using System;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;

namespace Shopping_Cart_System.Tests.Properties
{
    public class ItemProperties
    {
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Create_WithPositiveQuantity_ShouldSucceed(Product product, int quantity)
        {
            var item = new CartItem(product, quantity);
            return (item.Quantity == quantity).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Create_WithNonPositiveQuantity_ShouldThrow(Product product)
        {
            return Prop.ForAll(Gen.Choose(-100, 0).ToArbitrary(), quantity =>
            {
                try
                {
                    new CartItem(product, quantity);
                    return false;
                }
                catch (ArgumentException)
                {
                    return true; 
                }
            });
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Increase_ShouldAddQuantity(Product product, int initialQuantity, int amount)
        {
            var item = new CartItem(product, initialQuantity);
            item.Increase(amount);
            return (item.Quantity == initialQuantity + amount).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Decrease_ValidAmount_ShouldSubtract(Product product, int initialQuantity, int amount)
        {
            return (amount < initialQuantity).Implies(() =>
            {
                var item = new CartItem(product, initialQuantity);
                item.Decrease(amount);
                return item.Quantity == initialQuantity - amount;
            });
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Decrease_TooMuch_ShouldThrow(Product product, int initialQuantity, int amount)
        {
            return (amount >= initialQuantity).Implies(() =>
            {
                var item = new CartItem(product, initialQuantity);
                try
                {
                    item.Decrease(amount);
                    return false;
                }
                catch (InvalidOperationException)
                {
                    return true;    
                }
            });
        }
    }
}