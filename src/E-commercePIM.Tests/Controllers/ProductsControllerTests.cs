using System.Threading.Tasks;
using System.Web.Mvc;
using ApprovalTests.Asp;
using ApprovalTests.Asp.Mvc;
using ApprovalTests.Reporters;
using AutoMapper;
using E_commerce.Data;
using E_commercePIM.Controllers;
using E_commercePIM.Mapping;
using Moq;
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    [UseReporter(typeof(DiffReporter))]
    public class ProductsControllerTests : MvcTest
    {
        private IMapper _mapper;

        public ProductsControllerTests()
        {
            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public void TestInitial()
        {
            MvcApprovals.VerifyMvcPage(new HomeController().Index);
        }

        [Fact]
        public async Task TestProductIndex()
        {
            var productRepo = new Mock<ProductRepository>();
            var controller = new ProductsController(productRepo.Object, _mapper, new WebshopContext());

            var result = await controller.Index() as ViewResult;

            Assert.NotNull(result);
        }
    }

    public class MvcTest
    {
        protected MvcTest()
        {
            PortFactory.MvcPort = 51037;
        }
    }
}