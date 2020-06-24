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
            var context = new WebshopContext {Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet(categories)};
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            Assert.Equal(5, model.Products.Count());
            Assert.Equal(2, model.CategoryNames.Count());
        }

        [Fact]
        public async Task Index_returns_products_sorted_ascending_when_called_with_all_parameters_null()
        {
            var products = new List<Product>
            {
                new Product {Name = "HP Pavillion XE455"},
                new Product {Name = "ASUS X554L Laptop"},
            };
            var context = new WebshopContext { Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet<Category>() };
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            var productsFromModel = model.Products.ToList();
            Assert.Equal("ASUS X554L Laptop", productsFromModel[0].Name);
        }

        [Fact]
        public async Task Index_returns_products_sorted_descending_when_sortOrder_parameter_is_called_with_namedesc()
        {
            var products = new List<Product>
            {
                new Product {Name = "HP Pavillion XE455"},
                new Product {Name = "ASUS X554L Laptop"},
            };
            var context = new WebshopContext { Products = TestHelpers.MockDbSet(products), Categories = TestHelpers.MockDbSet<Category>() };
            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            //Act
            var result = await controller.Index(null, null, null) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductIndexViewModel>(result.Model);

            //Assert
            var productsFromModel = model.Products.ToList();
            Assert.Equal("ASUS X554L Laptop", productsFromModel[0].Name);
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