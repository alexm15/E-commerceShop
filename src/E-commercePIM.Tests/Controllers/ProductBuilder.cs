using E_commerce.Library;

namespace E_commercePIM.Tests.Controllers
{
    public class ProductBuilder
    {
        private readonly Product _product = new Product();

        public ProductBuilder WithName(string name)
        {
            _product.Name = name;
            return this;
        }

        public ProductBuilder WithPrice(decimal price)
        {
            _product.Price = price;
            return this;
        }

        public ProductBuilder WithDescription(string description)
        {
            _product.Description = description;
            return this;
        }

        public ProductBuilder WithVariant(int variantId, string variantName, decimal variantPrice)
        {
            var variant = new Product
            {
                Id = variantId, Name = variantName, Price = variantPrice, ParentId = _product.Id,
                ParentProduct = _product
            };
            _product.Variants.Add(variant);
            return this;
        }

        public ProductBuilder WithVariant(Product variant)
        {
            variant.ParentId = _product.Id;
            variant.ParentProduct = _product;

            _product.Variants.Add(variant);
            return this;
        }

        public ProductBuilder SimpleLaptop(int id, string name = "Laptop", decimal price = 4000m,
            string description = "This is a simple test laptop")
        {
            return WithId(id).WithName(name).WithPrice(price).WithDescription(description);
        }

        public ProductBuilder SimpleMovie(int id, string name = "Some Movie", decimal price = 120m,
            string description = "This is some kind of movie")
        {
            return WithId(id).WithName(name).WithPrice(price).WithDescription(description);
        }


        public ProductBuilder WithId(int id)
        {
            _product.Id = id;
            return this;
        }


        public Product Build()
        {
            if (_product.Id == 0) _product.Id = 1;
            return _product;
        }
    }
}