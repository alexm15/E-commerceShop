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
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    [UseReporter(typeof(DiffReporter))]
    [Trait("Product", "Approval Tests E-commercePIM")]
    public class ProductsControllerApprovalTests : MvcTest
    {
        private IMapper _mapper;

        public ProductsControllerApprovalTests()
        {
            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            _mapper = config.CreateMapper();
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