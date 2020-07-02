using System.Threading.Tasks;
using System.Web.Mvc;
using ApprovalTests.Asp;
using ApprovalTests.Asp.Mvc;
using ApprovalTests.Reporters;
using AutoMapper;
using E_commerce.Data;
using E_commercePIM.Controllers;
using E_commercePIM.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestHelpers;
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    [UseReporter(typeof(DiffReporter))]
    [Trait("Product", "Approval Tests E-commercePIM")]
    public class ProductsControllerApprovalTests : MvcTest
    {
        private IMapper _mapper;
        private readonly MapperForTests _mapConfig;

        public ProductsControllerApprovalTests()
        {
            _mapConfig = new MapperForTests();
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