# BrailleLens

Application mobile d'apprentissage et de conversion du braille, pensée pour les débutants et les personnes voyantes (enseignants, proches, aidants) qui veulent lire, saisir ou vérifier du braille.

## Fonctionnalités

- **Apprendre** : alphabet interactif lettre par lettre, cellule explorable point par point, sélection rapide d'une lettre.
- **S'entraîner** : séances de 10 questions sur quatre exercices (reconnaître une lettre, trouver le braille, écrire en braille, convertir un mot), avec score, niveau et lettres maîtrisées.
- **Convertir** : Texte vers Braille (copie et partage en braille Unicode) et Braille vers Texte sur un même écran.
- **Scanner** : photographier une ligne de braille imprimé et la lire, puis l'ouvrir dans Convertir.
- **Paramètres** : numéros des points, grandes cellules, vibrations.

Périmètre braille : alphabet français intégral de A à Z. Les accents, chiffres et signes de ponctuation sont signalés comme non pris en charge.

## Accessibilité et écoconception

- Contrastes conformes WCAG AA, textes qui suivent la taille de police du système, zones tactiles d'au moins 48 points.
- Chaque bouton et chaque point est annoncé par le lecteur d'écran ; les états ne reposent jamais sur la seule couleur.
- Fonctionnement entièrement hors ligne, sans compte ni permission réseau ; progression et réglages stockés sur l'appareil.
- Le scan est analysé sur l'appareil, sans bibliothèque tierce ; la photo est supprimée juste après l'analyse.
- Aucune animation décorative.

## Prérequis

- SDK .NET 10
- Workloads `maui-android` et `maui-windows`
- Pour Android : un JDK et le SDK Android (ceux fournis avec Android Studio conviennent)

```bash
dotnet workload install maui-android maui-windows
```

## Structure

```
BrailleLens.csproj      application .NET MAUI (Android, Windows)
BrailleLens.slnx        solution
Core/                   logique métier sans dépendance MAUI
  Braille/              alphabet, cellule, traduction, descriptions vocales
  Learning/             progression, niveaux
  Training/             questions, générateur, séance
  Scanning/             détection des points et lecture des cellules
Controls/               composants réutilisables (BrailleCell, BottomSheet, ...)
Pages/                  écrans
Sheets/                 contenu des bottom sheets
ViewModels/             ViewModels (CommunityToolkit.Mvvm)
Services/               persistance locale, réglages, navigation, scan
Resources/              styles, icônes, polices, icône d'application, splash
Platforms/              code spécifique Android et Windows
BrailleLens.Tests/      tests xUnit de la logique métier
```

## Compiler et lancer

Windows :

```bash
dotnet run --project BrailleLens.csproj -f net10.0-windows10.0.19041.0
```

Android, installation sur un appareil branché en USB avec le débogage activé (préciser le JDK s'il n'est pas détecté) :

```bash
dotnet build BrailleLens.csproj -f net10.0-android -t:Install "-p:JavaSdkDirectory=C:\Program Files\Android\Android Studio\jbr"
```

APK de Release (32 et 64 bits ARM) :

```bash
dotnet publish BrailleLens.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk "-p:JavaSdkDirectory=C:\Program Files\Android\Android Studio\jbr"
```

L'APK est produit dans `bin/Release/net10.0-android/publish/`.

## Tests

```bash
dotnet test BrailleLens.Tests/BrailleLens.Tests.csproj
```

Les tests couvrent l'alphabet, la traduction, la progression, la génération des exercices, la séance d'entraînement et la lecture d'images de braille de synthèse (bruit, échelle, rotation, couleurs inversées).

## Licences

Police Inter, distribuée sous licence SIL Open Font License 1.1 (`Resources/Licenses/Inter-OFL.txt`).
