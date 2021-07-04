using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public class Category : Entity
    {
        public Category(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
    }
}