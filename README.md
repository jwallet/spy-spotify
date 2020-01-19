<a title="https://jwallet.github.io/spy-spotify/" href="https://jwallet.github.io/spy-spotify/">
   <img alt="Spotify Logo" src="https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png" /></a>

[![Build Status](https://travis-ci.org/jwallet/spy-spotify.svg?branch=master)](https://travis-ci.org/jwallet/spy-spotify)
<a href="https://github.com/jwallet/spy-spotify/releases/latest">
   <img src="https://img.shields.io/github/tag/jwallet/spy-spotify.svg?label=version" />
   <img src="https://img.shields.io/github/downloads/jwallet/spy-spotify/total.svg?color=yellow&label=downloads" />
</a>
<a href="https://www.reddit.com/r/spytify">
   ![Subreddit subscribers](https://img.shields.io/reddit/subreddit-subscribers/spytify.svg?label=r%2Fspytify)
</a>
[![Donate](https://img.shields.io/badge/support-donate-ff69b4)](https://jwallet.github.io/spy-spotify/donate.html)
<a href="https://issuehunt.io/r/jwallet/spy-spotify"><img src="https://jwallet.github.io/spy-spotify/assets/images/isohunt_badge.svg" /></a>

Spytify is a Spotify recorder for Windows which records Spotify audio without recording or playing ads, ensuring no loss in sound quality. It automatically splits songs into separate tracks and records to MP3 with media metadata, meaning you can start easily enjoying your music offline.

Spytify runs on Windows only. Requirements: 
- Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or higher)
- Spotify Desktop application

A __free Spotify account will work__, but restricts audio quality to 120 kbps. Having a Premium Spotify subsciption will enable recording of up to 320 kbps audio.

<p align="center"><img alt="Spotify Recorder logs" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/ui_record.png" /></p>

## How it works
Spytify records what Spotify outputs, which is a longer process than downloading a Spotify playlist with a tool.

However, Spytify ensures that all tracks will be the official released one, all sound volume normalized and with media tags and album cover. Playlist Downloaders get mostly all tracks from YouTube which means that they can’t guarantee the choosen track will fit 100% the one in your playlist and they will all be the same quality.

Spytify records the same quality that Spotify outputs (Spotify Free 160kbps, Spotify Premium 320kbps), so the recorded copy will be indistinguishable from Spotify’s one.
## App features

|| Feature | Description |
| - | :-: | :- |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_no_ad.png" /> | __No Ads__ | Doesn't record ads and has an option to disable audio ads completely |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_mute_apps.png" /> | __Mutes Other Apps__ | Mutes any other applications while spying |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_audio_quality.png" /> | __Great Audio Quality__ | Gets and records with the same great audio quality than Spotify |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_max_out.png" /> | __Normalize volume__ | Max out the volume of your Spotify audio session to records all song at the same volume level delivered by Spotify |

## File features
- Splits your recording session into individual tracks formatted using the Artist and Song Title, like so:
   - `Artist - Track Title.mp3`
- Saves all recordings under the same path:
   - `../My Music/`
- Automatically adds following metadata to .mp3 file, if found:
   - \# track
   - Track Title
   - Artist
   - Album Title
   - Album Art Cover
   - Genre

<p align="center"><img alt="Recorded songs with album cover and media tags in Windows Explorer" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/saved_songs_list.png" /></p>

## Translate
You can help translate Spytify on Zanata.org, see [Translate](translate.md) for more details.

## Download
### [Download](https://github.com/jwallet/spy-spotify/releases)

## Support Spytify
😃 If you like Spytify, you can help me out for a [couple of beers](https://jwallet.github.io/spy-spotify/donate.html) 🍺.
