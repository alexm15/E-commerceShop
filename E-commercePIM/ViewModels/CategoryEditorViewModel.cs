using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using E_commerce.Library;

namespace E_commercePIM.ViewModels
{
    public class CategoryEditorViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<int> ProductsInCategory { get; set; }
        public IEnumerable<SelectListItem> AvailableProducts { get; set; }
    }
}