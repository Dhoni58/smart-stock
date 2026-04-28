namespace WarehouseSystem.Models;

public enum InvoiceType
{
    Received,   // přijatá — od dodavatele
    Issued      // vydaná — pro zákazníka
}

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType Type { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? Note { get; set; }
    public int CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<InvoiceItem> Items { get; set; } = new();
    public List<WarehouseMovement> Movements { get; set; } = new();
}