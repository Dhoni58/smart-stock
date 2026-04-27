namespace WarehouseSystem.Models;

public enum MovementType
{
    Receipt,
    Issue
}

public class WarehouseMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; } 
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Audit zaměstnanců
    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
}