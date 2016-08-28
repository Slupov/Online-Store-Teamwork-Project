using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class HomeController : Controller
    {
        private OnlineStorePSGMEntities db = new OnlineStorePSGMEntities();

        public ActionResult Index()
        {
            ViewBag.category = "All categories";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ShopByCategories()
        {
            ViewBag.Message = "Your categories.";
            ViewBag.category = "All categories";
            var categories = db.Products.Select(p => p.ProductType).Distinct();
            return View(categories);
        }
    }
}