using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using E_commerce.Data;
using E_commerce.Library;

namespace E_commercePIM.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductRepository _repository;

        public ProductsController(ProductRepository repository)
        {
            _repository = repository;
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetProductsAsync());
        }

        public async Task<ActionResult> Editor(int? id)
        {
            var product = await _repository.GetProductAsync(id);
            return View(product);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(Product product)
        {
            return !ModelState.IsValid ? View(product) : null;
        }

    }
}