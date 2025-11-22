namespace Lab6_InventoryManager.Entities
{
    public class WarehouseStock
    {
        public WarehouseStock()
        {
        }

        public WarehouseStock(
            int warehouseId,
            string warehouseName,
            string productCode,
            string productName,
            int quantity)
        {
            WarehouseId = warehouseId;
            WarehouseName = warehouseName;
            ProductCode = productCode;
            ProductName = productName;
            Quantity = quantity;
        }

        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}