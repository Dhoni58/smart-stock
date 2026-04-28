namespace WarehouseSystem.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Ico { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Note { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<WarehouseMovement> WarehouseMovements { get; set; } = new();
}