using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt.Controllers
{
    public class ReceiptController : Controller
    {
        WorthITDatabaseEntities entities = new WorthITDatabaseEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Receipt
        public ActionResult Index()
        {
            IEnumerable<OrderProduct> orderModel = null;
            if (User.Identity.IsAuthenticated)
            {
                var orderId = entities.Orders.OrderByDescending(x => x.Created).FirstOrDefault(x => x.AspNetUser.UserName == User.Identity.Name).Id;

                orderModel = entities.OrderProducts.Where(x => x.OrderId == orderId);
            }
            return View(orderModel);
        }
    }
}