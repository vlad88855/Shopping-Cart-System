using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Shopping_Cart_System.Domain;
using Shopping_Cart_System.Tests.Arbitraries;

namespace Shopping_Cart_System.Tests.Properties
{
    public class RepositoryProperties
    {
        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Save_ShouldAddCartToStorage(ShoppingCart cart)
        {
            var repo = new InMemoryCartRepository();
            repo.Save(cart);

            return (repo.All().Count == 1 && repo.All().Contains(cart)).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Save_MultipleCarts_ShouldStoreAll(List<ShoppingCart> carts)
        {
            var repo = new InMemoryCartRepository();

            foreach (var cart in carts)
            {
                repo.Save(cart);
            }

            var stored = repo.All();

            bool countMatch = stored.Count == carts.Count;
            bool contentMatch = carts.All(c => stored.Contains(c));

            return (countMatch && contentMatch).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(ShopArbitraries) })]
        public Property Save_ShouldPreserveCartData(ShoppingCart cart)
        {
            var repo = new InMemoryCartRepository();
            repo.Save(cart);

            var retrieved = repo.All().First();

            return (ReferenceEquals(cart, retrieved) && cart.TotalPrice() == retrieved.TotalPrice()).ToProperty();
        }

        [Fact] 
        public void NewRepository_ShouldBeEmpty()
        {
            var repo = new InMemoryCartRepository();
            Assert.Empty(repo.All());
        }
    }
}