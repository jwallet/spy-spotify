<a href="https://jwallet.github.io/spy-spotify/"><img src="https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png" /></a>

Spytify is a Spotify recorder for Windows which records audio from your sound card, ensuring no loss in quality of songs recorded from Spotify, without recording or playing its ads. It automatically splits songs into separate tracks and records straight to MP3 with media infos and album cover, meaning you can start enjoying your music offline.

Spytify runs on Windows only.
- It requires the Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or upper).
- It also requires Spotify Desktop application.

You don't need a Spotify Premium account to use Spytify, __any free account will do__, however having a premium account will enable more audio qualities.

<p align="center"><img src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/ui_record.png" /></p>

## How it works ?
Spytify records the sound that is coming out of it on your computer sound card. Even if it transcodes the song to an mp3 file, you won't be able to tell the difference between listening to the mp3 file and playing the song on Spotify, because this app, Spytify, provides the same quality that Spotify streaming quality (Spotify Free delivers 160kbps). But, be aware of the quality loss when comparing to a cd ripped file, if you expect flac quality you are not looking at the right tool, and Spotify at its best only delivers 320kbps, not an audiophile app, so no need to rip their songs.

<p align="center"><img src="https://raw.githubusercontent.com/jwallet/spy-spotify/master/assets/images/saved_songs_list.png" /></p>

## Dependencies
- .NET Framwork 4.6
- Newtonsoft.Json
- NAudio.Lame
- last.fm API
- Spotify API (see app.config)
- taglib

## Download
### [Download](https://github.com/jwallet/spy-spotify/releases)
