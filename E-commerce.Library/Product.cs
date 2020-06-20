using System.Collections.Generic;

namespace E_commerce.Library
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public int? ParentId { get; set; }
        public virtual ICollection<Product> Variants { get; set; } = new List<Product>();
        public Product ParentProduct { get; set; }
    }
}
