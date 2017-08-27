# Espion Spotify
### Enregistrez Spotify durant sa lecture
Fonctionne sous Windows seulement (nécessitant [.NET 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17851)) et sous l'application de Spotify.

## Comment ça fonctionne ?
Le programme s'attache au processus de Spotify et enregistre le son provenant de celui-ci à partir de la carte de son de l'ordinateur.

## Utilisation
Une utilisation standard serait de vouloir enregistrer un disque d'artiste ou une liste de lecture que vous aimez ou avez créée. Pour éviter de devoir attendre la fin de l'enregistrement, puisque le programme capture le son, lancez l'enregistrement la nuit et au matin tout sera terminé sans aucune publicité enregistrée.

![image](https://user-images.githubusercontent.com/23088305/29753216-5ce39cf6-8b3a-11e7-8754-9391324fdc08.png)

## Fonctionnalités
### ...lors de la capture:
- Ignore les publicités.
- Coupe le son des autres applications durant la session.
- Capture le son avec une qualité audio prédéfinie.
- Capture le son maximal de Spotify sans varier que le volume varie entre les enregistrements, même si le son de l'ordinateur est variant ou au plus bas (muêt).
### ...lors de l'enregistrement:
- Ajout d'informations au nom du fichier (ex: Artiste - Titre.mp3)
- Ajout d'informations pour la lecture du fichier : # track, titre, artiste, album, genre (si trouvé sur le web)
- Enregistre les fichiers sous un répertoire prédéfini.

## Options disponibles
- Définir un dossier de sauvegarde
- Définir le format audio MP3 ou WAV
- Définir la qualité audio (basse à haute qualité)
- Définir la durée minimale d'un enregistrement pour éliminer ceux trop courts ou lorsque vous sautez une chanson.
- Ajouter le nom de l'artiste en tant que dossier au lieu de l'ajouter au nom du fichier `Artiste = Dossier [✓]`
- Retirer les espace au nom du fichier `Sans_espaces [✓]`
- Inscrire le numéro d'enregistrement actuel (compteur) [ * ](#note-pour-le-compteur):
  - aux fichiers `Inscrire le compteur devant le titre du fichier [✓]`
  - au lieu de la position CD (infos de lecture) `Remplacer le No de Track par le compteur [✓]`

## Note pour le compteur
L'ajout du compteur au nom du fichier, tel que `07_Artiste_-_Titre.mp3` est utile lors que vous désirez concevoir un cd mp3 avec toutes les fichiers à la racine (sans dossiers) et que vous désirez que le lecteur mp3 (ex: lecteur de votre voiture) filtre l'ordre des chansons dans l'ordre que vous désirez les écouter, autrement l'ordre de ceux-ci pourrait être compromis.

## Techniqualités
- .NET Framwork 4.0
- NAudio et NAudio.Lame
- last.fm API
