using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using E_commerce.Data;
using E_commerce.Library;
using E_commercePIM.Controllers;
using E_commercePIM.ViewModels;
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    public class CategoriesControllerIT : IntegrationTestBase
    {
        private CategoriesController _controller;

        public CategoriesControllerIT()
        {
            _controller = new CategoriesController(new CategoryRepository(_context), _mapper, new ProductRepository(_context));
        }

        [Fact]
        public async Task TestIndex()
        {
            //See seeded data in Configuration class (called from super class)
            
            var result = await _controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<CategoryIndexViewModel>(result.Model);
            var categoyData = model.Categories.ToList();
            Assert.Equal(4, categoyData.Count());
            var category = categoyData[0];
            Assert.Equal("Electronics", category.Name);
            Assert.Equal(2, category.ProductCount);
        }

        [Fact]
        public async Task TestEditorPage()
        {
            //See seeded data in Configuration class (called from super class)

            var result = await _controller.Editor(1) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<CategoryEditorViewModel>(result.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("Electronics", model.Name);
            Assert.Equal(8, model.AvailableProducts.Count());
        }

        [Fact]
        public async Task TestEditorPageUpdate()
        {
            //See seeded data in Configuration class (called from super class)

            var viewModel = new CategoryEditorViewModel
            {
                Id = 1,
                Name = "Electronics Updated",
                ProductsInCategory = new[] { 1, 3, 4 }
            };

            await _controller.Editor(viewModel);


            var result = await _controller.Index() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<CategoryIndexViewModel>(result.Model);
            var category = model.Categories.ToList()[0];
            Assert.Equal("Electronics Updated", category.Name);
            Assert.Equal(3, category.ProductCount);
        }

        [Fact]
        public async Task TestEditorPageCreate()
        {
            //See seeded data in Configuration class (called from super class)
            var viewModel = new CategoryEditorViewModel
            {
                Name = "New Category",
            };

            await _controller.Editor(viewModel);

            var result = await _controller.Index() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<CategoryIndexViewModel>(result.Model);
            Assert.Contains(model.Categories, c => c.Name.Equals("New Category"));
        }

        [Fact]
        public async Task TestDelete()
        {
            //See seeded data in Configuration class (called from super class)
            var dbCategory = _context.Categories.FirstOrDefault(c => c.Name.Equals("Electronics"));
            Assert.NotNull(dbCategory);
            
            await _controller.Delete(dbCategory.Id);

            var result = await _controller.Index() as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<CategoryIndexViewModel>(result.Model);
            Assert.Equal(3, model.Categories.Count());
            Assert.DoesNotContain(model.Categories, c => c.Name.Equals("Electronics"));
        }



    }
}