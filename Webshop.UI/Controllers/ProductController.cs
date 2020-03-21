using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using AutoMapper;
using WebGrease.Css.Extensions;
using Webshop.UI.ViewModels;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_commerce.Data;
using E_commerce.Library;

namespace Webshop.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext _context;
        private IMapper _mapper;
        private readonly ProductRepository _repo;

        public ProductController(WebshopContext context, IMapper mapper, ProductRepository repo)
        {
            _context = context;
            _mapper = mapper;
            _repo = repo;
        }

        // GET: Product
        public async Task<ActionResult> Index()
        {
            return View(await _repo.GetProductsAsync());
        }



        // GET: Product/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await _repo.GetProductAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }


            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Price")]
            Product product)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(product);
                return RedirectToAction("Index");
            }

            return View(product);
        }



        // GET: Product/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = await _repo.GetProductAsync(id);
            if (product == null) return HttpNotFound();
            
            var viewModel = _mapper.Map<ProductEditorViewModel>(product);

            viewModel.AvailableCategories = _context.Categories.Include(category => category.Products)
                .ProjectTo<AssignedCategoryData>(_mapper.ConfigurationProvider, new {currentProduct = product})
                .ToList();

            return View(viewModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductEditorViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbProduct = await _repo.GetProductAsync(model.Id);
            if (dbProduct == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            dbProduct.Id = model.Id;
            dbProduct.Name = model.Name;
            dbProduct.Description = model.Description;
            dbProduct.Price = model.Price;

            var selectedCategoryIds = new HashSet<int>(model.AvailableCategories.Where(c => c.Assigned).Select(c => c.Id));
            await _repo.AddOrUpdateProduct(dbProduct, selectedCategoryIds);

            
            return RedirectToAction("Index");
        }



        

        // GET: Product/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = await _repo.GetProductAsync(id);
            if (product == null) return HttpNotFound();

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}