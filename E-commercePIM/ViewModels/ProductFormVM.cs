using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using E_commerce.Library;
using E_commercePIM.Controllers;

namespace E_commercePIM.ViewModels
{
    public class ProductFormVM
    {
        public int Id { get; set; }

        public string Heading { get; set; }

        public ProductVM Product { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem> AvailableCategories { get; set; } = new List<SelectListItem>();

        [Display(Name = "Variant Name")]
        public string VariantName { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal VariantPrice { get; set; }
        
        public IEnumerable<Product> CurrentProductVariants { get; set; } = new List<Product>();
        public IEnumerable<ProductVM> CurrentProductVariants2 { get; set; } = new List<ProductVM>();
        public string ShowVariantPage { get; set; }
        public string ShowGeneralPage { get; set; }
        public string VariantButtonName { get; set; } = "Add Variant";
        public int? VariantId { get; set; }
        public bool EditMode { get; set; }

        public string Action 
        {
            get
            {
                Expression<Func<ProductsController, Task<ActionResult>>> edit = 
                    (c => c.Edit(this));
                Expression<Func<ProductsController, Task<ActionResult>>> create = 
                    (c => c.Create(this));
                var action = (Id == 0) ? create : edit;

                return (action.Body as MethodCallExpression)?.Method.Name;
            }
        }
    }

    public class ProductVM
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}