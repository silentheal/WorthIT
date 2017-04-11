using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt
{
    public class CartFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (var entities = new WorthITDatabaseEntities())
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var name = HttpContext.Current.User.Identity.Name;
                    
                    if (entities.Carts.Where(x => x.AspNetUser.UserName == name).Count() == 0)
                    {
                        filterContext.Controller.ViewBag.cartCount = 0;
                        filterContext.Controller.ViewBag.cartPriceSum = 0;
                    }
                    else
                    {
                        filterContext.Controller.ViewBag.cartCount = entities.Carts.First(x => x.AspNetUser.UserName == name).CartProducts.Count();

                        decimal? cartAllProductPrice = 0;
                        foreach (var item in entities.Carts.First(x => x.AspNetUser.UserName == name).CartProducts)
                        {
                            cartAllProductPrice += (item.Product.Price * item.Quantity);
                        }
                        filterContext.Controller.ViewBag.cartPriceSum = cartAllProductPrice;
                    }

                    
                }
                else
                {
                    filterContext.Controller.ViewBag.cartCount = 0;
                }

                
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}