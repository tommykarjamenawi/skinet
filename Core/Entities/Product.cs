namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } // decimal is a better choice than double for money
        public string PictureUrl { get; set; }
        public ProductType ProductType { get; set; } // navigation property
        public int ProductTypeId { get; set; } // foreign key
        public ProductBrand ProductBrand { get; set; } // navigation property
        public int ProductBrandId { get; set; } // foreign key
    }
}