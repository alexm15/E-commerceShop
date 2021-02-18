using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using E_commerce.Data;
using E_commerce.Library;
using E_commercePIM.Controllers;
using E_commercePIM.Mapping;
using E_commercePIM.ViewModels;
using TestHelpers;
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    [Collection("Database Tests")]
    [Trait("Product IT", "Integration Tests E-commercePIM")]
    public class ProductsControllerIT : IntegrationTestBase
    {
        private readonly DatabaseTestBase _dbBase;
        private readonly ProductsController _productsController;

        public ProductsControllerIT(DatabaseTestBase dbBase)
        {
            _dbBase = dbBase;
            _productsController = new ProductsController(new ProductRepository(Context), _dbBase.Mapper, Context);
        }

        [Fact(DisplayName = "Index Page contains list of products")]
        public async Task IndexPage()
        {
            //See seeded data in Configuration class (called from super class)
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                    _productsController.Index(null, null, null));

            Assert.Equal(8, model.Products.Count());
        }

        [Fact(DisplayName = "Edit page shows details of product and both selected and available categories")]
        public async Task ShowEditorPage()
        {
            //See seeded data in Configuration class (called from super class)
            var model = await ControllerHelper.ExecuteActionAsync<ProductFormVM>(
                _productsController.Edit(1,null));

            Assert.Equal(1, model.Id);
            Assert.Equal("ASUS X554L Laptop", model.Name);
            Assert.Equal("Laptop computer", model.Description);
            Assert.Equal(5499M, model.Price);
            Assert.Equal(4, model.AvailableCategories.Count());
            Assert.Equal(1, model.SelectedCategories.Count());
        }



        [Fact(DisplayName = "Update on EditorPage updates details of product")]
        public async Task UpdateProductEditorPage()
        {
            //See seeded data in Configuration class (called from super class)
            var viewModel = new ProductFormVM
            {
                Id = 1,
                Name = "ASUS Updated",
                SelectedCategories = new[] {1, 3}
            };
            
            await _productsController.Edit(viewModel);
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                _productsController.Index(null, null, null));

            Assert.Equal(8, model.Products.Count());
            var product = model.Products.ToList()[0];
            Assert.Equal("ASUS Updated", product.Name);
            var productCategories = product.Categories.Select(c => c.Name).ToArray();
            Assert.Equal("Electronics,Office Supplies", string.Join(",", productCategories));
        }

        [Fact(DisplayName = "Create Product on editor page adds new product")]
        public async Task CreateProductWithEditorPage()
        {
            //See seeded data in Configuration class (called from super class)
            var viewModel = new ProductFormVM
            {
                Name = "New Product",
            };

            await _productsController.Edit(viewModel);
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                _productsController.Index(null, null, null));

            Assert.Equal(9, model.Products.Count());
            Assert.Contains(model.Products, p => p.Name.Equals("New Product"));
        }

        [Fact(DisplayName = "Add Product Variant adds new product to DB and adds relationship")]
        public async Task AddProductVariant()
        {
            //See seeded data in Configuration class (called from super class)
            var viewModel = new ProductFormVM
            {
                Name = "New Product",
            };

            await _productsController.Edit(viewModel);
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                _productsController.Index(null, null, null));

            Assert.Equal(9, model.Products.Count());
            Assert.Contains(model.Products, p => p.Name.Equals("New Product"));
        }

        [Fact(DisplayName = "Delete removes product from app")]
        public async Task DeleteProductPage()
        {
            //See seeded data in Configuration class (called from super class)
            var dbProduct = Context.Products.FirstOrDefault(p => p.Name.Equals("ASUS X554L Laptop"));
            Assert.NotNull(dbProduct);
            await _productsController.Delete(dbProduct.Id);

            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                _productsController.Index(null, null, null));

            Assert.Equal(7, model.Products.Count());
            Assert.DoesNotContain(model.Products, p => p.Name.Equals("ASUS X554L Laptop"));
        }


    }
}