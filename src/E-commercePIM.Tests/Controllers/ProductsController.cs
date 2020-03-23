using System.Threading.Tasks;
using ApprovalTests.Asp;
using ApprovalTests.Asp.Mvc;
using ApprovalTests.Reporters;
using E_commercePIM.Controllers;
using Xunit;

namespace E_commercePIM.Tests.Controllers
{
    [UseReporter(typeof(DiffReporter))]
    public class ProductsController : MvcTest
    {
        [Fact]
        public void TestInitial()
        {
            MvcApprovals.VerifyMvcPage(new HomeController().Index);
        }

        [Fact]
        public async Task TestProductIndex()
        {
        }
    }

    public class MvcTest
    {
        public MvcTest()
        {
            PortFactory.MvcPort = 51037;
        }
    }
}