using Lab3_ShoppingCart.ExtensionMethod;
using Lab3_ShoppingCart.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab3_ShoppingCart.Controllers
{
	public class ProductController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult List(int Id)
        {
            List<Product> products = null;
            List<Category> categories = null;
            using (var context = new NorthwindContext())
            {
                if (Id == null || Id == 0)
                {
                    products = context.Products.ToList();
                }
                else
                {
                    products = context.Products.Where(x => x.CategoryId == Id).ToList();
                }
                categories = context.Categories.ToList();
            }
            ViewBag.Categories = categories;
            return View(products);
        }


        public List<CartItems> cartItems
        {
            get
            {
                var data = HttpContext.Session.Get<List<CartItems>>("GioHang");
                if (data == null)
                {
                    data = new List<CartItems>();
                }
                return data;
            }
        }

        public IActionResult AddToCart(int Id)
        {
            using (var context = new NorthwindContext())
            {
                var myCart = cartItems;
                var item = myCart.SingleOrDefault(p => p.ProductId == Id);
                if (item == null)
                {
                    var p = context.Products.SingleOrDefault(p => p.ProductId == Id);
                    item = new CartItems
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        quantity = 1
                    };
                    myCart.Add(item);
                }
                else
                {
                    item.quantity++;
                }
                HttpContext.Session.Set("GioHang", myCart);

                return RedirectToAction("Cart");
            }
        }

        public IActionResult Order()
        {
            var myCart = cartItems;
            return View(myCart);
        }

        public IActionResult DoOrder()
        {
            string shipAddress = Request.Form["ShipAddress"];
            string shipCity = Request.Form["ShipCity"];
            string shipCountry = Request.Form["ShipContry"];
            var myCart = cartItems;
            using (var context = new NorthwindContext())
            {
                List<OrderDetail> details = new List<OrderDetail>();
                Order order = new Order
                {
                    CustomerId = "RATTC",
                    EmployeeId = 1,
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now,
                    ShippedDate = null,
                    ShipVia = 1,
                    Freight = 5,
                    ShipName = "Bon app",
                    ShipAddress = shipAddress,
                    ShipCity = shipCity,
                    ShipRegion = null,
                    ShipPostalCode = null,
                    ShipCountry = shipCountry,

                };

                context.Orders.Add(order);
                context.SaveChanges();
                int oID = context.Orders.OrderBy(x => x.OrderId).Last().OrderId;
                foreach (var c in myCart)
                {
                    OrderDetail o = new OrderDetail(oID,c.ProductId,(decimal)c.UnitPrice,(short)c.quantity, 0);
                    details.Add(o);
                }
                foreach(var detail in details)
                {
                    context.OrderDetails.Add(detail);
                    context.SaveChanges();
                }
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Cart");

        }

        public IActionResult Remove(int Id)
        {
            using (var context = new NorthwindContext())
            {
                var myCart = cartItems;
                var item = myCart.SingleOrDefault(p => p.ProductId == Id);
                myCart.Remove(item);
                HttpContext.Session.Set("GioHang", myCart);
                return RedirectToAction("Cart");
            }
        }


        public IActionResult Cart()
        {
            return View(cartItems);
        }
    }
}
