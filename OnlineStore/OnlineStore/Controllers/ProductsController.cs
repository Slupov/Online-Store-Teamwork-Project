using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models;
using Microsoft.AspNet.Identity;

namespace OnlineStore.Controllers
{
    public class ProductsController : Controller
    {
        private OnlineStorePSGMEntities db = new OnlineStorePSGMEntities();
        private Dictionary<string, string[]> filtersByCategory = new Dictionary<string, string[]>
        {
            {"TV",new string[] {"Smart","Curved","4K", "LG", "Samsung"} },
            {"Earbud Headphones",new string[] {"Panasonic","Sony","Apple"} },
            {"On-Ear Headphones",new string[] {"Beats","Amazon","Koss","Sennheiser"} },
            {"Over-Ear Headphones",new string[] {"Sennheiser", "Bose", "LilGadgets", "Beats"  } },
            {"Home Theater Systems",new string[] {"Samsung","Bose","Sony"} },
            {"Surround Sound Systems",new string[] {"5.1"} },
            {"Speakers",new string[] { "Klipsch","Edifier","Polk","Floorstanding","Bookshelf" } },
            {"Subwoofers",new string[] {"Polk","Acoustic","Yamaha"}},
            {"Blu-ray Players",new string[] { "Sony", "Samsung", "Panasonic" } },
            {"DSLR Cameras",new string[] {"Nikon","Canon","Kit"} },
            {"Mirrorless Cameras",new string[] {"Sony","Panasonic","Fujifilm","Nikon"} },
            {"Camera Lenses",new string[] { "Canon", "Sony", "Samsung" } },
            {"Smart Phones",new string[] {"Samsung","Apple","LG","Moto"} },
            {"Phone Cases",new string[] {"Iphone","Samsung","LG"} },
            {"Phone Accessories",new string[] {"USD","SD","Protector"} },
            {"Tower PCs",new string[] {"Dell","Asus","Acer","Lenovo","HP"} },
            {"PC Monitors",new string[] {"Acer","Dell","Asus","Samsung"} },
            {"Tablets",new string[] {"Samsung","Apple","Nexus","Android","Tablet PC"} },
            {"Laptops",new string[] {"Apple","Acer","Asus","HP"} },
            {"Mice",new string[] {"Logitech","Microsoft","HP"} },
            {"Keyboards",new string[] {"Microsoft","Apple","Logitech","Razer"} },
            {"Drives & Storage",new string[] {"Samsung","Seagate","WD","SanDisk"} },
            {"",new string[] {} },
                        {"All categories",new string[] {} }

        };

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Member);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Username");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,ProductType,Description,Stock,Price,ImageSource,MemberID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Username", product.MemberID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Username", product.MemberID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ProductType,Description,Stock,Price,ImageSource,MemberID")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MemberID = new SelectList(db.Members, "MemberID", "Username", product.MemberID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult BrowseProducts(string category = "All categories",
                                            string[] filters = null,
                                            string[] checkedFilters = null,
                                            string minPriceInput = "",
                                            string maxPriceInput = "",
                                            string searchTerms = null,
                                            string orderBy = "name,asc")
        {

            ViewBag.Messege = "Listing " + category;

            var products = db.Products.ToList();
            filters = filtersByCategory[category];


            if (checkedFilters != null)
            {
                products = products.Where(p => ArraysHaveCommonWords(checkedFilters,p.ProductName.Split(' '))).ToList();
            }
            if (category != "" && category != "All categories")
            {
                products = products.Where(p => p.ProductType.Equals(category)).ToList();
            }

            if (searchTerms != null)
            {
                foreach (string term in searchTerms.Split(' '))
                {
                    products = products.Where(p => p.ProductName.ToLower().Contains(term.ToLower())).ToList();
                }
            }

            switch (orderBy)
            {
                case "name,asc":
                    products = products.OrderBy(p => p.ProductName).ToList();
                    break;
                case "name,desc":
                    products = products.OrderByDescending(p => p.ProductName).ToList();
                    break;
                case "price,asc":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "price,desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
            }

            double minPrice = minPriceInput == "" ? products.Select(p => p.Price).Min() : double.Parse(minPriceInput) - 1;
            double maxPrice = maxPriceInput == "" ? products.Select(p => p.Price).Max() : double.Parse(maxPriceInput) + 1;

            products = products.Where(p => p.Price > minPrice).ToList();
            products = products.Where(p => p.Price < maxPrice).ToList();

            ViewBag.category = category;
            ViewBag.filters = filters;
            ViewBag.searchTerms = searchTerms;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;

            return View(products);
        }

        public ActionResult MySales()
        {
            var aspUserName = User.Identity.GetUserName();
            var dbUser = db.Members.Where(m => m.Username == aspUserName).First();
            var products = db.Products.Where(p => p.MemberID == dbUser.MemberID);

            return View(products);
        }

        private bool ArraysHaveCommonWords(string[] a, string[] b)
        {
            foreach (string word in a)
            {
                if (b.Contains(word))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
