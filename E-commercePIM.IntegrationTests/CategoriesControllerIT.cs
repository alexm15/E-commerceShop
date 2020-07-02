using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using E_commerce.Data;
using E_commerce.Library;
using E_commercePIM.Controllers;
using E_commercePIM.ViewModels;
using TestHelpers;
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    [Collection("Database Tests")]
    [Trait("Category IT", "Integration Tests E-commercePIM")]
    public class CategoriesControllerIT : IntegrationTestBase
    {
        private readonly DatabaseTestBase _dbBase;
        private CategoriesController _controller;


        public CategoriesControllerIT(DatabaseTestBase dbBase)
        {
            _dbBase = dbBase;
            _controller = new CategoriesController(new CategoryRepository(Context), _dbBase.Mapper, new ProductRepository(Context));
        }

        [Fact(DisplayName = "Index page returns list of category and product count on each category")]
        public async Task IndexPageCategories()
        {
            //See seeded data in Configuration class (called from super class)
            var model = await ControllerHelper.ExecuteActionAsync<CategoryIndexViewModel>(_controller.Index());

            var categoyData = model.Categories.ToList();
            Assert.Equal(4, categoyData.Count());
            var category = categoyData[0];
            Assert.Equal("Electronics", category.Name);
            Assert.Equal(2, category.ProductCount);

        }

        [Fact(DisplayName = "Editor page returns details of category and list of available product to add to category")]
        public async Task EditorPage()
        {
            //See seeded data in Configuration class (called from super class)
            var model = await ControllerHelper.ExecuteActionAsync<CategoryEditorViewModel>(_controller.Editor(1));

            Assert.Equal(1, model.Id);
            Assert.Equal("Electronics", model.Name);
            Assert.Equal(8, model.AvailableProducts.Count());
        }

        [Fact(DisplayName = "Update Category on editor page changes the details of the category")]
        public async Task UpdateEditorPage()
        {
            //See seeded data in Configuration class (called from super class)

            var viewModel = new CategoryEditorViewModel
            {
                Id = 1,
                Name = "Electronics Updated",
                ProductsInCategory = new[] { 1, 3, 4 }
            };

            await _controller.Editor(viewModel);

            var model = await ControllerHelper.ExecuteActionAsync<CategoryIndexViewModel>(_controller.Index());

            var category = model.Categories.ToList()[0];
            Assert.Equal("Electronics Updated", category.Name);
            Assert.Equal(3, category.ProductCount);
        }

        [Fact(DisplayName = "Creating category on editor page adds new product")]
        public async Task CreateEditorPage()
        {
            //See seeded data in Configuration class (called from super class)
            var viewModel = new CategoryEditorViewModel
            {
                Name = "New Category",
            };

            await _controller.Editor(viewModel);
            var model = await ControllerHelper.ExecuteActionAsync<CategoryIndexViewModel>(_controller.Index());

            Assert.Contains(model.Categories, c => c.Name.Equals("New Category"));
        }

        [Fact(DisplayName = "Delete category removes that category from app")]
        public async Task DeleteCategory()
        {
            //See seeded data in Configuration class (called from super class)
            var dbCategory = Context.Categories.FirstOrDefault(c => c.Name.Equals("Electronics"));
            Assert.NotNull(dbCategory);

            await _controller.Delete(dbCategory.Id);
            var model = await ControllerHelper.ExecuteActionAsync<CategoryIndexViewModel>(_controller.Index());

            Assert.Equal(3, model.Categories.Count());
            Assert.DoesNotContain(model.Categories, c => c.Name.Equals("Electronics"));
        }



    }
}