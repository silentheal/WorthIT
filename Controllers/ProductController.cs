using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;
using System.Data.Entity;
using System.Collections;

namespace WorthIt.Controllers
{
    public class ProductController : Controller
    {
        WorthITDatabaseEntities entities = new WorthITDatabaseEntities();

        protected override void Dispose(bool disposing)
        {
            entities.Dispose();
            base.Dispose(disposing);
        }

        // GET: Product
        [CartFilter]
        public async Task<ActionResult> Index(string id, int? rating, int? minprice, int? maxprice)
        {
            IEnumerable<Product> model = null;
            if (id != null)
            {
                var category = await entities.Categories.SingleAsync(x => x.Name == id);
                model = category.Products;
            }
            else
            {
                model = entities.Products;
            }

            if (rating.HasValue)
            {
                model = model.Where(x => x.Reviews.Any() && x.Reviews.Average(y => y.Rating) > rating - 1 && x.Reviews.Average(y => y.Rating) <= rating);
            }

            if (minprice.HasValue && maxprice.HasValue)
            {
                model = model.Where(x => x.Price >= minprice && x.Price < maxprice);
            }

            ViewBag.CategoryName = id;
           
            return View(model);
        }

        public ActionResult Search(string query)
        {
            var searchResult = entities.Products.Where(x => x.Name.Contains(query)).Select(x => new { Name = x.Name, URL = "/Product/Detail/" + x.Id + "?name=" + x.Category.Name});
            
            return Json(searchResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategoryProducts(string id, int? rating)
        {
            IEnumerable<Product> model = null;
            if (id != null)
            {
                model = entities.Categories.Single(x => x.Name == id).Products;
            }
            else
            {
                model = entities.Products;
            }

            if (rating.HasValue)
            {
                model = model.Where(x => x.Reviews.Any() && x.Reviews.Average(y => y.Rating) > rating - 1);
            }

            return Json(model.Select(x => new Product { Id = x.Id, ManufacturingUserId = x.ManufacturingUserId, Name = x.Name  }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(int? id, string name)
        {
            Product model = null;
            
            if (id.HasValue)
            {
                model = entities.Products.First(x => x.Id == id);
                if (name != null)
                {
                    ViewBag.equalCategoryItems = entities.Products.Where(x => x.Category.Name == name);
                }
                else
                {
                    ViewBag.equalCategoryItems = entities.Products.Any();
                }
                ViewBag.manufactererItems = entities.Products.Where(x => x.ManufacturingUserId == model.ManufacturingUserId);   
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewCreate(Review reviewModel, int? id)
        {
            if (ModelState.IsValid)
            {
                reviewModel.UserId = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name).Id;
                reviewModel.ProductId = id;
                reviewModel.Created = DateTime.Now;
                reviewModel.Modified = DateTime.Now;
                reviewModel.Abuse = 0;
                reviewModel.Like = 0;
                reviewModel.Product = entities.Products.Single(x => x.Id == id);

                entities.Reviews.Add(reviewModel);
                entities.SaveChanges();
            }

            return RedirectToAction("detail", "product", new { id = id, name = reviewModel.Product.Category.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewEdit(Review reviewModel, int productId)
        {
            string userId = entities.AspNetUsers.Single(x => x.UserName == User.Identity.Name).Id;

            var r = entities.Reviews.FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);
            r.Rating = reviewModel.Rating;
            r.description = reviewModel.description;
            r.Modified = DateTime.Now;
            r.Header = reviewModel.Header;
            reviewModel.Product = entities.Products.Single(x => x.Id == productId);

            entities.SaveChanges();

            return RedirectToAction("detail", "product", new { id = productId, name = reviewModel.Product.Category.Name });
        }

        [HttpPost]
        public ActionResult ReviewDelete(int? id)
        {
            var target = entities.Reviews.Single(x => x.Id == id);
            entities.Reviews.Remove(target);
            entities.SaveChanges();

            return new HttpStatusCodeResult(200);
        }
    }
}