using System.Collections.Generic;
using E_commerce.Library;

namespace E_commercePIM.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<string> CategoryNames { get; set; } = new List<string>();
        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string CategorySort { get; set; }
        public string CurrentFilter { get; set; }
    }
}