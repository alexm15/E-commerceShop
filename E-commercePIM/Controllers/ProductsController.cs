using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            
            model.AvailableCategories = PopulateCategories(product);
            return View(model);
        }


        private IEnumerable<SelectListItem> PopulateCategories(Product product)
        {
            var categories = _context.Categories
                .Include(c => c.Products).ToList();
            return categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Products.Contains(product)
                })
                .ToList();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(ProductEditorViewModel model)
        {
            var product = await _repository.GetProductAsync(model.Id);
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = PopulateCategories(product);
                return View(model);
            }
            product = _mapper.Map(model, product);
            await _repository.AddOrUpdateProduct(product, new HashSet<int>(model.SelectedCategories));
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}