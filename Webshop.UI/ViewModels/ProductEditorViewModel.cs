using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Webshop.UI.Controllers;
using Webshop.UI.Models;

namespace Webshop.UI.ViewModels
{
    public class ProductEditorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        public ICollection<Category> CurrentCategories { get; set; }
        public IList<AssignedCategoryData> AvailableCategories { get; set; }
    }
}