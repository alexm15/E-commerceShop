using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Library;

namespace E_commerce.Data
{
    public class ProductRepository
    {
        private readonly WebshopContext _context;

        public ProductRepository(WebshopContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (product == null)
            {
                throw new ObjectNotFoundException($"The requested product with id:{id} could not be found in DB");
            }
            foreach (var variant in product.Variants.ToList())
            {
                _context.Products.Remove(variant);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public Task<List<Product>> GetProductsAsync(string sortQuery = null, string category = null,
            string searchString = null)
        {
            var products = from p in _context.Products
                where p.ParentId == null
                select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Categories.Any(c => c.Name == category));
            }

            switch (sortQuery)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            return products.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int? id)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var dbProduct = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Variants.Select(v => v.Categories)) //TODO: consider if this big join statement is better than multiple simple queries.
                .FirstOrDefaultAsync(p => p.Id == id);
            return dbProduct;
        }

        public async Task UpdateProduct(Product product, ICollection<int> selectedCategoryIds)
        {
            await UpdateProductRelations(product, selectedCategoryIds);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateProductRelations(Product product, ICollection<int> selectedCategoryIds)
        {
            //TODO: try to clear list and then add a new instead.
            var allCategories = await _context.Categories.ToListAsync();
            UpdateProductCategories(product, selectedCategoryIds, await _context.Categories.ToListAsync());

            if (IsParent(product))
            {
                product.Variants
                    .ToList()
                    .ForEach(variant => UpdateProductCategories(variant, selectedCategoryIds, allCategories));
            }
        }
        private static bool IsParent(Product product) => product.ParentId == null && product.Variants.Count > 0;

        //TODO: make this generic with UpdateCategoryProducts
        private void UpdateProductCategories(Product product, ICollection<int> selectedCategoryIds, IEnumerable<Category> dbCategories)
        {
            foreach (var category in dbCategories)
            {
                if (selectedCategoryIds.Contains(category.Id))
                {
                    if (!product.Categories.Contains(category))
                    {
                        product.Categories.Add(category);
                    }
                }
                else
                {
                    if (product.Categories.Contains(category))
                    {
                        product.Categories.Remove(category);
                    }
                }
            }
        }

        public async Task AddProduct(Product product, ICollection<int> selectedCategoryIds)
        {
            _context.Products.Add(product);
            await UpdateProductRelations(product, selectedCategoryIds);
            await _context.SaveChangesAsync();
        }
    }
}