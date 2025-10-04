using System.ComponentModel.DataAnnotations;

public class Book
{
    [Key]
    [MaxLength(32)]
    public string ISBN { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    public int? Year { get; set; }
    public int? Pages { get; set; }
}