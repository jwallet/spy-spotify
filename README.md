<a title="https://jwallet.github.io/spy-spotify/" href="https://jwallet.github.io/spy-spotify/"><img alt="Spotify Logo" src="https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png" /></a>

Spytify is a Spotify recorder for Windows which records audio from your sound card, ensuring no loss in quality of songs recorded from Spotify, without recording or playing its ads. It automatically splits songs into separate tracks and records straight to MP3 with media infos and album cover, meaning you can start enjoying your music offline.

Spytify runs on Windows only.
- It requires the Microsoft Framework ([.NET 4.5](https://www.microsoft.com/en-ca/download/details.aspx?id=17851) or upper).
- It also requires Spotify Desktop application.

You don't need a Spotify Premium account to use Spytify, __any free account will do__, however having a premium account will enable more audio qualities.

<p align="center"><img alt="Spotify Recorder logs" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/ui_record.png" /></p>

## How it works ?
Spytify records the sound that is coming out of it on your computer sound card. Even if it transcodes the song to an mp3 file, you won't be able to tell the difference between listening to the mp3 file and playing the song on Spotify, because this app, Spytify, provides the same quality that Spotify streaming quality (Spotify Free delivers 160kbps). But, be aware of the quality loss when comparing to a cd ripped file, if you expect flac quality you are not looking at the right tool, and Spotify at its best only delivers 320kbps, not an audiophile app, so no need to rip their songs.

## App features

<p align="center"><img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/features_icons.png" /></p>

| No Ads  | Mutes Other Apps | Spotify Audio Quality  | Record songs at the same volume |
|:------------------|:------------------------------|:---------------------------|:---------------------------------|
| Doesn't record ads and has an option to disable audio ads &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Mutes any other applications while spying &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Gets and records with the same great audio quality than Spotify &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Max out the volume from Spotify and records all song at the same volume level |

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

<p align="center"><img alt="Recorded songs with album cover and media tags in Windows Explorer" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/saved_songs_list.png" /></p>


## Dependencies
- .NET Framwork 4.5
- Newtonsoft.Json
- NAudio.Lame
- last.fm API
- taglib

## Download
### [Download](https://github.com/jwallet/spy-spotify/releases)
