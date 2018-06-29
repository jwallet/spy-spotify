![logo-en](https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png)

### Spytify: Records Spotify while it plays without ads
Spytify runs on Windows only.
- It requires the Microsoft Framework ([.NET 4.5](https://www.microsoft.com/en-ca/download/details.aspx?id=17851) or upper).
- It also requires Spotify Desktop application.

You don't need a Spotify Premium account to use Spytify, __any free account will do__, however having a premium account will enable more audio qualities.

## How it works ?
Spytify uses the local Spotify API¹ and records the sound that is coming out of it on your computer sound card. Even if it transcodes the song to an mp3 file, you won't be able to tell the difference between listening to the mp3 file and playing the song on Spotify, because this app, Spytify, provides the same quality that Spotify streaming quality (Spotify Free delivers 160kbps). But, be aware of the quality loss when comparing to a cd ripped file, if you expect flac quality you are not looking at the right tool, and Spotify at its best only delivers 320kbps, not an audiophile app, so no need to rip their songs.

### Standard Use
A standard use it's to start a recording session at night using your favorite playlist and let it work overnight, so you avoid waiting for it to end, because Spytify does not download but records. You will then get all your songs automatically split into separate tracks without ads. Don't forget that the output path can be your android music folder.

<span><img width="420" height="auto" src="https://user-images.githubusercontent.com/23088305/37263373-39d18762-257e-11e8-9735-758d6517d4c8.png"/>
<img width="420" height="auto" src="https://user-images.githubusercontent.com/23088305/37263401-62d56ed0-257e-11e8-8eaf-102043c0196f.png"/></span>

## App features
- Doesn't record ads and has an option to disable audio ads.
- Mutes any other applications while spying.
- Gets and records with the same great audio quality than Spotify.
- Max out the volume from Spotify and records all song at the same level, even if you scroll up/down your main volume.

## File features
- Split into separate tracks and add names the file like defined in settings `Artist - Title.mp3`
- Records all songs under the same defined path.
- Automatically add infos to mp3 files if found on Internet :
   - \# track
   - Track title
   - Artist name
   - Album tile
   - Album cover art
   - Genre

![image](https://user-images.githubusercontent.com/23088305/37262916-232d806c-257c-11e8-8d2f-8d5c16ab5e2f.png)

## Parameters

| Parameter               | Description and values                 | Default value  |
|:------------------------|:---------------------------------------|:---------------|
| Output path             | Folder where recorded songs will be stored | `Music`     |
| Audio quality           | From Low to High `128kbps` `160kbps (Spotify Free)`¹ `256kbps` `320kbps (Spotify Premium)`² | `160kbps`¹ |
| Minimal length          | Remove songs shorter that the time set  | `30s`  |
| Audio format            | `WAV` and `MP3` (adds media info and album cover) | `MP3`    |
| Disable Ads             | `On`: Disable a whole list of ads, `Off`: Mutes all audios ads | `Off`   |
| Group artists by folder | Save all artist songs inside their own folder, but it will remove the artist of the file name `../Artist/Title.mp3` | `Off` |
| Files names with underscores | Remove from the file name any space and replace it by underscore `Artist_-_Title.mp3` | `Off` | 
| Add number infront of files | Add a recording order number infront of files name `001 Artist - Title.mp3` | `Off` |
| Replace track numbers by number | Replace the album track number in the media info by the recording order number | `Off` |
| Recording number starting position | Changing the position will take effect if one of the options above is enabled, change this number when resuming an old recording session | `001` |
| Language               | Currently supporting `English` and `French` | `English` |

> ¹ Spotify Free streams at 160kbps, so you shouldn't go above that quality.     
> ² Spotify Premium streams at 320kpbs (if enabled in your settings), so you shouldn't go above that quality.

### About the recording order number
Adding the recording order number to files `017_Artiste_-_Titre.mp3` is useful if you want to burn songs to cds and that your mp3 player (like those in cars) orders songs by files name. You will get the a cd with songs ordered in the same order than the album. If it's a playlist, order it first on Spotify and start Spytify.

## Dependencies
- .NET Framwork 4.5
- [Spotify API NET](https://johnnycrazy.github.io/SpotifyAPI-NET/) by [JohnnyCrazy](https://github.com/JohnnyCrazy)
- Newtonsoft.Json
- NAudio.Lame
- last.fm API
- taglib

## Download
### [Download](https://github.com/jwallet/Espion-Spotify/releases)
