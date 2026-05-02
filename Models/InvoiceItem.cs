namespace WarehouseSystem.Models;

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DphRate { get; set; } = 21m;
    public decimal TotalWithoutDph => Quantity * UnitPrice;
    public decimal DphAmount => TotalWithoutDph * (DphRate / 100);
    public decimal TotalWithDph => TotalWithoutDph + DphAmount;
}