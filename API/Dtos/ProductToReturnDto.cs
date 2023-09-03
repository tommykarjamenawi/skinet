namespace API.Dtos
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } // decimal is a better choice than double for money
        public string PictureUrl { get; set; }
        public string ProductType { get; set; } 
        public string ProductBrand { get; set; } 
    }
}