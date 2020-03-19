using System.Collections.Generic;
using System.Data.Entity;
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
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Include(c => c.Products).ToListAsync();
        }

        public Task<Category> GetCategoryAsync(int? id)
        {
            return _context.Categories.FindAsync(id);
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
    }
}