using System.Threading.Tasks;
using System.Web.Mvc;

namespace Webshop.UI.Controllers
{
    public class CartController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToCart()
        {
            return null;
        }
    }
}