using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_commerce.Data;
using E_commerce.Library;

namespace Webshop.UI.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _repo;

        public CategoryController(CategoryRepository repo)
        {
            _repo = repo;
        }
        

        // GET: Category
        public async Task<ActionResult> Index()
        {
            var categories = await _repo.GetCategoriesAsync();

            return View(categories);
        }

        

        // GET: Category/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var category = await _repo.GetCategoryAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(category);
                return RedirectToAction("Index");
            }

            return View(category);
        }



        // GET: Category/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = await _repo.GetCategoryAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _repo.UpdateAsync(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        

        // GET: Category/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = await _repo.GetCategoryAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
