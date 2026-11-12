# A52 Cinema Player V2

Lecteur vidéo Windows x64 basé sur WinForms, LibVLCSharp et le moteur VLC.
Le décodage AC-3 (Dolby Digital) et E-AC-3 (Dolby Digital Plus) est confié au moteur VLC/FFmpeg embarqué dans les distributions VLC compatibles.

## Compilation GitHub Actions
1. Crée un dépôt GitHub vide.
2. Décompresse cette archive et envoie **tout le contenu** du dossier dans le dépôt.
3. Ouvre l'onglet **Actions** et lance `Build Windows x64` (ou pousse un commit).
4. Télécharge l'artefact `A52CinemaPlayer-win-x64` à la fin du workflow.

## Compilation locale
Installer le SDK .NET 8, puis depuis `A52CinemaPlayer/` :
```powershell
dotnet restore
dotnet build -c Release -r win-x64
```

## État des fonctions
- Lecture vidéo et audio, pause, arrêt, volume, plein écran : implémentés via LibVLC.
- Décodage AC-3/E-AC-3 : assuré par les codecs disponibles dans le moteur VLC utilisé.
- Sélection Auto/Stéréo/5.1/7.1 : choix de configuration exposé dans l'interface; la disponibilité réelle dépend du périphérique Windows et du moteur VLC.
- DRC, Dialnorm, Lt/Rt, Surround EX, Headphone et bass management : contrôles d'interface préparatoires dans cette version; ils ne sont **pas** présentés comme des traitements DSP Dolby opérationnels. Ils nécessitent une chaîne PCM dédiée et des algorithmes validés.

## Dépendances
NuGet : LibVLCSharp, LibVLCSharp.WinForms.
Le moteur natif VLC est chargé depuis une installation VLC compatible présente sur le PC.
Installer VLC 64 bits avant le premier lancement, ou placer les bibliothèques VLC natives dans le chemin de recherche de l'application.
