![logo-en](https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png)

### Spytify: Records Spotify while it's playing
Runs on Windows only ([.NET 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17851) and Spotify Desktop app needed).

No need of a Spotify Premium account, __any free account will do__, however a premium account will give you some advantages: no ads and more audio qualities available.

## How it works ?
Spytify finds the Spotify process and records the sound that is coming out of it on your computer sound card.

## Use
A standard use will be something like recording an artist album or a Spotify playlist that you like or created. To avoid waiting for the end of the recording session, since the app does not download but records, start spying at night and let it work until sunrise. You will get all your songs automatically split into separate tracks without ads.

<span><img width="420" height="auto" src="https://user-images.githubusercontent.com/23088305/29935589-4404d068-8e4d-11e7-9ecc-31c09c9f24f8.png"/>
<img width="420" height="auto"  src="https://user-images.githubusercontent.com/23088305/29935628-63b13f0a-8e4d-11e7-9843-3da9884fc952.png"/></span>

## Features
### App features:
- Skips ads.
- Mutes any other applications while spying.
- Gets and records with the same great audio quality than Spotify.
- Max out the volume from Spotify and records all song at the same level, even if you play with your main volume.
### File features:
- Split into separate tracks and add names the file like defined in settings `Artist - Title.mp3`
- Add infos into the file : # track, title, artist, album, genre (if found on Internet)
- Records all songs under the same defined path.

## Parameters
- Choose an output path
- Choose audio format : mp3 or wav
- Choose audio quality : low to high
- Choose a minimal length to remove songs that are too short in time or songs that you skiped.
- You can save all artist songs inside their own folder, but it will remove the artist of the file name.
- You can remove from the file name any space and replace it by underscore
- Your can add a recording order number to... [*](#about-the-recording-order-number):
  - infront of files name.
  - inside files and replace the track number.

## About the recording order number
Adding the recording order number to files `07_Artiste_-_Titre.mp3` is useful if you want to burn songs to cds and that your mp3 player (like those in cars) orders songs by files name. You will get the a cd with songs ordered in the same order than the album. If it's a playlist, order it first on Spotify and start Spytify.

## Dependencies
- .NET Framwork 4.0
- NAudio et NAudio.Lame
- last.fm API

### [Download](https://github.com/jwallet/Espion-Spotify/releases)


![logo-fr](https://user-images.githubusercontent.com/23088305/29906217-7169fd92-8de1-11e7-8fa6-2183ee7b8e1c.png)

### Espion Spotify: Enregistrez Spotify durant sa lecture
Fonctionne sous Windows seulement (nécessitant [.NET 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17851)) et sous l'application de Spotify. 

Pas besoin d'un compte Spotify Premium, __un compte gratuit suffit pour l'utilisation du programme__, parcontre un compte premium à ses avantages: sans pubs et plus de choix de qualité d'enregistrement.

## Comment ça fonctionne ?
Le programme s'attache au processus de Spotify et enregistre le son provenant de celui-ci à partir de la carte de son de l'ordinateur.

## Utilisation
Une utilisation standard serait de vouloir enregistrer un disque d'artiste ou une liste de lecture que vous aimez ou avez créée. Pour éviter de devoir attendre la fin de l'enregistrement, puisque le programme capture le son, lancez l'enregistrement la nuit et au matin tout sera terminé sans aucune publicité enregistrée.

<span><img width="420" height="auto" src="https://user-images.githubusercontent.com/23088305/29935767-f3c17380-8e4d-11e7-8650-4ff6fb1dd736.png"/>
<img width="420" height="auto"  src="https://user-images.githubusercontent.com/23088305/29935742-da2f06f8-8e4d-11e7-8c66-ffc379d2ca14.png"/></span>

## Fonctionnalités
### ...lors de la capture:
- Ignore les publicités.
- Coupe le son des autres applications durant la session.
- Capture le son avec une qualité audio prédéfinie.
- Capture le son maximal de Spotify sans que le volume varie entre les enregistrements, même si le son de l'ordinateur est variant ou au plus bas (muêt).
### ...lors de l'enregistrement:
- Ajout d'informations au nom du fichier `Artiste - Titre.mp3`
- Ajout d'informations pour la lecture du fichier : # track, titre, artiste, album, genre (si trouvé sur le web)
- Enregistre les fichiers sous un répertoire prédéfini.

## Options disponibles
- Définir un dossier de sauvegarde
- Définir le format audio MP3 ou WAV
- Définir la qualité audio (basse à haute qualité)
- Définir la durée minimale d'un enregistrement pour éliminer ceux trop courts, lorsque vous sautez une chanson.
- Ajouter le nom de l'artiste en tant que dossier au lieu de l'ajouter au nom du fichier
- Retirer les espace au nom du fichier
- Inscrire le numéro d'enregistrement actuel (compteur) [ * ](#note-pour-le-compteur):
  - aux fichiers
  - au lieu de la position CD (infos de lecture)

## Note pour le compteur
L'ajout du compteur au nom du fichier, tel que `07_Artiste_-_Titre.mp3` est utile si vous désirez concevoir un cd mp3 avec tous les fichiers à la racine (sans dossiers) et que le lecteur mp3 (ex: lecteur de votre voiture) filtre l'ordre des chansons dans l'ordre que vous désirez les écouter, autrement l'ordre de ceux-ci pourrait être compromis.

## Techniqualités
- .NET Framwork 4.0
- NAudio et NAudio.Lame
- last.fm API

### [Télécharger](https://github.com/jwallet/Espion-Spotify/releases)
