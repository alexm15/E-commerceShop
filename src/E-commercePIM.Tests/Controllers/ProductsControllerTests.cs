using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using E_commerce.Data;
using E_commerce.Library;
using E_commercePIM.Controllers;
using E_commercePIM.Mapping;
using E_commercePIM.Tests.Helpers;
using E_commercePIM.ViewModels;
using TestHelpers;
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    [Trait("Product", "Unit Tests E-commercePIM")]
    public class ProductsControllerTests
    {
        private readonly MapperForTests _mapperForTests;

        public ProductsControllerTests()
        {
            _mapperForTests = new MapperForTests();
        }


        [Theory(DisplayName = "Index can return products sorted in various ways ")]
        [InlineData("Price", "Z", 10)]
        [InlineData("price_desc", "A", 20)]
        [InlineData(null, "A", 20)]
        [InlineData("name_desc", "Z", 10)]
        public async Task
            IndexPageSortedProducts(
                string sortOrder, string nameOfFirstProduct, decimal priceOfFirstProduct)
        {
            var products = new List<Product>
            {
                new Product {Name = "Z", Price = 10m},
                new Product {Name = "A", Price = 20m},
            };
            var context = new WebshopContext
                {Products = TestHelper.MockDbSet(products), Categories = TestHelper.MockDbSet<Category>()};
            var controller = new ProductsController(new ProductRepository(context), _mapperForTests.Mapper, context);

            //Act
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(controller.Index(sortOrder, null, null));

            //Assert
            var productsFromModel = model.Products.ToList();
            Assert.Equal(nameOfFirstProduct, productsFromModel[0].Name);
            Assert.Equal(priceOfFirstProduct, productsFromModel[0].Price);
        }

        [Theory(DisplayName = "Index can return products filtered by selected category")]
        [InlineData("CategoryB", 2, "A", 30)]
        [InlineData("CategoryA", 1, "Z", 10)]
        [InlineData(null, 3, "A", 30)]
        [InlineData("", 3, "A", 30)]
        public async Task IndexPageFilteredCategories(string categoryQueryString, int numberOfProductsShown, string nameOfFirstProduct, decimal priceOfFirstProduct)
        {
            var categoryA = new Category {Name = "CategoryA"};
            var categoryB = new Category {Name = "CategoryB"};
            var categories = new List<Category> {categoryA, categoryB};
            var products = new List<Product>
            {
                new Product {Name = "Z", Price = 10m, Categories = new List<Category> {categoryA}},
                new Product {Name = "Q", Price = 20m, Categories = new List<Category> {categoryB}},
                new Product {Name = "A", Price = 30m, Categories = new List<Category> {categoryB}},
            };
            var context = new WebshopContext
                {Products = TestHelper.MockDbSet(products), Categories = TestHelper.MockDbSet(categories)};
            var controller = new ProductsController(new ProductRepository(context), _mapperForTests.Mapper, context);

            //Act
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(controller.Index(null, categoryQueryString, null));

            //Assert
            Assert.Equal(numberOfProductsShown, model.Products.Count());
            var productsFromModel = model.Products.ToList();
            Assert.Equal(nameOfFirstProduct, productsFromModel[0].Name);
            Assert.Equal(priceOfFirstProduct, productsFromModel[0].Price);
        }

    }
}