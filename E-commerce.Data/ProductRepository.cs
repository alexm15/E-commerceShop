using System.Collections.Generic;
using System.Data.Entity;
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
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int? id)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var dbProduct = await _context.Products.Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);
            return dbProduct;
        }

        public async Task UpdateProduct(Product dbProduct, ICollection<int> selectedCategoryIds)
        {
            foreach (var dbCategory in _context.Categories)
            {
                if (selectedCategoryIds.Contains(dbCategory.Id))
                {
                    if (!dbProduct.Categories.Contains(dbCategory))
                    {
                        dbProduct.Categories.Add(dbCategory);
                    }
                }
                else
                {
                    if (dbProduct.Categories.Contains(dbCategory))
                    {
                        dbProduct.Categories.Remove(dbCategory);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}