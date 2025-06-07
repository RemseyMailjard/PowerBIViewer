# PowerBI Viewer Desktop
![PowerBI Viewer Screenshot](Assets/screenshot.png)
Een moderne WPF-desktopapplicatie voor het tonen van Power BI dashboards en widgets. Deze tool laadt rapporten dynamisch vanuit JSON-configuratiebestanden en ondersteunt toekomstige uitbreidingen via modulaire widgets.

---

## ✨ Features

- 🚀 **Lichtgewicht & responsieve WPF UI**
- 🧩 **Dynamische dashboards**  
  Genereert automatisch navigatieknoppen vanuit `reports.json`. Voeg eenvoudig nieuwe dashboards toe zonder te programmeren.
- 📊 **Naadloze Power BI-integratie**  
  Gebruikt de Microsoft Edge WebView2-engine om interactieve Power BI-rapporten weer te geven, net als in je browser.
- 🌓 **Light & Dark Theme**  
  Wissel met één klik tussen licht- en donker thema. De UI past zich realtime aan.
- 🗂️ **Modulaire Widget Launcher**  
  Open kleine, specifieke rapporten ("widgets") in aparte vensters – ideaal voor focus en multitasking.
- 📁 **Flexibele architectuur**  
  Duidelijke scheiding tussen Views (UI), Services (logica) en Models (data) voor eenvoudige extensie.

---

## 🛠️ Technische Stack

Voor de tech-liefhebbers onder ons:

- **Framework**: WPF met .NET 8  
- **Rendering Engine**: Microsoft Edge WebView2  
- **Architectuur**: MV-structured (Views, Services, Models)  
- **Styling**: Dynamisch themasysteem via `DynamicResource` en `ResourceDictionary`-bestanden  
- **Configuratie**: Externe JSON-bestanden voor dashboards en widgets (`reports.json`, `widgets.json`)

---

## 🏁 Getting Started

1. Clone deze repository.
2. Open het `.sln`-bestand in Visual Studio 2022 of later.
3. Installeer de WebView2 Runtime (vaak al aanwezig in moderne Windows-versies).
4. Druk op `F5` om de app te bouwen en uit te voeren.
5. Pas `reports.json` en `widgets.json` aan om je eigen Power BI-rapporten te laden.

---

## 👤 Auteur

Gemaakt door [Remsey Mailjard](https://www.remsey.nl)  
Voor trainingsdoeleinden, demo’s en intern gebruik.
