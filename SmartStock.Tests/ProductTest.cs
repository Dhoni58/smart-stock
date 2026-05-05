using NUnit.Framework;
using WarehouseSystem.Models;

namespace SmartStock.Tests;

[TestFixture]
public class ProductTests
{
    [Test]
    public void CanIssue_ReturnsTrue_WhenStockIsSufficient()
    {
        var product = new Product { WarehouseInv = 50 };
        var result = product.CanIssue(30);
        Assert.That(result, Is.True);
    }

    [Test]
    public void CanIssue_ReturnsFalse_WhenQuantityExceedsStock()
    {
        var product = new Product { WarehouseInv = 10 };
        var result = product.CanIssue(20);
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanIssue_ReturnsFalse_WhenStockIsZero()
    {
        var product = new Product { WarehouseInv = 0 };
        var result = product.CanIssue(1);
        Assert.That(result, Is.False);
    }

    [Test]
    public void CanIssue_ReturnsTrue_WhenQuantityEqualsStock()
    {
        var product = new Product { WarehouseInv = 15 };
        var result = product.CanIssue(15);
        Assert.That(result, Is.True);
    }

    [Test]
    public void ApplyMovement_IncreasesStock_OnReceipt()
    {
        var product = new Product { WarehouseInv = 10 };
        product.ApplyMovement(MovementType.Receipt, 20);
        Assert.That(product.WarehouseInv, Is.EqualTo(30));
    }

    [Test]
    public void ApplyMovement_DecreasesStock_OnIssue()
    {
        var product = new Product { WarehouseInv = 50 };
        product.ApplyMovement(MovementType.Issue, 20);
        Assert.That(product.WarehouseInv, Is.EqualTo(30));
    }

    [Test]
    public void ApplyMovement_StockBecomesZero_WhenAllIssued()
    {
        var product = new Product { WarehouseInv = 20 };
        product.ApplyMovement(MovementType.Issue, 20);
        Assert.That(product.WarehouseInv, Is.EqualTo(0));
    }

    [Test]
    public void PurchasePriceWithDPH_CalculatesCorrectly()
    {
        var product = new Product { PurchasePrice = 100m, DphRate = 21m };
        var result = product.PurchasePriceWithDph;
        Assert.That(result, Is.EqualTo(121m));
    }

    [Test]
    public void SellingPriceWithDPH_CalculatesCorrectly()
    {
        var product = new Product { SellingPrice = 200m, DphRate = 21m };
        var result = product.SellingPriceWithDph;
        Assert.That(result, Is.EqualTo(242m));
    }

    [Test]
    public void PurchasePriceWithDPH_WithReducedVat_CalculatesCorrectly()
    {
        var product = new Product { PurchasePrice = 100m, DphRate = 10m };
        var result = product.PurchasePriceWithDph;
        Assert.That(result, Is.EqualTo(110m));
    }
}