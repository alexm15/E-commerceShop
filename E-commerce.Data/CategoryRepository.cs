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

        public async Task AddOrUpdateCategory(Category category, IEnumerable<int> selectedProductsIds)
        {
            if (category.Id == 0) _context.Categories.Add(category);

            var dbProducts = _context.Products.ToList();
            UpdateCategoryProducts(category, selectedProductsIds, dbProducts);
            
            await _context.SaveChangesAsync();

        }

        //TODO: make this generic with UpdateProductCategories
        private void UpdateCategoryProducts(Category category, IEnumerable<int> selectedProductsIds, IEnumerable<Product> dbProducts)
        {
            foreach (var product in dbProducts)
            {
                if (selectedProductsIds.Contains(product.Id))
                {
                    if (!category.Products.Contains(product))
                    {
                        category.Products.Add(product);
                    }
                }
                else
                {
                    if (category.Products.Contains(product))
                    {
                        category.Products.Remove(product);
                    }
                }
            }
        }

        public int GetProductCount(int categoryId)
        {
            return _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == categoryId).Products.Count;
        }
    }
}