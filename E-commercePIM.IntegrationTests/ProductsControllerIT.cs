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
using Xunit;

namespace E_commercePIM.IntegrationTests
{
    public class ProductsControllerIT : IntegrationTestBase
    {
        private ProductsController _productsController;

        public ProductsControllerIT()
        {
            _productsController = new ProductsController(new ProductRepository(_context), _mapper, _context);
        }


        [Fact]
        public async Task TestIndexPage()
        {
            //See seeded data in Configuration class (called from super class)
            
            var result = await _productsController.Index(null, null, null) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            Assert.Equal(8, model.Count());
        }

        [Fact]
        public async Task TestEditorPage()
        {
            //See seeded data in Configuration class (called from super class)

            var result = await _productsController.Editor(1) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductEditorViewModel>(result.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("ASUS X554L Laptop", model.Name);
            Assert.Equal("Laptop computer", model.Description);
            Assert.Equal(5499M, model.Price);
            Assert.Equal(4, model.AvailableCategories.Count());
        }

        [Fact]
        public async Task TestEditorPageUpdate()
        {
            //See seeded data in Configuration class (called from super class)

            var viewModel = new ProductEditorViewModel
            {
                Id = 1,
                Name = "ASUS Updated",
                SelectedCategories = new[] {1, 3}
            };
            
            await _productsController.Editor(viewModel);


            var result = await _productsController.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            Assert.Equal(8, model.Count());
            var product = model.ToList()[0];
            Assert.Equal("ASUS Updated", product.Name);
            var productCategories = product.Categories.Select(c => c.Name).ToArray();
            Assert.Equal("Electronics,Office Supplies", string.Join(",", productCategories));
        }

        [Fact]
        public async Task TestEditorPageCreate()
        {
            //See seeded data in Configuration class (called from super class)

            var viewModel = new ProductEditorViewModel
            {
                Name = "New Product",
            };

            await _productsController.Editor(viewModel);


            var result = await _productsController.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            Assert.Equal(9, model.Count());
            Assert.Contains(model, p => p.Name.Equals("New Product"));
        }

        [Fact]
        public async Task TestDelete()
        {
            //See seeded data in Configuration class (called from super class)
            var dbProduct = _context.Products.FirstOrDefault(p => p.Name.Equals("ASUS X554L Laptop"));
            Assert.NotNull(dbProduct);
            await _productsController.Delete(dbProduct.Id);

            var result = await _productsController.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            Assert.Equal(7, model.Count());
            Assert.DoesNotContain(model, p => p.Name.Equals("ASUS X554L Laptop"));
        }


    }
}