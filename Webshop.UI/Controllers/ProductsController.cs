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
    public class ProductsController : Controller
    {
        private WebshopContext _context;
        public ProductsController()
        {
            _context = new WebshopContext();
        }


        // GET: Products
        public async Task<ActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Categories).ToListAsync();
            return View(products);
        }
    }
}