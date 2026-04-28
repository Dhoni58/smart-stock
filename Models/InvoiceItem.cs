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
    public decimal VatRate { get; set; } = 21m;
    public decimal TotalWithoutVat => Quantity * UnitPrice;
    public decimal VatAmount => TotalWithoutVat * (VatRate / 100);
    public decimal TotalWithVat => TotalWithoutVat + VatAmount;
}