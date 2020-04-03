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
        public async Task IndexReturnsListOfProducts()
        {
            //Arrange
            var electronics = new Category {Name = "Electronics"};
            var books = new Category {Name = "Books"};
            var officeSupplies = new Category {Name = "Office Supplies"};
            var moviesAndTv = new Category {Name = "Movies and TV-Shows"};

            var data = new List<Product>
            {
                new Product
                {
                    Name = "ASUS X554L Laptop", Description = "Laptop computer", Price = 5499M,
                    Categories = new List<Category> {electronics}
                },
                new Product
                {
                    Name = "HP Pavillion XE455", Description = "Newest Laptop with high specs", Price = 9299M,
                    Categories = new List<Category> {electronics}
                },
                new Product
                {
                    Name = "Harry Potter and the Chamber of Secrects",
                    Description = "A Book by J.K Rowling Second Book", Price = 49M,
                    Categories = new List<Category> {books}
                },
                new Product
                {
                    Name = "Monty Python's Life of Brian", Description = "A Comedy movie", Price = 49M,
                    Categories = new List<Category> {moviesAndTv}
                },
                new Product
                {
                    Name = "W5 Glasses Wipes", Description = "For cleaning glasses", Price = 15M,
                    Categories = new List<Category> {officeSupplies}
                },
                new Product
                {
                    Name = "Terminator Genysis", Description = "The fifth movies in the Terminator Movie Series",
                    Price = 99M, Categories = new List<Category> {moviesAndTv}
                },
                new Product
                {
                    Name = "How I Met Your Mother Complete Series",
                    Description = "View all the loved episodes of the complete series", Price = 569M,
                    Categories = new List<Category> {moviesAndTv}
                },
                new Product
                {
                    Name = "The Day After Tommorrow",
                    Description = "A big climate change changes the whole world. But not for the better", Price = 128M,
                    Categories = new List<Category> {officeSupplies}
                }
            }.AsQueryable();

            var context = new WebshopContext {Products = DbMocker.MockDbSet(data)};

            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            var result = await controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
            Assert.Equal(8, model.Count());
        }

        [Fact]
        public async Task TestEditorPage()
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

            var context = new WebshopContext {Products = DbMocker.MockDbSet(productData), Categories = DbMocker.MockDbSet(categoryData)};

            var controller = new ProductsController(new ProductRepository(context), _mapper, context);

            var result = await controller.Editor(1) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<ProductEditorViewModel>(result.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("ASUS X554L Laptop", model.Name);
            Assert.Equal("Laptop computer", model.Description);
            Assert.Equal(5499M, model.Price);
            Assert.Equal(2, model.AvailableCategories.Count());
        }

        //[Fact]
        //public async Task TestEditorPageUpdate()
        //{
        //    //See seeded data in Configuration class (called from super class)

        //    var viewModel = new ProductEditorViewModel
        //    {
        //        Id = 1,
        //        Name = "ASUS Updated",
        //        SelectedCategories = new[] { 1, 3 }
        //    };

        //    await _productsController.Editor(viewModel);


        //    var result = await _productsController.Index() as ViewResult;
        //    Assert.NotNull(result);
        //    var model = Assert.IsAssignableFrom<IEnumerable<Product>>(result.Model);
        //    Assert.Equal(8, model.Count());
        //    var product = model.ToList()[0];
        //    Assert.Equal("ASUS Updated", product.Name);
        //    var productCategories = product.Categories.Select(c => c.Name).ToArray();
        //    Assert.Equal("Electronics,Office Supplies", string.Join(",", productCategories));
        //}

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