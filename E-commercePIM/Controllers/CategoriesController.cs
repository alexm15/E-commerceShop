﻿using System;
using System.Collections.Generic;
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
    public class CategoriesController : Controller
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ProductRepository _productRepository;

        public CategoriesController(CategoryRepository categoryRepository, IMapper mapper, ProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            return View(await _categoryRepository.GetCategoriesAsync());
        }

        public async Task<ActionResult> Editor(int id)
        {
            var category = await _categoryRepository.GetCategoryAsync(id);
            var model = _mapper.Map<CategoryEditorViewModel>(category);
            model.AvailableProducts = PopulateProducts(category);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(CategoryEditorViewModel model)
        {
            var category = await _categoryRepository.GetCategoryAsync(model.Id);
            if (!ModelState.IsValid)
            {
                model.AvailableProducts = PopulateProducts(category);
                return View(model);
            }
            category = _mapper.Map(model, category);
            await _categoryRepository.AddOrUpdateCategory(category, new HashSet<int>(model.ProductsInCategory));
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SelectListItem> PopulateProducts(Category category)
        {
            var products = _productRepository.GetProductsAsync().Result;
            return products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                    Selected = p.Categories.Contains(category)
                })
                .ToList();
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}