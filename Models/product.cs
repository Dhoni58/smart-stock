namespace WarehouseSystem.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int WarehouseInv { get; set; }
    public int MinimumInv { get; set; }
    public DateTime Createdat { get; set; } = DateTime.Now;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }


public bool CanIssue(int quantity) =>
    WarehouseInv >= quantity;

/// <summary>
/// Aplikuje pohyb a aktualizuje stav skladu
/// Při výdeji se ověřří dostupnost zboží.
/// </summary>
public void ApplyMovement(MovementType type, int quantity)
    {
        if (type == MovementType.Receipt)
            WarehouseInv += quantity;
        else
            WarehouseInv -= quantity;
    }
}