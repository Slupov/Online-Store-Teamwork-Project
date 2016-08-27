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



        public ActionResult FilterProducts(string category = null, string[] filters = null, int minPrice = -1, int maxPrice = -1, string searchTerms = null)
        {

            ViewBag.Messege = "Listing " + category;

            var products = db.Products.ToList();

            if (category != null && category != "All categories")
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

            if (minPrice > 0)
            {
                products = products.Where(p => p.Price > minPrice).ToList();
            }

            if (maxPrice > 0)
            {
                products = products.Where(p => p.Price < maxPrice).ToList();
            }

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
    }
}
