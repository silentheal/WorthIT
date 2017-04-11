using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            using (var entities = new WorthITDatabaseEntities())
            {
                var carousel = entities.carouselBanners.Where(x => x.IsActive == 1).ToList();

                return View(carousel);
            }
        }
    }
}