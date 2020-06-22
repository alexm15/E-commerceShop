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
            if (id == 0) return new Product();
            _context.Configuration.ProxyCreationEnabled = false;
            var dbProduct = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);
            return dbProduct;
        }

        public async Task AddOrUpdateProduct(Product product, ICollection<int> selectedCategoryIds)
        {
            if (product.Id == 0) _context.Products.Add(product);

            //TODO: try to clear list and then add a new instead.
            var allCategoriesFromDB = await _context.Categories.ToListAsync();
            UpdateProductCategories(product, selectedCategoryIds, allCategoriesFromDB);

            if (product.ParentId == null)
            {
                foreach (var variant in product.Variants.ToList())
                {
                    variant.Description = product.Description;
                    UpdateProductCategories(variant, selectedCategoryIds, allCategoriesFromDB);
                }
            }

            await _context.SaveChangesAsync();
        }

        private void UpdateProductCategories(Product product, ICollection<int> selectedCategoryIds, IEnumerable<Category> dbCateogries)
        {
            foreach (var category in dbCateogries)
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

        public async Task CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}