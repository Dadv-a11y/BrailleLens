# BrailleLens

Application mobile d'apprentissage et de conversion du braille, pensée pour les débutants et les personnes voyantes qui veulent lire, saisir ou vérifier du braille.

## Fonctionnalités

- **Apprendre** : alphabet interactif, lettre par lettre.
- **Convertir** : traduction Texte vers Braille et Braille vers Texte sur un seul écran.
- **S'entraîner** : reconnaître une lettre, trouver le braille, écrire en braille, convertir un mot, avec score et progression.
- **Scanner** : photographier une ligne ou un mot en braille pour le lire.

L'application fonctionne entièrement hors ligne, sans compte, et conserve ses données uniquement sur l'appareil. La caméra n'est utilisée que pendant un scan et aucune image n'est conservée.

## Prérequis

- SDK .NET 10
- Workloads `maui-android` et `maui-windows` (`dotnet workload install maui-android maui-windows`)
- Pour Android : un JDK et le SDK Android (ceux fournis avec Android Studio conviennent)

## Structure

```
BrailleLens.csproj      application .NET MAUI (Android, Windows)
BrailleLens.slnx        solution
Core/                   logique métier sans dépendance MAUI
Controls/               composants réutilisables (BrailleCell, BottomSheet, ...)
Pages/                  écrans
ViewModels/             ViewModels (CommunityToolkit.Mvvm)
Services/               persistance locale, navigation, scan
Resources/              styles, polices, icône, splash
Platforms/              code spécifique Android et Windows
BrailleLens.Tests/      tests xUnit de la logique métier
```

## Compiler et lancer

Windows :

```bash
dotnet build BrailleLens.csproj -f net10.0-windows10.0.19041.0
```

```bash
dotnet run --project BrailleLens.csproj -f net10.0-windows10.0.19041.0
```

Android (préciser le JDK s'il n'est pas détecté automatiquement) :

```bash
dotnet build BrailleLens.csproj -f net10.0-android "-p:JavaSdkDirectory=C:\Program Files\Android\Android Studio\jbr"
```

## Tests

```bash
dotnet test BrailleLens.Tests/BrailleLens.Tests.csproj
```

## Licences

Police Inter, distribuée sous licence SIL Open Font License 1.1 (`Resources/Licenses/Inter-OFL.txt`).
