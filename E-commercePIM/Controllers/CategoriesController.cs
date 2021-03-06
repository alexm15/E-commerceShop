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
using WebGrease.Css.Extensions;

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
            var categories = await _categoryRepository.GetCategoriesAsync();
            var model = new CategoryIndexViewModel
            {
                Categories = _mapper.Map<List<CategoryDataViewModel>>(categories)
            };
            model.Categories.ForEach(c => c.ProductCount = _categoryRepository.GetProductCount(c.Id));

            return View(model);
        }

        public async Task<ActionResult> Editor(int id)
        {
            var category = await _categoryRepository.GetCategoryAsync(id);
            var model = _mapper.Map<CategoryEditorViewModel>(category);
            PopulateNavigationData(model, category);
            model.ProductsInCategory = category.Products.Select(p => p.Id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Editor(CategoryEditorViewModel model)
        {
            var category = await _categoryRepository.GetCategoryAsync(model.Id);
            if (!ModelState.IsValid)
            {
                PopulateNavigationData(model, category);
                return View(model);
            }
            category = _mapper.Map(model, category);
            await _categoryRepository.AddOrUpdateCategory(category, new HashSet<int>(model.ProductsInCategory));
            return RedirectToAction(nameof(Index));
        }

        private void PopulateNavigationData(CategoryEditorViewModel model, Category category)
        {
            model.AvailableProducts = _productRepository.GetProductsAsync().Result
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name,
                })
                .ToList();
            model.ProductsInCategory = category.Products.Select(p => p.Id);
        }

        public async Task<ActionResult> Delete(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}