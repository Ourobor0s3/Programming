namespace Lab6_InventoryManager.Entities
{
    public class PriceHistory
    {
        public long Id { get; set; }

        public string ProductCode { get; set; } = default!;

        public decimal OldPrice { get; set; }

        public decimal NewPrice { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string? Reason { get; set; }
    }
}