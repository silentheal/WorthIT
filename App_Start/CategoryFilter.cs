using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WorthIt.Models;

namespace WorthIt
{
    public class CategoryFilter : FilterAttribute, IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            using (var entities = new WorthITDatabaseEntities())
            {
                var model = entities.Categories.ToList();
                filterContext.Controller.ViewBag.Categories = model;
                string categoryName = filterContext.Controller.ViewBag.CategoryName;

                var modelRatingList = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    var modelRating = entities.Products
                                        .Count(x => x.Reviews.Any()
                                                    && x.Reviews.Average(y => y.Rating) > i
                                                    && x.Reviews.Average(y => y.Rating) <= i + 1
                                                    && x.Category.Name == categoryName);
                    modelRatingList.Add(modelRating);
                }

                var modelFilterByRating_1 = modelRatingList[0];
                var modelFilterByRating_2 = modelRatingList[1];
                var modelFilterByRating_3 = modelRatingList[2];
                var modelFilterByRating_4 = modelRatingList[3];
                var modelFilterByRating_5 = modelRatingList[4];

                filterContext.Controller.ViewBag.modelFilterByRating_1 = modelFilterByRating_1;
                filterContext.Controller.ViewBag.modelFilterByRating_2 = modelFilterByRating_2;
                filterContext.Controller.ViewBag.modelFilterByRating_3 = modelFilterByRating_3;
                filterContext.Controller.ViewBag.modelFilterByRating_4 = modelFilterByRating_4;
                filterContext.Controller.ViewBag.modelFilterByRating_5 = modelFilterByRating_5;

                var modelFilterByCost_1 = entities.Products.Count(x => x.Price >= 10 && x.Price < 30 && x.Category.Name == categoryName);
                var modelFilterByCost_2 = entities.Products.Count(x => x.Price >= 30 && x.Price < 50 && x.Category.Name == categoryName);
                var modelFilterByCost_3 = entities.Products.Count(x => x.Price >= 50 && x.Price < 100 && x.Category.Name == categoryName);
                var modelFilterByCost_4 = entities.Products.Count(x => x.Price >= 100 && x.Price < 150 && x.Category.Name == categoryName);
                var modelFilterByCost_5 = entities.Products.Count(x => x.Price >= 150 && x.Price < 200 && x.Category.Name == categoryName);

                filterContext.Controller.ViewBag.modelFilterByCost_1 = modelFilterByCost_1;
                filterContext.Controller.ViewBag.modelFilterByCost_2 = modelFilterByCost_2;
                filterContext.Controller.ViewBag.modelFilterByCost_3 = modelFilterByCost_3;
                filterContext.Controller.ViewBag.modelFilterByCost_4 = modelFilterByCost_4;
                filterContext.Controller.ViewBag.modelFilterByCost_5 = modelFilterByCost_5;
            }    
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }
    }
}
