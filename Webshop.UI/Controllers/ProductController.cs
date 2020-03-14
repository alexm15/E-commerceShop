using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using Webshop.UI.App_Data;
using Webshop.UI.Models;
using Webshop.UI.ViewModels;

namespace Webshop.UI.Controllers
{
    public class ProductController : Controller
    {
        private WebshopContext db = new WebshopContext();

        // GET: Product
        public async Task<ActionResult> Index()
        {
            return View(await db.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }



            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = await db.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var allCategories = await db.Categories.Include(c => c.Products).ToListAsync();

            var availableCategories = allCategories.Select(c => new AssignedCategoryData
            {
                Id = c.Id,
                Name = c.Name,
                Assigned = c.Products.Contains(product)
            }).ToList();

            var productEditorViewModel = new ProductEditorViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                AvailableCategories = availableCategories
            };


            return View(productEditorViewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductEditorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbProduct = await db.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == model.Id);
                if (dbProduct == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                dbProduct.Id = model.Id;
                dbProduct.Name = model.Name;
                dbProduct.Description = model.Description;
                dbProduct.Price = model.Price;

                UpdateProduct(model, dbProduct);
                
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            return View(model);
        }

        private void UpdateProduct(ProductEditorViewModel model, Product dbProduct)
        {
            var selectedCategoryIds = new HashSet<int>(model.AvailableCategories.Where(c => c.Assigned).Select(c => c.Id));
            foreach (var dbCategory in db.Categories)
            {
                if (selectedCategoryIds.Contains(dbCategory.Id))
                {
                    if (!dbProduct.Categories.Contains(dbCategory))
                    {
                        dbProduct.Categories.Add(dbCategory);
                    }
                }
                else
                {
                    if (dbProduct.Categories.Contains(dbCategory))
                    {
                        dbProduct.Categories.Remove(dbCategory);
                    }
                }
            }
        }

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
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
    }

    public class Logger
    {
        public static void Log(string obj)
        {
            Console.WriteLine(obj);
        }
    }

    public class AssignedCategoryData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
