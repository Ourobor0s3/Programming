using System.ComponentModel.DataAnnotations;

namespace Lab6_InventoryManager.Entities
{
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}