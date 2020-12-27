---
layout: page
hash: media-tags-not-found
title: "Media tags and album cover are not always found (Switch to Spotify API)"
namespace: faq
---

You can switch between last.fm API to Spotify API (still in development but can be set in the **spytify.exe.config** file).

Open a text editor and follow these steps to enable the gathering of MP3 media info from Spotify:

- Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications/)
- Register a new local application, e.g.:
  - App or Hardware Name: spotify
  - App or Hardware Description: none
  - What are you building?: i don't know
- Click on the new spotify application that you created and click on _Edit the settings_
- Set 'Redirect URI' with the value **http://localhost:4002**, add and save it.

<p align="center"><img alt="Spotify API Dashboard - Set App" src="./assets/images/faq_spotify_api_dashboard.png" /></p>

On the spotify API dashboard, copy these info and paste them in Spytify in _Settings_ using the key icon:

- The _Client ID_
- The _Client Secret_ (click on _Show Client Secret_)

<p align="center"><img alt="Set Spotify API credentials in app" src="./assets/images/faq_spotify_credentials.png" /></p>
