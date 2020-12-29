---
layout: page
hash: multiple-channels-not-supported
title: How to reduce my audio endpoint quality for mp3 encoding since multi-channel is not supported?
namespace: faq
---

Multi channels audio endpoint device are not supported when encoding to MP3, it must be at least _stereo_ channels with a sample rate of _44,000 Hz_. You can change an audio endpoint channels configuration by:

- Typing **Sound Settings** in Windows menu
- On the right side bar, use the link **Sound Control Panel**
- Find and select your audio endpoint device in the list
- Click on **Configure**
- Make sure _Stereo_ channels is selected, otherwise select it, then applies the changes.

<p align="center"><img alt="Spotify audio device multi-channel fallback" src="./assets/images/faq_fallback_stereo_channels.gif" /></p>
