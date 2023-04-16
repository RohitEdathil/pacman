using System.ComponentModel.DataAnnotations;

namespace pacman.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public double Amount { get; set; }
    
    [Required]
    public bool isReceived { get; set; }
}