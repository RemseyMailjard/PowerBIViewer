// FILE: AssemblyInfo.cs

using System.Runtime.InteropServices;
using System.Windows;

// De meeste assembly-informatie wordt nu beheerd via het .csproj-bestand.
// We laten hier alleen de attributen staan die specifiek zijn voor het project
// en niet eenvoudig via de project-properties kunnen worden ingesteld.

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
// Dit is een unieke identifier voor je project. Je kunt deze zo laten.
[assembly: Guid("CE5D849A-7AB9-4CDC-B6EB-A0AC467EB42C")]

// ThemeInfo-attribuut voor WPF. Dit vertelt de framework hoe om te gaan met theming.
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            // Geen speciale stijlen voor Windows-thema's.
    ResourceDictionaryLocation.SourceAssembly   // De generieke, standaard stijlen zitten in de assembly's zelf.
)]