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
        public async Task<ActionResult> Index(string sortOrder, string category, string searchString)
        {
            var model = new ProductIndexViewModel
            {
                CategoryNames = _context.Categories.Select(c => c.Name).ToList(),
                NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "",
                PriceSort = sortOrder == "Price" ? "price_desc" : "Price",
                CategorySort = category,
                CurrentFilter = searchString,
                Products = await _repository.GetProductsAsync(sortOrder, category, searchString)
            };

            return View(model);
        }

        public async Task<ActionResult> Editor(int? id, int? variantId, bool? addNewVariant = false)
        {
            var product = await _repository.GetProductAsync(id);
            var model = _mapper.Map<ProductEditorViewModel>(product);
            model.CurrentProductVariants = product.Variants.ToList();
            model.VariantButtonName = "Add Variant";
            
            if (variantId != null || addNewVariant == true)
                model.ShowVariantPage = "active";
            else model.ShowGeneralPage = "active";

            if (variantId != null)
            {
                var variant = await _repository.GetProductAsync(variantId);
                model.VariantId = variantId;
                model.VariantName = variant.Name;
                model.VariantPrice = variant.Price;
                model.VariantButtonName = "Save";
            } 

            PopulateNavigationData(model, product);

            return View(model);
        }


        private void PopulateNavigationData(ProductEditorViewModel model, Product product)
        {
            model.AvailableCategories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
                .ToList();
            model.SelectedCategories = product.Categories.Select(c => c.Id).ToList();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(ProductEditorViewModel model)
        {
            var product = await _repository.GetProductAsync(model.Id);
            if (!ModelState.IsValid)
            {
                PopulateNavigationData(model, product);
                return View(model);
            }
            product = _mapper.Map(model, product);
            await _repository.AddOrUpdateProduct(product, new HashSet<int>(model.SelectedCategories));
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> Delete(int id)
        {
            var productToDelete = await _repository.GetProductAsync(id);

            await _repository.DeleteAsync(id);

            if (productToDelete.ParentId != null) return RedirectToAction(nameof(Editor), new { id = productToDelete.ParentId });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateVariant(ProductEditorViewModel model)
        {
            var dbProduct = await _repository.GetProductAsync(model.VariantId);
            if (dbProduct == null)
                return HttpNotFound();
            
            dbProduct.Name = model.VariantName;
            dbProduct.Price = model.VariantPrice;

            await _repository.AddOrUpdateProduct(dbProduct, new HashSet<int>(model.SelectedCategories));

            return RedirectToAction(nameof(Editor), new { id = model.Id, variantId = dbProduct.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddVariant(ProductEditorViewModel model)
        {
            var variantName = $"{model.Name} {model.VariantName}";
            var product = new Product //existing product becomes new product instead
            {
                ParentId = model.Id,
                Name = variantName,
                Price = model.VariantPrice,
                Description = model.Description,
            };

            await _repository.AddOrUpdateProduct(product, new HashSet<int>(model.SelectedCategories));
            return RedirectToAction(nameof(Editor), new { id = model.Id, addNewVariant = true });
        }
    }
}