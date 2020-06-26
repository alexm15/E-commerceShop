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
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private IMapper _mapper;

        public ProductsControllerTests()
        {
            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();
        }


        [Fact]
        public async Task Index_returns_list_of_products_and_categories()
        {
            //Arrange
            var categories = new List<Category>
            {
                new Category {Name = "Electronics"},
                new Category {Name = "Movies and TV-Shows"}
            };
            var products = new List<Product>
            {
                new Product {Name = "ASUS X554L Laptop", Price = 5499M},
                new Product {Name = "HP Pavillion XE455", Price = 9299M},
                new Product {Name = "Monty Python's Life of Brian", Price = 49M},
                new Product {Name = "Terminator Genysis", Price = 99M},
                new Product {Name = "How I Met Your Mother Complete Series", Price = 569M},
            };
            var context = new WebshopContext
                {Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet(categories)};
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            Assert.Equal(5, model.Products.Count());
            Assert.Equal(2, model.CategoryNames.Count());
        }


        [Theory]
        [InlineData("Price", "Z", 10)]
        [InlineData("price_desc", "A", 20)]
        [InlineData(null, "A", 20)]
        [InlineData("name_desc", "Z", 10)]
        public async Task
            Index_returns_products_sorted_ascending_by_price_when_sortOrder_parameter_is_called_with_Price(
                string sortOrder, string nameOfFirstProduct, decimal priceOfFirstProduct)
        {
            var products = new List<Product>
            {
                new Product {Name = "Z", Price = 10m},
                new Product {Name = "A", Price = 20m},
            };
            var context = new WebshopContext
                {Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet<Category>()};
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(sortOrder, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            var productsFromModel = model.Products.ToList();
            Assert.Equal(nameOfFirstProduct, productsFromModel[0].Name);
            Assert.Equal(priceOfFirstProduct, productsFromModel[0].Price);
        }

        [Theory]
        [InlineData("CategoryB", 2, "A", 30)]
        [InlineData("CategoryA", 1, "Z", 10)]
        [InlineData(null, 3, "A", 30)]
        [InlineData("", 3, "A", 30)]
        public async Task Index_returns_products_filtered_by_category_when_category_is_used_as_input(string categoryQueryString, int numberOfProductsShown, string nameOfFirstProduct, decimal priceOfFirstProduct)
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
                {Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet(categories)};
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(null, categoryQueryString, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            Assert.Equal(numberOfProductsShown, model.Products.Count());
            var productsFromModel = model.Products.ToList();
            Assert.Equal(nameOfFirstProduct, productsFromModel[0].Name);
            Assert.Equal(priceOfFirstProduct, productsFromModel[0].Price);
        }


        [Fact]
        public async Task EditorPageReturnsProductAsProductEditorModelWhenValid_Id_IsUsed()
        {
            var electronics = new Category {Name = "Electronics"};
            var books = new Category {Name = "Books"};
            var categoryData = new List<Category> {electronics, books};

            var productData = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "ASUS X554L Laptop", Description = "Laptop computer", Price = 5499M,
                    Categories = new List<Category> {electronics}
                }
            };

            var context = new WebshopContext
                {Products = TestHelpers.MockDbSet(productData), Categories = TestHelpers.MockDbSet(categoryData)};

            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            var result = await controller.Editor(1, null) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductEditorViewModel>(result.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("ASUS X554L Laptop", model.Name);
            Assert.Equal("Laptop computer", model.Description);
            Assert.Equal(5499M, model.Price);
            Assert.Equal(2, model.AvailableCategories.Count());
        }

        [Fact]
        public async Task TestEditorPageUpdate()
        {
            var electronics = new Category {Id = 1, Name = "Electronics"};
            var books = new Category {Id = 2, Name = "Books"};
            var categoryData = new List<Category> {electronics, books};
            var productData = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "ASUS X554L Laptop", Description = "Laptop computer", Price = 5499M,
                    Categories = new List<Category>()
                }
            };
            var context = new WebshopContext
                {Products = TestHelpers.MockDbSet(productData), Categories = TestHelpers.MockDbSet(categoryData)};
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            var viewModel = new ProductEditorViewModel
            {
                Id = 1,
                Name = "ASUS Updated",
                SelectedCategories = new[] {1, 2} //CategoryIDs
            };

            await controller.Editor(viewModel);


            var result = await controller.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            var product = model.ToList()[0];
            Assert.Equal("ASUS Updated", product.Name);
            var productCategories = product.Categories.Select(c => c.Name).ToArray();
            Assert.Equal("Electronics,Books", string.Join(",", productCategories));
        }

        //[Fact]
        //public async Task TestEditorPageCreate()
        //{
        //    //See seeded data in Configuration class (called from super class)

        //    var viewModel = new ProductEditorViewModel
        //    {
        //        Name = "New Product",
        //    };

        //    await _productsController.Editor(viewModel);


        //    var result = await _productsController.Index() as ViewResult;
        //    Assert.NotNull(result);
        //    var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
        //    Assert.Equal(9, model.Count());
        //    Assert.Contains(model, p => p.Name.Equals("New Product"));
        //}

        //[Fact]
        //public async Task TestDelete()
        //{
        //    //See seeded data in Configuration class (called from super class)
        //    var dbProduct = _context.Products.FirstOrDefault(p => p.Name.Equals("ASUS X554L Laptop"));
        //    Assert.NotNull(dbProduct);
        //    await _productsController.Delete(dbProduct.Id);

        //    var result = await _productsController.Index() as ViewResult;
        //    Assert.NotNull(result);
        //    var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
        //    Assert.Equal(7, model.Count());
        //    Assert.DoesNotContain(model, p => p.Name.Equals("ASUS X554L Laptop"));
        //}
    }
}