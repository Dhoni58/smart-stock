# smart-stock
Skladový systém s jednoduchým ovládáním. Plně funkční s produkty, pohyby. Vytvořeno jako projekt do CV.

## Stack
- C# / ASP.NET Core 8
- Razor Pages
- Entity Framework Core
- SQLite
- Bootstrap 5
## Funkce
- Skladové pohyby (výdej/příjem)
- Produkty (Přidání/Úprava/Smazání)
- Hlídač minimální zásoby
- Uživatelé (vedoucí-skladník/hlídání pohybů)
- Dodavatelé (Informace skrz ARES API)
- Export do Excelu a PDF
## Spuštění
- Instalace .NET 10 SDK (https://dotnet.microsoft.com/download)
- Naklonování repozitáře https://github.com/Dhoni58/smart-stock.git
- cd WarehouseSystem (cesta do složky projektu)
- dotnet run (spuštění aplikace)
- přihlášení přímo do aplikace pomocí 5 místného pinu (12345 pro vedoucího(admina)/11111 pro běžného zaměstnance)
