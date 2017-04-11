using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt.Controllers
{
    public class CartController : Controller
    {
        private WorthITDatabaseEntities entities = new WorthITDatabaseEntities();
        
        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Cart
        public ActionResult Index()
        {
            ICollection<CartProduct> model = null;
            if (User.Identity.IsAuthenticated && entities.Carts.Where(x => x.AspNetUser.UserName == User.Identity.Name).Count() != 0)
            {
                model = entities.Carts.Single(x => x.AspNetUser.UserName == User.Identity.Name).CartProducts;
            }
            else
            {
                return View();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(int? id)
        {
            Cart c = null;

            if (User.Identity.IsAuthenticated)
            {
                var user = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name);

                if (entities.Carts.Where(x => x.UserId == user.Id).Count() == 0)
                {
                    entities.Carts.Add(new Cart { UserId = user.Id });
                    entities.SaveChanges();
                }

                c = user.Carts.First(); //c => get 1 result
                if (c == null)
                {
                    c = new Cart(); //c = empty Cart
                    user.Carts.Add(c); //result => id = auto Increase, UserId = user.id
                }
            }
            else
            {
                c = new Cart();
            }

            if (c.CartProducts.Where(x => x.ProductId == id).Count() > 0)
            {
                c.CartProducts.Single(x => x.ProductId == id).Quantity += 1;
            }
            else
            {
                c.CartProducts.Add(new CartProduct { ProductId = id ?? 0, Quantity = 1 });
            }
            
            entities.SaveChanges();
            Session.Add("CartNumber", c.Id);
            
            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            Cart c = null;

            if (User.Identity.IsAuthenticated)
            {
                var user = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name);
                c = user.Carts.First();

                var target = c.CartProducts.First(x => x.ProductId == id);
                c.CartProducts.Remove(target);
            }

            entities.SaveChanges();
            Session.Remove("CartNumber");

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult Refresh(int? id, int? quantity)
        {
            Cart c = null;
            if (User.Identity.IsAuthenticated)
            {
                if (quantity > entities.Products.Single(x => x.Id == id).Inventory || !quantity.HasValue)
                {
                    quantity = entities.Products.Single(x => x.Id == id).Inventory;
                }
                
                var user = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name);
                c = user.Carts.First();

                c.CartProducts.Single(x => x.ProductId == id).Quantity = quantity;
                
                entities.SaveChanges();
            }

            return new HttpStatusCodeResult(200);
        }
    }
}