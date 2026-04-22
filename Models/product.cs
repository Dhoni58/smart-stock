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
}