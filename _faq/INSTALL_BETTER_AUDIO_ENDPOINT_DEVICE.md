---
layout: page
hash: install-better-audio-endpoint-device
title: Which audio endpoint device should I use for best recording quality? (Virtual Audio Cable)
namespace: faq
---

In Spytify **Settings** tab, under **Audio Device**, you will find a list of hardware devices that emit sounds, but you can also add _virtual devices_ to this list.

> Virtual devices don't have to do the big round-trip to render sound to you through a physical device, they can bypass that and keep the original audio quality much longer when sending it to different endpoints.
>
> Below is the difference of using a **normal/low** end audio device versus a **decent/high** end audio device versus a **virtual** device, all recorded at `2CH 24bits 44kHz @320kpbs` using a _Michael Bubble_ xmas song.

<div style="display:flex">
    <figure style="margin: 0">
        <figcaption>Normal <i style="font-size:85%">(RealTek High Definition)</i></figcaption>
        <audio
            controls
            src="./assets/audio/worst-2CH-24bits-44kHz-320kbps.mp3">
                <code>Your browser does not support the <kbd>audio</kbd> element.</code>
        </audio>
    </figure>
    <figure style="margin: 0 5px;">
        <figcaption>Decent <i style="font-size:85%">(Crystal Sound 3)</i></figcaption>
        <audio
            controls
            src="./assets/audio/better-2CH-24bits-44kHz-320kbps.mp3">
                <code>Your browser does not support the <kbd>audio</kbd> element.</code>
        </audio>
    </figure>
    <figure style="margin: 0">
        <figcaption>Virtual <i style="font-size:85%">(Virtual Audio Cable)</i></figcaption>
        <audio
            controls
            src="./assets/audio/virtual-2CH-24bits-44kHz-320kbps.mp3">
                <code>Your browser does not support the <kbd>audio</kbd> element.</code>
        </audio>
    </figure>
</div>

Spytify comes with a Virtual device that you can install from the app, or download it yourself: [Virtual Audio Cable Device](https://www.vb-audio.com/Cable/)
Go to Settings, at the far right of **Audio Device**, click on the **Speakers** button and the click on _Install Driver_

<p align="center"><img alt="Open Virtual Audio Cable device" src="./assets/images/faq_open_vac.png" /></p>

After installing the driver, it will appear in the audio device list.

> The selected audio device has to be the one that Spotify is set on.

If you change your default audio output (Audio icon in the taskbar) to this device, you will be able to record better quality with Spotify/Spytify. However, you won't be able to hear anything since it's a virtual device that has no physical output.

The way to use it properly is to [isolate Spotify and Spytify on this virtual device](#isolate-spotify-audio-endpoint) to record better audio, and you can use the [Spotify web app](https://open.spotify.com) with a different account for your daily music needs while your playlist is being recorded, or just let Spytify record overnight.
