using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt
{
    public class CheckoutFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (var entities = new WorthITDatabaseEntities())
            {
                filterContext.Controller.ViewBag.Month = new List<SelectListItem>() {
                    new SelectListItem { Text = "1", Value = "01" },
                    new SelectListItem { Text = "2", Value = "02" },
                    new SelectListItem { Text = "3", Value = "03" },
                    new SelectListItem { Text = "4", Value = "04" },
                    new SelectListItem { Text = "5", Value = "05" },
                    new SelectListItem { Text = "6", Value = "06" },
                    new SelectListItem { Text = "7", Value = "07" },
                    new SelectListItem { Text = "8", Value = "08" },
                    new SelectListItem { Text = "9", Value = "09" },
                    new SelectListItem { Text = "10", Value = "10" },
                    new SelectListItem { Text = "11", Value = "11" },
                    new SelectListItem { Text = "12", Value = "12" },
                };

                filterContext.Controller.ViewBag.Year = new List<SelectListItem>()
                {
                    new SelectListItem { Text = DateTime.Now.Year.ToString(), Value = DateTime.Now.Year.ToString() },
                    new SelectListItem { Text = DateTime.Now.AddYears(1).Year.ToString(), Value = DateTime.Now.AddYears(1).Year.ToString() },
                    new SelectListItem { Text = DateTime.Now.AddYears(2).Year.ToString(), Value = DateTime.Now.AddYears(2).Year.ToString() },
                    new SelectListItem { Text = DateTime.Now.AddYears(3).Year.ToString(), Value = DateTime.Now.AddYears(3).Year.ToString() },
                    new SelectListItem { Text = DateTime.Now.AddYears(4).Year.ToString(), Value = DateTime.Now.AddYears(4).Year.ToString() },
                    new SelectListItem { Text = DateTime.Now.AddYears(5).Year.ToString(), Value = DateTime.Now.AddYears(5).Year.ToString() }
                };

                filterContext.Controller.ViewBag.Country = entities.Countries;
                filterContext.Controller.ViewBag.State = entities.States;

                filterContext.Controller.ViewBag.userAddress = entities.Addresses.Where(x => x.AspNetUser.UserName == HttpContext.Current.User.Identity.Name).ToList();
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {

        }
    }
}