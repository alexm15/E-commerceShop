using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            var controller = CreateProductController(products, new List<Category>());


            //Act
            var model =
                await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                    controller.Index(sortOrder, null, null));

            //Assert
            var productsFromModel = model.Products.ToList();
            AssertNameAndPrice(nameOfFirstProduct, priceOfFirstProduct, productsFromModel[0]);
        }


        private static void AssertNameAndPrice(string expectedName, decimal expectedPrice, Product actualProduct)
        {
            Assert.Equal(expectedName, actualProduct.Name);
            Assert.Equal(expectedPrice, actualProduct.Price);
        }

        [Theory(DisplayName = "Index can return products filtered by selected category")]
        [InlineData("CategoryB", 2, "A", 30)]
        [InlineData("CategoryA", 1, "Z", 10)]
        [InlineData(null, 3, "A", 30)]
        [InlineData("", 3, "A", 30)]
        public async Task IndexPageFilteredCategories(string categoryQueryString, int numberOfProductsShown,
            string nameOfFirstProduct, decimal priceOfFirstProduct)
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
            var controller = CreateProductController(products, categories);

            //Act
            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                controller.Index(null, categoryQueryString, null));

            //Assert
            Assert.Equal(numberOfProductsShown, model.Products.Count());
            var productsFromModel = model.Products.ToList();
            AssertNameAndPrice(nameOfFirstProduct, priceOfFirstProduct, productsFromModel[0]);
        }

        [Fact(DisplayName = "Index page only shows parent products")]
        public async Task IndexPageOnlyParentProducts()
        {
            IList<Product> products = new List<Product>
            {
                new Product {Id = 1, Name = "A", Price = 20m},
                new Product {Id = 2, Name = "A Variant", Price = 50m, ParentId = 1},
            };
            var controller = CreateProductController(products, new List<Category>());


            var model = await ControllerHelper.ExecuteActionAsync<ProductIndexViewModel>(
                controller.Index(null, null, null));

            Assert.Equal(1, model.Products.Count());
        }

        [Fact(DisplayName = "Editor page where variantId is null shows general tab")]
        public async Task EditorPageGeneralTab()
        {
            IList<Product> products = new List<Product> {new Product {Id = 1},};
            var controller = CreateProductController(products, new List<Category>());


            //See seeded data in Configuration class (called from super class)
            var model = await ControllerHelper.ExecuteActionAsync<ProductEditorViewModel>(
                controller.Editor(1, null));

            Assert.Equal("Add Variant", model.VariantButtonName);
            Assert.Null(model.ShowVariantPage);
            Assert.Equal("active", model.ShowGeneralPage);
        }

        [Fact(DisplayName = "Editor page where variantId has value shows variant tab with details")]
        public async Task EditorPageVariantTab()
        {
            var product = new ProductBuilder()
                    .WithName("A")
                    .WithPrice(20m)
                    .WithVariant(2, "A Variant", 50m)
                    .Build();
            var parentProduct = new Product {Id = 1, Name = "A", Price = 20m};
            var productVariant = new Product
                {Id = 2, Name = "A Variant", Price = 50m, ParentId = 1, ParentProduct = parentProduct};
            parentProduct.Variants = new List<Product> {productVariant};
            IList<Product> products = new List<Product>
            {
                parentProduct,
                productVariant,
            };
            var controller = CreateProductController(products, new List<Category>());


            var model = await ControllerHelper.ExecuteActionAsync<ProductEditorViewModel>(
                controller.Editor(1, 2));

            Assert.Equal("Save", model.VariantButtonName);
            Assert.Equal("active", model.ShowVariantPage);
            Assert.Null(model.ShowGeneralPage);

            Assert.Equal(2, model.VariantId);
            Assert.Equal("A Variant", model.VariantName);
            Assert.Equal(50m, model.VariantPrice);
        }

        private ProductsController CreateProductController(IList<Product> products, IList<Category> categories)
        {
            var context = new WebshopContext
                {Products = TestHelper.MockDbSet(products), Categories = TestHelper.MockDbSet(categories)};
            var controller = new ProductsController(new ProductRepository(context), _mapperForTests.Mapper, context);
            return controller;
        }
    }

    public class ProductBuilder
    {
        private readonly Product _product = new Product();

        public ProductBuilder WithName(string name)
        {
            _product.Name = name;
            return this;
        }

        public ProductBuilder WithPrice(decimal price)
        {
            _product.Price = price;
            return this;
        }

        public ProductBuilder WithDescription(string description)
        {
            _product.Description = description;
            return this;
        }

        public ProductBuilder WithVariant(int variantId, string variantName, decimal variantPrice)
        {
            var variant = new Product
            {
                Id = variantId, Name = variantName, Price = variantPrice, ParentId = _product.Id,
                ParentProduct = _product
            };
            _product.Variants.Add(variant);
            return this;
        }

        public ProductBuilder SimpleLaptop(int id, string name = "Laptop", decimal price = 4000m,
            string description = "This is a simple test laptop")
        {
            return WithId(id).WithName(name).WithPrice(price).WithDescription(description);
        }

        public ProductBuilder WithId(int id)
        {
            _product.Id = id;
            return this;
        }

        public ProductBuilder SimpleLaptopWithVariants(int parentId, int variantId, string variantName, decimal variantPrice)
        {
            return SimpleLaptop(parentId).WithVariant(variantId, variantName, variantPrice);
        }

        public Product Build()
        {
            return _product;
        }
    }
}