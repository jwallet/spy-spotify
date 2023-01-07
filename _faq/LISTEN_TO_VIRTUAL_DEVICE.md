---
layout: page
hash: listen-to-virtual-device
order: 73
title: How to reroute sound/output of a virtual audio device to my main audio device to listen to it?
namespace: faq
---

[Virtual Audio Device](#install-better-audio-endpoint-device) are a good way to isolate apps together and get better recordings quality. However, by default, the apps you set on this device cannot be heard once you move them to this device, even if you change your default audio device for this one.

To be able to still listen to Spotify when you are recording with Spytify with a Virtual Audio Cable device. You can do one of the following:

- use the Spytify advanced setting **Always listen to Spotify current playback on the default audio device**
- set the Windows option `Listen to this device` in the device properties.

---

#### Windows option: Listen to this device option

Right click on the Audio 🔉 icon in the taskbar and select **Open Sound settings**, go down to the _Sound_ section and select **Sound Control Panel**. In the _Recording_ tab, select the **CABLE Output** device, then click on **Properties**. In the Device properties window, under _Listen_ tab, check the option `Listen to this device`.

Click **Apply** and you should now hear it (and it won't record YouTube or other background noise).

<p align="center"><img alt="Listen to the virtual device output" src="./assets/images/faq_listen_to_cable_output.png" /></p>
