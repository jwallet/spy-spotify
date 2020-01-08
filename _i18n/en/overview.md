Spytify is a Spotify recorder for Windows which records Spotify audio without recording or playing ads, ensuring no loss in sound quality. It automatically splits songs into separate tracks and records to MP3 with media metadata, meaning you can start easily enjoying your music offline.

<p align="center"><img alt="Spotify Recorder logs" src="./assets/images/ui_record.png" /></p>

## [How it works?](#how-it-works)
Spytify records what your computer’s sound card outputs. Spytify records the same quality that Spotify outputs, so the recorded copy will be indistinguishable from Spotify’s one. Spytify records at .mp3 quality, and not at higher qualities like .flac, since Spotify can only deliver 320 kbps audio quality.

<figure>
    <figcaption>Listen to an mp3 sample at 120kbp/s:</figcaption>
    <audio
        controls
        src="./assets/audio/sample.mp3">
            <code>Your browser does not support the <kbd>audio</kbd> element.</code>
    </audio>
</figure>

### [Standard use](#standard-use)
A standard use is to start a recording session at night using your favorite playlist and let it work overnight, so you avoid waiting for it to end, because Spytify does not download but records. You will then get all your songs automatically split into separate tracks without ads.

### [Requirements](#requirements)
Spytify runs on Windows only.
- Microsoft Framework ([.NET 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) or higher).
- Spotify Desktop application.

A __free Spotify account__ will work, but Spotify restricts audio quality to 120 kbps. Having a Premium Spotify subscription will enable recording of up to 320 kbps audio.