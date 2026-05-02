using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WarehouseSystem.Models;

namespace WarehouseSystem.Services;

public class ExportService
{
    // ==================== EXCEL ====================

    public byte[] ExportProductsToExcel(List<Product> products)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Produkty");

        // Hlavička
        sheet.Cell(1, 1).Value = "Název";
        sheet.Cell(1, 2).Value = "Kategorie";
        sheet.Cell(1, 3).Value = "Nákupní Cena";
        sheet.Cell(1, 4).Value = "Prodejní Cena";
        sheet.Cell(1, 5).Value = "Na skladě";
        sheet.Cell(1, 6).Value = "Min. zásoba";
        sheet.Cell(1, 7).Value = "Stav";

        // Styl hlavičky
        var header = sheet.Range("A1:F1");
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
        header.Style.Font.FontColor = XLColor.White;

        // Data
        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var row = i + 2;

            sheet.Cell(row, 1).Value = p.Name;
            sheet.Cell(row, 2).Value = p.Category?.Name ?? "—";
            sheet.Cell(row, 3).Value = p.PurchasePrice;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00 Kč";
            sheet.Cell(row, 4).Value = p.SellingPrice;
            sheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00 Kč";
            sheet.Cell(row, 5).Value = p.WarehouseInv;
            sheet.Cell(row, 6).Value = p.MinimumInv;
            sheet.Cell(row, 7).Value = p.WarehouseInv <= p.MinimumInv ? "Nízká zásoba" : "OK";

            // Červené řádky pro nízkou zásobu
            if (p.WarehouseInv <= p.MinimumInv)
                sheet.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEE2E2");
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportMovementsToExcel(List<WarehouseMovement> movements)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Pohyby");

        // Hlavička
        sheet.Cell(1, 1).Value = "Datum";
        sheet.Cell(1, 2).Value = "Produkt";
        sheet.Cell(1, 3).Value = "Typ";
        sheet.Cell(1, 4).Value = "Množství";
        sheet.Cell(1, 5).Value = "Dodavatel";
        sheet.Cell(1, 6).Value = "Vytvořil";
        sheet.Cell(1, 7).Value = "Poznámka";

        var header = sheet.Range("A1:G1");
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#2563EB");
        header.Style.Font.FontColor = XLColor.White;

        for (int i = 0; i < movements.Count; i++)
        {
            var m = movements[i];
            var row = i + 2;

            sheet.Cell(row, 1).Value = m.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            sheet.Cell(row, 2).Value = m.Product?.Name ?? "—";
            sheet.Cell(row, 3).Value = m.Type == MovementType.Receipt ? "Příjem" : "Výdej";
            sheet.Cell(row, 4).Value = m.Quantity;
            sheet.Cell(row, 5).Value = m.Supplier?.Name ?? "—";
            sheet.Cell(row, 6).Value = m.CreatedByUser?.Name ?? "—";
            sheet.Cell(row, 7).Value = m.Note ?? "—";

            if (m.Type == MovementType.Receipt)
                sheet.Cell(row, 3).Style.Font.FontColor = XLColor.FromHtml("#16A34A");
            else
                sheet.Cell(row, 3).Style.Font.FontColor = XLColor.FromHtml("#DC2626");
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ==================== PDF ====================

    public byte[] ExportProductsToPdf(List<Product> products)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(0.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Text("Přehled produktů")
                    .FontSize(16).Bold()
                    .FontColor(Colors.Blue.Medium);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(1);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                    });

                    // Hlavička
                    table.Header(header =>
                    {
                        foreach (var col in new[] { "Název", "Kategorie", "Nák. cena", "Nák. cena s DPH", "DPH", "Prodejní cena", "Na skladě", "Minimum", "Stav" })
                        {
                            header.Cell().Background(Colors.Blue.Medium)
                                .Padding(3).Text(col).FontColor(Colors.White).Bold();
                        }
                    });

                    // Data
                    foreach (var p in products)
                    {
                        var isLow = p.WarehouseInv <= p.MinimumInv;
                        var bg = isLow ? Colors.Red.Lighten4 : Colors.White;

                        table.Cell().Background(bg).Padding(2).Text(p.Name);
                        table.Cell().Background(bg).Padding(2).Text(p.Category?.Name ?? "—");
                        table.Cell().Background(bg).Padding(2).Text($"{p.PurchasePrice:N2} Kč");
                        table.Cell().Background(bg).Padding(2).Text($"{p.PurchasePriceWithDph:N2} Kč");
                        table.Cell().Background(bg).Padding(2).Text($"{p.DphRate:N2} Kč");
                        table.Cell().Background(bg).Padding(2).Text($"{p.SellingPrice:N2} Kč");
                        table.Cell().Background(bg).Padding(2).Text(p.WarehouseInv.ToString());
                        table.Cell().Background(bg).Padding(2).Text(p.MinimumInv.ToString());
                        table.Cell().Background(bg).Padding(2)
                            .Text(isLow ? "Nízká zásoba" : "OK")
                            .FontColor(isLow ? Colors.Red.Medium : Colors.Green.Medium);
                    }
                });

                page.Footer().AlignRight()
                    .Text($"Exportováno: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] ExportMovementsToPdf(List<WarehouseMovement> movements)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text("Přehled skladových pohybů")
                    .FontSize(16).Bold()
                    .FontColor(Colors.Blue.Medium);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2);
                        c.RelativeColumn(3);
                        c.RelativeColumn(1);
                        c.RelativeColumn(1);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        foreach (var col in new[] { "Datum", "Produkt", "Typ", "Množství", "Dodavatel", "Vytvořil", "Poznámka" })
                        {
                            header.Cell().Background(Colors.Blue.Medium)
                                .Padding(5).Text(col).FontColor(Colors.White).Bold();
                        }
                    });

                    foreach (var m in movements)
                    {
                        var isReceipt = m.Type == MovementType.Receipt;
                        var typeColor = isReceipt ? Colors.Green.Medium : Colors.Red.Medium;

                        table.Cell().Padding(5).Text(m.CreatedAt.ToString("dd.MM.yyyy HH:mm"));
                        table.Cell().Padding(5).Text(m.Product?.Name ?? "—");
                        table.Cell().Padding(5).Text(isReceipt ? "Příjem" : "Výdej").FontColor(typeColor);
                        table.Cell().Padding(5).Text(m.Quantity.ToString());
                        table.Cell().Padding(5).Text(m.Supplier?.Name ?? "—");
                        table.Cell().Padding(5).Text(m.CreatedByUser?.Name ?? "—");
                        table.Cell().Padding(5).Text(m.Note ?? "—");
                    }
                });

                page.Footer().AlignRight()
                    .Text($"Exportováno: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateInvoicePdf(Invoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(col =>
                {
                    // Hlavička
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("SMART STOCK").FontSize(20).Bold()
                                .FontColor(Colors.Blue.Medium);
                            c.Item().Text(invoice.Type == InvoiceType.Received
                                ? "PŘIJATÁ FAKTURA"
                                : "VYDANÁ FAKTURA")
                                .FontSize(12).FontColor(Colors.Grey.Medium);
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text($"Číslo: {invoice.InvoiceNumber}").Bold();
                            c.Item().Text($"Datum vystavení: {invoice.IssueDate:dd.MM.yyyy}");
                            c.Item().Text($"Datum splatnosti: {invoice.DueDate:dd.MM.yyyy}");
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);

                    // Dodavatel nebo zákazník
                    col.Item().PaddingBottom(15).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(invoice.Type == InvoiceType.Received
                                ? "DODAVATEL" : "ZÁKAZNÍK")
                                .FontSize(8).FontColor(Colors.Grey.Medium);
                            c.Item().Text(invoice.Type == InvoiceType.Received
                                ? invoice.Supplier?.Name ?? "—"
                                : invoice.CustomerName ?? "—").Bold();
                            c.Item().Text(invoice.Type == InvoiceType.Received
                                ? invoice.Supplier?.Address ?? "—"
                                : invoice.CustomerAddress ?? "—");
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("Vystavil").FontSize(8).FontColor(Colors.Grey.Medium);
                            c.Item().Text(invoice.CreatedByUser?.Name ?? "—").Bold();
                        });
                    });

                    // Tabulka položek
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(4);
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            foreach (var h in new[] { "Produkt", "Množství", "Cena/ks", "DPH", "Bez DPH", "S DPH" })
                            {
                                header.Cell().Background(Colors.Blue.Medium)
                                    .Padding(5).Text(h).FontColor(Colors.White).Bold();
                            }
                        });

                        foreach (var item in invoice.Items)
                        {
                            table.Cell().Padding(5).Text(item.ProductName);
                            table.Cell().Padding(5).Text($"{item.Quantity} ks");
                            table.Cell().Padding(5).Text($"{item.UnitPrice:N2} Kč");
                            table.Cell().Padding(5).Text($"{item.DphRate:0} %");
                            table.Cell().Padding(5).Text($"{item.TotalWithoutDph:N2} Kč");
                            table.Cell().Padding(5).Text($"{item.TotalWithDph:N2} Kč");
                        }
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);

                    // Celková částka
                    col.Item().AlignRight().Column(c =>
                    {
                        var totalWithoutDph = invoice.Items.Sum(i => i.TotalWithoutDph);
                        var totalDph = invoice.Items.Sum(i => i.DphAmount);
                        var totalWithDph = invoice.Items.Sum(i => i.TotalWithDph);

                        c.Item().Text($"Celkem bez DPH: {totalWithoutDph:N2} Kč");
                        c.Item().Text($"DPH 21%: {totalDph:N2} Kč");
                        c.Item().Text($"Celkem s DPH: {totalWithDph:N2} Kč")
                            .FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                    });

                    // Poznámka
                    if (!string.IsNullOrEmpty(invoice.Note))
                    {
                        col.Item().PaddingTop(15).Column(c =>
                        {
                            c.Item().Text("Poznámka").FontSize(8).FontColor(Colors.Grey.Medium);
                            c.Item().Text(invoice.Note);
                        });
                    }
                });

                page.Footer().AlignCenter()
                    .Text($"Vygenerováno: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }
}