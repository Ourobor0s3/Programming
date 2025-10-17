using System.ComponentModel.DataAnnotations;

namespace Lab5_CostAccounting
{
    public class Transaction
    {

        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Category { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}