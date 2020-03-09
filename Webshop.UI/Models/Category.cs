using System.Collections.Generic;

namespace Webshop.UI.Models
{
    public class Category
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; } = new List<Product>();
    }
}
