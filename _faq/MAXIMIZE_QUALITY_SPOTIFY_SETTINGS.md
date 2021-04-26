---
layout: page
hash: maximize-quality-spotify-settings
title: How can I maximize the audio quality of my recordings?
namespace: faq
---

Spytify relies on your audio device quality to record audio files. If your choosen audio endpoint device in Spytify produces bad `.mp3` quality, I suggest you :

- Use another device like the your main PC sound board.
- Avoid using a bluetooth audio device.
- Avoid using a cheap external usb sound card.
- Use a different computer/laptop if it has a better sound card.
- Install Virtual Audio Cable that comes with Spytify and connect both apps with it.

When encoding, `.mp3` has a limitation of 2 channels (stereo) and a sample rate of 44,000 Hz, compare to `.wav` that has less limitations.

You can increase the bitrate in Spytify settings, but if you have _Spotify Free_ it normally delivers music to `160kbps` and `320kbps` on Premium.

<p align="center"><img alt="Spytify audio settings" src="./assets/images/faq_audio_profile.png" /></p>

---

Using Spotify desktop app, click on your profile and go to your settings.

In **Music Quality** section you will find:

- _Streaming quality_ should be set to Very High (or High with Spotify Free).
- _Normalize Volume_ should be turned on.

Click on **Show advanced settings**:

- Turn off _Crossfade songs_ in the **Playback** section.
- Turn off _Allow smooth transitions between songs in a playlist_ in **Automix** section.
- Turn on _Enable hardware acceleration_ in **Compatibility** section.

If you want to avoid recording suggested songs by Spotify when the music ends.

- Turn off _Autoplay similar songs when your music ends_ in **Autoplay** section.

Don't forget to max out the Spotify volume bar inner-app (bottom-right corner).

<p align="center"><img alt="Maximize Spotify audio quality" src="./assets/images/maximize_spotify_settings.gif" /></p>
