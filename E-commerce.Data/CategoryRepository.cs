using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using E_commerce.Library;

namespace E_commerce.Data
{
    public class CategoryRepository
    {
        private readonly WebshopContext _context;

        public CategoryRepository(WebshopContext context)
        {
            _context = context;
            _context.Configuration.ProxyCreationEnabled = false; //TODO understand this
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Include(c => c.Products).ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(int? id)
        {
            if (id == 0) return new Category();
            return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Category category = await GetCategoryAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateCategory(Category category, IEnumerable<int> selectedProductsIds)
        {
            if (category.Id == 0) _context.Categories.Add(category);

            foreach (var dbProduct in _context.Products)
            {
                if (selectedProductsIds.Contains(dbProduct.Id))
                {
                    if (!category.Products.Contains(dbProduct))
                    {
                        category.Products.Add(dbProduct);
                    }
                }
                else
                {
                    if (category.Products.Contains(dbProduct))
                    {
                        category.Products.Remove(dbProduct);
                    }
                }
            }
            await _context.SaveChangesAsync();

        }

        public int GetProductCount(int categoryId)
        {
            return _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == categoryId).Products.Count;
        }
    }
}