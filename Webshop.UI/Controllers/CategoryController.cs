using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Webshop.UI.App_Data;

namespace Webshop.UI.Controllers
{
    public class CategoryController : Controller
    {
        private WebshopContext _context;

        public CategoryController()
        {
            _context = new WebshopContext();
        }

        // GET: Category
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            return View(category);
        }
    }
}