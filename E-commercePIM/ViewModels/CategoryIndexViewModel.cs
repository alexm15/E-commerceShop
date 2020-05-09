using System.Collections.Generic;

namespace E_commercePIM.ViewModels
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<CategoryDataViewModel> Categories { get; set; } = new List<CategoryDataViewModel>();
    }
}