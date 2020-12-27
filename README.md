[![Spytify Logo](https://user-images.githubusercontent.com/23088305/29906214-6daad21c-8de1-11e7-80f5-ef6791cc7825.png)](https://jwallet.github.io/spy-spotify/)

[![Build status](https://ci.appveyor.com/api/projects/status/s26ibv6ls9j56enr/branch/master?svg=true)](https://ci.appveyor.com/project/jwallet/spy-spotify/branch/master)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/jwallet/spy-spotify/master?compact_message)](https://ci.appveyor.com/project/jwallet/spy-spotify/branch/master/tests)
[![Latest release](https://img.shields.io/github/tag/jwallet/spy-spotify.svg?label=version)](https://github.com/jwallet/spy-spotify/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/jwallet/spy-spotify/total.svg?color=yellow&label=downloads)](https://github.com/jwallet/spy-spotify/releases/latest)
[![Sub Reddit](https://img.shields.io/reddit/subreddit-subscribers/spytify.svg?label=r%2Fspytify)](https://www.reddit.com/r/spytify)
[![Donate](https://img.shields.io/badge/support-donate-ff69b4)](https://jwallet.github.io/spy-spotify/donate.html)
[![Issuehunt](https://jwallet.github.io/spy-spotify/assets/images/isohunt_badge.svg)](https://issuehunt.io/r/jwallet/spy-spotify)

Spytify is a Spotify recorder for Windows which records Spotify audio without recording or playing ads, ensuring no loss in sound quality. It automatically splits songs into separate tracks and records to MP3 with media metadata, meaning you can start easily enjoying your music offline.

Spytify runs on Windows only. Requirements:

- Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or higher)
- Spotify Desktop application

A **free Spotify account will work**, but restricts audio quality to 120 kbps. Having a Premium Spotify subsciption will enable recording of up to 320 kbps audio.

<p align="center"><img alt="Spotify Recorder logs" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/ui_record.png" /></p>

## How it works

Spytify records what Spotify outputs, which is a longer process than downloading a Spotify playlist with a tool.

However, Spytify ensures that all tracks will be the official released one, all sound volume normalized and with media tags and album cover. Playlist Downloaders get mostly all tracks from YouTube which means that they can’t guarantee the choosen track will fit 100% the one in your playlist and they will all be the same quality.

Spytify records the same quality that Spotify outputs ([Spotify Free 160kbps, Spotify Premium 320kbps](https://support.spotify.com/us/using_spotify/system_settings/high-quality-streaming/)), so the recorded copy will be indistinguishable from Spotify’s one.

## App features

|                                                                                                                                     |         Feature         | Description                                                                                                        |
| ----------------------------------------------------------------------------------------------------------------------------------- | :---------------------: | :----------------------------------------------------------------------------------------------------------------- |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_no_ad.png" />         |       **No Ads**        | Doesn't record ads and has an option to disable audio ads completely                                               |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_mute_apps.png" />     |  **Mutes Other Apps**   | Mutes any other applications while spying                                                                          |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_audio_quality.png" /> | **Great Audio Quality** | Gets and records with the same great audio quality than Spotify                                                    |
| <img alt="features" src="https://raw.githubusercontent.com/jwallet/spy-spotify/gh-pages/assets/images/feature_max_out.png" />       |  **Normalize volume**   | Max out the volume of your Spotify audio session to records all song at the same volume level delivered by Spotify |

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

## Want a feature fast?

Spytify is supported by _IssueHunt_. [![Issuehunt](https://jwallet.github.io/spy-spotify/assets/images/isohunt_badge.svg)](https://issuehunt.io/r/jwallet/spy-spotify)

That means you can have your issue (feature/improvement) prioritized, just open an [feature request](https://github.com/jwallet/spy-spotify/issues/new/choose) on this repository and go to our IssueHunt page and fund [your issue](https://issuehunt.io/r/jwallet/spy-spotify?tab=idle) to promote it. When the bounty will be enough for someone to start working on it, he will then submit a Pull Request with his code on this repo and link it to the IssueHunt page. Once merged and approved the bounty will then be rewarded to the PR author after the repo owner ensures that the requested feature is covered. See a past example [here](https://issuehunt.io/r/jwallet/spy-spotify/issues/282)

## Translate

You can help translate Spytify on Zanata.org, see [Translate](translate.md) for more details.

## Download

### [Download](https://github.com/jwallet/spy-spotify/releases)

## Support Spytify

😃 If you like Spytify, you can help me out for a [couple of beers](https://jwallet.github.io/spy-spotify/donate.html) 🍺.
