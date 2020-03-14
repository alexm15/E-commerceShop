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
using AutoMapper;
using WebGrease.Css.Extensions;
using Webshop.UI.App_Data;
using Webshop.UI.Models;
using Webshop.UI.ViewModels;

namespace Webshop.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext _context;
        private IMapper _mapper;

        public ProductController(WebshopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = await _context.Products.FindAsync(id);
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
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
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
            var product = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var allCategories = await _context.Categories.Include(c => c.Products).ToListAsync();

            var availableCategories = allCategories.Select(c => new AssignedCategoryData
            {
                Id = c.Id,
                Name = c.Name,
                Assigned = c.Products.Contains(product)
            }).ToList();

            var viewModel = _mapper.Map<ProductEditorViewModel>(product);
            viewModel.AvailableCategories = availableCategories;

            return View(viewModel);
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
                var dbProduct = await _context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == model.Id);
                if (dbProduct == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                dbProduct.Id = model.Id;
                dbProduct.Name = model.Name;
                dbProduct.Description = model.Description;
                dbProduct.Price = model.Price;

                UpdateProduct(model, dbProduct);
                
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            return View(model);
        }

        private void UpdateProduct(ProductEditorViewModel model, Product dbProduct)
        {
            var selectedCategoryIds = new HashSet<int>(model.AvailableCategories.Where(c => c.Assigned).Select(c => c.Id));
            foreach (var dbCategory in _context.Categories)
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
            Product product = await _context.Products.FindAsync(id);
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
            Product product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
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
