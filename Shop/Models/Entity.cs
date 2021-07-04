using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; private set; }
    }
}