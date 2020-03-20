using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace E_commercePIM.ViewModels
{
    public class ProductEditorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public IEnumerable<int> SelectedCategories { get; set; }
        public IEnumerable<SelectListItem> AvailableCategories { get; set; }
    }
}