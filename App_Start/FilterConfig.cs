using System.Web;
using System.Web.Mvc;

namespace WorthIt
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CategoryFilter());
            filters.Add(new CheckoutFilter());
            filters.Add(new CartFilter());
        }
    }
}
