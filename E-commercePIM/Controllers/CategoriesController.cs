using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using E_commerce.Data;

namespace E_commercePIM.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryRepository _repository;

        public CategoriesController(CategoryRepository repository)
        {
            _repository = repository;
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {

            return View(await _repository.GetCategoriesAsync());
        }

        public async Task<ActionResult> Editor(int id)
        {
            return View(await _repository.GetCategoryAsync(id));

        }
    }
}