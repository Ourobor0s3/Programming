using System.ComponentModel.DataAnnotations;

namespace Lab6_InventoryManager.Entities
{
    public class Product
    {
        [Key]
        public string ProductCode { get; set; }  = string.Empty;

        public string Name { get; set; }  = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}