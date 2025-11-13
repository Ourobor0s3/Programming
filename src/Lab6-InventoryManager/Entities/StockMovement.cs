using System.ComponentModel.DataAnnotations;

namespace Lab6_InventoryManager.Entities
{
    public class StockMovement
    {
        [Key]
        public int Id { get; set; }

        public string? ProductCode { get; set; }

        public int FromWarehouseId { get; set; }

        public int ToWarehouseId { get; set; }

        public int Quantity { get; set; }

        public DateTime When { get; set; }

        public Product? Product { get; set; }

        public Warehouse? FromWarehouse { get; set; }

        public Warehouse? ToWarehouse { get; set; }
    }
}