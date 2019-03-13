<a title="https://jwallet.github.io/spy-spotify/" href="https://jwallet.github.io/spy-spotify/"><img alt="Spotify Logo" src="https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png" /></a>

Spytify is a Spotify recorder for Windows which records Spotify audio without recording or playing ads, ensuring no loss in sound quality. It automatically splits songs into separate tracks and records to MP3 with media metadata, meaning you can start easily enjoying your music offline.

Spytify runs on Windows only. Requirements: 
- Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or higher)
- Spotify Desktop application

A __free Spotify account will work__, but restricts audio quality to 120 kbps. Having a Premium Spotify subsciption will enable recording of up to 320 kbps audio.

<p align="center"><img alt="Spotify Recorder logs" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/ui_record.png" /></p>

## How it works
Spytify records what your computer's sound card outputs. Spytify records the same quality that Spotify outputs, so the recorded version will be indistinguishable from Spotify's version. Spytify records at .mp3 quality, and not at higher qualities like .FLAC, since Spotify can only deliver 320 kbps audio quality.

## App features

<p align="center"><img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/features_icons.png" /></p>

| No Ads  | Mutes Other Apps | Spotify Audio Quality  | Record songs at the same volume |
|:------------------|:------------------------------|:---------------------------|:---------------------------------|
| Doesn't record ads and has an option to disable audio ads &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Mutes any other applications while spying &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;| Gets and records with the same great audio quality than Spotify &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; | Max out the volume from Spotify and records all song at the same volume level |

## File management features
- Split into separate tracks and add file names, which can be defined in settings like this: `Artist - Title.mp3`
- Saves all recordings in the same directory
- Automatically adds following metadata to .mp3 file (if found) :
   - \# track
   - Track title
   - Artist name
   - Album tile
   - Album cover art
   - Genre

<p align="center"><img alt="Recorded songs with album cover and media tags in Windows Explorer" src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/saved_songs_list.png" /></p>


## Dependencies
- .NET Framwork 4.6
- Newtonsoft.Json
- NAudio.Lame
- last.fm API
- Spotify API (see your `Spotify.exe.config` file)
- taglib

## Download
### [Download](https://github.com/jwallet/spy-spotify/releases)
