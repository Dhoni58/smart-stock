# smart-stock
Skladový systém s pro operaci s produkty, pohyby, a fakturami. Vytvořeno jako projekt do CV.

## Stack
- C# / ASP.NET Core 10
- Razor Pages
- Entity Framework Core
- SQLite
- Bootstrap 5
- ClosedXML a QuestPDF (export do excelu a pdf)
- ARES API (Hledáni firem dle IČO)
## Funkce
- Přihlášení pomoní 5 místného PIN kódu s přiřazenými rolemi (vedoucí/skladník)
- Správa produktů + kategorií (Nákupní a prodejní cena + DPH)
- Skladové pohyby (příjem/výdej) s audit logem
- Hlídač minimální zásoby s upozorněním
- Dodavatelé (automatické doplnění informací skrz ARES API)
- Export do Excelu a PDF (produkty/pohyby)
- Faktury (přijaté/vydané) s generováním PDF
- Unit testy (NUnit)
- CI/CD pipeline (GitHubActions)
## Spuštění bez Dockeru
1. Instalace .NET 10 SDK (https://dotnet.microsoft.com/download)
2. Naklonování repozitáře:
    git clone https://github.com/Dhoni58/smart-stock.git
3. Spuštění aplikace:
    cd smart-stock
    dotnet run
## Spuštění přes Docker
1. Instalace Docker Desktop (https://www.docker.com/products/docker-desktop)
2. Naklonování repozitáře:
    git clone https://github.com/Dhoni58/smart-stock.git
3. Spuštění aplikace:
    cd smart-stock
    docker compose up --build
4. Otevři prohlížeč na http://localhost:8080
## Přihlášení
- Pro roli vedoucí(admin) - 12345
- Pro roli skladníka - 11111 nebo 22222