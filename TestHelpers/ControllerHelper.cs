using System.Threading.Tasks;
using System.Web.Mvc;
using Xunit;

namespace TestHelpers
{
    public static class ControllerHelper
    {
        public static async Task<T> ExecuteActionAsync<T>(Task<ActionResult> asyncActionToExecute)
        {
            return ExecuteAction<T>(await asyncActionToExecute);
        }

        public static T ExecuteAction<T>(ActionResult actionToExecute)
        {
            var result = actionToExecute as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<T>(result.Model);
            return model;
        }
    }
}