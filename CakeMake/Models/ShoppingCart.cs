﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CakeMake.Models
{
    public class ShoppingCart
    {
        private readonly AppDbContext _appDbContext;
        public string ShoppingCartId { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }

        public ShoppingCart(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            var context = services.GetService<AppDbContext>();
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Cake cake, int amount)
        {
            var shoppingCartItem = _appDbContext.ShopppingCartItems.SingleOrDefault(
                s => s.Cake.CakeId == cake.CakeId && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Cake = cake,
                    Amount = amount
                };

                _appDbContext.ShopppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }

            _appDbContext.SaveChanges();
        }

        public int RemoveFromCart(Cake cake)
        {
            var shoppingCartItem = _appDbContext.ShopppingCartItems.SingleOrDefault(
                s => s.Cake.CakeId == cake.CakeId && s.ShoppingCartId == ShoppingCartId);

            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _appDbContext.ShopppingCartItems.Remove(shoppingCartItem);
                }
            }

            _appDbContext.SaveChanges();

            return localAmount;
        }

        public int IncreaseQuanInCart(Cake cake)
        {
            var shoppingCartItem = _appDbContext.ShopppingCartItems.SingleOrDefault(
                s => s.Cake.CakeId == cake.CakeId && s.ShoppingCartId == ShoppingCartId);



            var localAmount = 0;



            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount >= 1)
                {
                    shoppingCartItem.Amount++;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _appDbContext.ShopppingCartItems.Add(shoppingCartItem);
                }
            }



            _appDbContext.SaveChanges();



            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _appDbContext.ShopppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Include(s => s.Cake)
                .ToList());
        }

        public void ClearCart()
        {
            var cartItems = _appDbContext.ShopppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId);

            _appDbContext.ShopppingCartItems.RemoveRange(cartItems);
            _appDbContext.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _appDbContext.ShopppingCartItems.Where(c => c.ShoppingCartId == ShoppingCartId)
                .Select(c => c.Cake.Price * c.Amount).Sum();

            return total;
        }
    }
}
