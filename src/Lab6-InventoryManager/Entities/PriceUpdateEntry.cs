namespace Lab6_InventoryManager.Entities
{
    public class PriceUpdateEntry
    {
        public string ProductCode { get; set; } = default!;

        public decimal NewPrice { get; set; }
    }
}