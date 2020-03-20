using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using E_commerce.Data;
using E_commerce.Library;
using E_commercePIM.ViewModels;

namespace E_commercePIM.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly WebshopContext _context;

        public ProductsController(ProductRepository repository, IMapper mapper, WebshopContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetProductsAsync());
        }

        public async Task<ActionResult> Editor(int? id)
        {
            var product = await _repository.GetProductAsync(id);
            var model = _mapper.Map<ProductEditorViewModel>(product);

            var categories = _context.Categories
                .Include(c => c.Products).ToList();
            model.AvailableCategories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(), 
                    Text = c.Name,
                    Selected = c.Products.Contains(product)
                })
                .ToList();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(ProductEditorViewModel model)
        {
            return !ModelState.IsValid ? View(model) : null;
        }

    }
}