using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace TestHelpers
{
    public static class ControllerHelper
    {
        public static async Task<T> RunControllerAction<T>(Task<ActionResult> asyncActionResult)
        {
            var result = await asyncActionResult as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<T>(result.Model);
            return model;
        }
    }
}