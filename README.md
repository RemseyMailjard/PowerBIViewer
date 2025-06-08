# PowerBI Viewer Desktop

![Power BI Viewer Screenshot](Assets/screenshot.png)

**Een moderne, configureerbare WPF-desktopapplicatie voor het tonen en beheren van Power BI dashboards en widgets.**  
Gebouwd op een robuuste en schaalbare architectuur — ideaal voor zowel persoonlijk gebruik als teams die een centrale, aanpasbare viewer nodig hebben.

---

## ✨ Features
- 🚀 **Moderne & Responsieve UI**  
  Gebouwd met .NET 8 en WPF voor een vloeiende, snelle gebruikerservaring.
- ⚙️ **Volledig Beheer via de UI**  
  Voeg, bewerk en verwijder rapporten en widgets direct vanuit het instellingenvenster. Handmatige JSON-aanpassing is verleden tijd.
- ⭐ **Favorieten Systeem**  
  Markeer je belangrijkste rapporten met een ster en krijg er snel toegang toe via een aparte favorietenlijst. Voorkeuren worden onthouden.
- 📊 **Naadloze Power BI-integratie**  
  Gebruikt Microsoft Edge WebView2 om interactieve Power BI-rapporten perfect te renderen.
- 🌓 **Light & Dark Mode**  
  Wissel met één klik tussen licht en donker. De complete UI, inclusief titelbalk, past zich realtime aan.
- 🖼️ **Ingebouwde Tools**  
  Maak screenshots, gebruik fullscreen-presentaties of open rapporten als losse "widgets" voor multitasking.

- 🧱 **Robuuste Architectuur**  
  Op basis van MVVM en Dependency Injection voor schaalbaarheid en onderhoudbaarheid.

---

## 🛠️ Technische Specificaties

**Framework:** WPF op .NET 8

**Architectuur:**

- **MVVM (Model-View-ViewModel):** Scheiding van UI, logica en data voor maximale testbaarheid.
- **Dependency Injection:** Gebruik van `Microsoft.Extensions.DependencyInjection`.
- **Repository Pattern:** Centrale toegang tot rapport- en widgetdata.
- **Styling:** Dynamische thema's via `ResourceDictionary` en `DynamicResource`.

**Rendering Engine:** Microsoft Edge WebView2

**Data-opslag:**

- `reports.json` en `widgets.json` voor configuratie
- Gebruikersinstellingen opgeslagen via `Properties.Settings`
- Validatie via `IDataErrorInfo` in instellingenvenster

---

## 🏁 Getting Started

1. Clone deze repository naar je lokale machine.
2. Open het `.sln`-bestand in Visual Studio 2022 of nieuwer.
3. Zorg ervoor dat de **WebView2 Runtime** is geïnstalleerd.
4. Druk op `F5` om de applicatie te bouwen en starten.
5. Bij de eerste keer opstarten wordt de standaard `reports.json` en `widgets.json` geladen. Je kunt deze wijzigen via de ⚙️ **Instellingen** knop.

---

## 👤 Auteur

Gemaakt door **Remsey Mailjard**

Dit project is een voorbeeld van moderne WPF-ontwikkeling met focus op:
- Best practices
- Heldere architectuur
- Uitstekende gebruikerservaring

---

> 📣 *Contributies, issues en sterren zijn van harte welkom!*

