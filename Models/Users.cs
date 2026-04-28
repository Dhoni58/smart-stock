namespace WarehouseSystem.Models;

public enum UserRole
{
    Skladnik,
    Vedouci
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public List<WarehouseMovement> WarehouseMovements { get; set; } = new();
}