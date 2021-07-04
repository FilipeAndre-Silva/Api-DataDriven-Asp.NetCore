using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Product : Entity
    {
        public Product(string title, string description, decimal price, int categoryId)
        {
            Title = title;
            Description = description;
            Price = price;
            CategoryId = categoryId;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}