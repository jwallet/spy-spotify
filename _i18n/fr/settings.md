<p align="center">
    <a href="./assets/images/ui_settings.png">
        <img width="420" alt="Spotify Core Settings" height="auto" src="./assets/images/ui_settings.png"/>
    </a>
    <a href="./assets/images/ui_advanced_settings.png">
        <img alt="Spotify Recorder and Watcher Settings" width="420" height="auto" src="./assets/images/ui_advanced_settings.png"/>
    </a>
</p>

## [Paramètres](#parameters)

| Paramètre               | Description et valeurs                 | Valeur par défaut  |
|:------------------------|:---------------------------------------|:---------------|
| Dossier de sauvegarde   | Dossier où seront entreposer les chansons enregistrées | `Musique`     |
| Périphérique audio      | Périphérique audio que Spytify écoutera dessus. Doit être le même que Spotify utilise, pour que Spytify soit capable d'enregistrer. Spotify/Spytify périphériques audio par défaut peuvent être modifiés dans **Windows 🡂 Sound Settings 🡂 App volume and device preferences**. Plus détails dans la [F.A.Q.](./faq.html#spotify-audio-endpoint) | `Default`   |
| Qualité audio           | Du plus bas au plus haut `128kbps` `160kbps (Spotify Gratuit)`¹ `256kbps` `320kbps (Spotify Premium)`² | `160kbps`¹ |
| Durée minimale          | Retire les chansons sous la durée minimale paramétrée  | `30s`  |
| Format audio            | `WAV` et `MP3` (ajoute des infos médiathèque et la couverture de l'album) | `MP3`    |
| Langage                 | Supporte présentement `Anglais` et `Français` | `Anglais` |
| Publicités | Coupe le son des publicités audio lorsque détectées | `On` |

> ¹ Un compte gratuit Spotify pourra _streamer_ à un taux maximal de 160kbps, alors vous ne devriez pas aller plus haut que cette qualité.    
> ² Un compte Spotify Premium pourra _streamer_ à un taux maximal de 320kpbs (si vous activez dans les paramètres de Spotify la qualité maximale), alors vous ne devriez pas aller plus haut que cette qualité.    

## [Paramètres avancés](#advanced-parameters)

| Paramètres de l'enregistreur                  | Description et valeurs                  | Valeur par défaut  |
|:-------------------------------------|:---------------------------------------|:---------------|
| Minuteur pour la session d'enregistrement     | Paramètre un minuteur qui arrêtera la session d'enregistrement après le temps donné jusqu'à la fin de la piste en cours | `00h00m00s` |
| Position de départ du numéro d'enregistrement   | Changer la position prendra effet si l'une des options pour le numéro d'enregistrement ci-dessous sont activés. Changer ce numéro quand vous reprenez une ancienne session d'enregistrement. Plus détails dans la [F.A.Q.](./faq.html#recording-order-number) | `001` |
| Remplace le numéro de la piste by le numéro ci-dessus | Remplace le numéro de la piste de l'album dans les informations médiathèque par le numéro d'enregistrement | `Off` |
| Ajoute le numéro ci-dessus devant le nom du fichier | Ajoute le numéro d'enregistrement devant le nom des fichiers `001 Artiste - Titre.mp3` | `Off` |
| Grouper les artistes dans un répertoire | Groupe les chansons par un répertoire au nom de l'artiste et retire l'artiste du nom du fichier `../Artiste/Titre.mp3` | `Off` |
| Nom des fichiers avec _underscores_ | Retire du nom du fichier tout espace et le remplace par _underscore_ (traits de soulignment) `Artiste_-_Titre.mp3` | `Off` | 


| Paramètre de l'espion             | Description et valeurs                    | Valeur par défaut  |
|:-----------------------------|:---------------------------------------|:---------------|
| Délai le prochain enregistrement   | Délai l'enregistrement de la prochaine chanson si du son est détecté ou le délai d'attente de 1 seconde a été dépassé | `On` |
| Enregistre les types inconnus de piste | Enregistre tout ce qui joue et laisse jouer les publicités, puisque les podcasts sont détectés comme une publicité  | `Off` |
