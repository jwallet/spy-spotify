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
<a title="Like Spytify? Get me a beer :)" href="https://beerpay.io/jwallet/spy-spotify">
<img src="https://github.com/jwallet/spy-spotify/blob/gh-pages/assets/images/beerpaybuck.svg" />
</a>

Spytify is a Spotify recorder for Windows which records Spotify audio without recording or playing ads, ensuring no loss in sound quality. It automatically splits songs into separate tracks and records to MP3 with media metadata, meaning you can start easily enjoying your music offline.

Spytify runs on Windows only. Requirements: 
- Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or higher)
- Spotify Desktop application

A __free Spotify account will work__, but restricts audio quality to 120 kbps. Having a Premium Spotify subsciption will enable recording of up to 320 kbps audio.

<p align="center"><img alt="Spotify Recorder logs" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/ui_record.png" /></p>

## How it works
Spytify records what your computer's sound card outputs. Spytify records the same quality that Spotify outputs, so the recorded version will be indistinguishable from Spotify's version. Spytify records at .mp3 quality, and not at higher qualities like .FLAC, since Spotify can only deliver 320 kbps audio quality.

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
   - \# trac
   - Track Titl
   - Artist
   - Album Title
   - Album Art Cover
   - Genre

<p align="center"><img alt="Recorded songs with album cover and media tags in Windows Explorer" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/saved_songs_list.png" /></p>


## Dependencies
- .NET Framwork 4.6
- Newtonsoft.Json
- NAudio.Lame
- last.fm API
- Spotify API (see your `Spytify.exe.config` file)
- taglib

## Translate
You can help translate Spytify on Zanata.org, see [Translate](translate.md) for more details.

## Download
### [Download](https://github.com/jwallet/spy-spotify/releases)

## Support on Beerpay
If you like Spytify, you can help me out for a couple of :beers:!   
Also, you can make me work on a feature if you Make a wish - [see details](https://beerpay.io/faqs)

<a title="Like Spytify? Get me a beer :)" href="https://beerpay.io/jwallet/spy-spotify"><img src="https://github.com/jwallet/spy-spotify/blob/gh-pages/assets/images/beerpaybadge.svg" /></a> [![Beerpay](https://beerpay.io/jwallet/spy-spotify/make-wish.svg?style=flat-square)](https://beerpay.io/jwallet/spy-spotify?focus=wish)
