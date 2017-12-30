
![logo-en](https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png)
#### v.0.741 -- december 30th 2017
# English documentation

### Spytify: Records Spotify while it plays
Runs on Windows only ([.NET 4.0](https://www.microsoft.com/en-ca/download/details.aspx?id=17851) and Spotify Desktop app needed).

No need of a Spotify Premium account, __any free account will do__, however a premium account will give you some advantages: no ads and more audio qualities available.

## How it works ?
Spytify finds the Spotify process and records the sound that is coming out of it on your computer sound card.

## Use
A standard use it's to start a recording session at night and let it work overnight to avoid waiting for it to end, because Spytify does not download but records. You will get all your songs automatically split into separate tracks without ads.

<span><img width="420" height="auto" src="https://user-images.githubusercontent.com/23088305/29951698-5e4ebc5a-8e92-11e7-820e-8cd50294b1f9.png"/>
<img width="420" height="auto"  src="https://user-images.githubusercontent.com/23088305/29951688-3c0d8d42-8e92-11e7-862d-e156a1017cb1.png"/></span>

## Features
### App features:
- Does not record ads.
- Mutes any other applications while spying.
- Gets and records with the same great audio quality than Spotify.
- Max out the volume from Spotify and records all song at the same level, even if you play with your main volume.
### File features:
- Split into separate tracks and add names the file like defined in settings `Artist - Title.mp3`
- Records all songs under the same defined path.
- Automatically add infos to mp3 files if found on Internet :
   - \# track
   - Track title
   - Artist name
   - Album tile
   - Album cover art
   - Genre

## Parameters
- Choose an output path
- Choose audio format : mp3 or wav
- Choose audio quality : low to high
- Choose a minimal length to remove songs that are too short in time or songs that you skiped.
- You can save all artist songs inside their own folder, but it will remove the artist of the file name. `../Artist/Title.mp3`
- You can remove from the file name any space and replace it by underscore `Artist_-_Title.mp3`
- Your can add a recording order number to:
  - infront of files name. `001 Artist - Title.mp3`
  - inside files and replace the track number.

## About the recording order number
Adding the recording order number to files `017_Artiste_-_Titre.mp3` is useful if you want to burn songs to cds and that your mp3 player (like those in cars) orders songs by files name. You will get the a cd with songs ordered in the same order than the album. If it's a playlist, order it first on Spotify and start Spytify.

## Dependencies
- .NET Framwork 4.0
- NAudio
- NAudio.Lame
- last.fm API
- taglib

## Download
### [Download](https://github.com/jwallet/Espion-Spotify/releases)
