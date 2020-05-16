---
layout: page
hash: media-tags-not-found
title: Media tags and album cover are not always found (Switch to Spotify API).
namespace: faq
---
You can switch between last.fm API to Spotify API (still in development but can be set in the **spytify.exe.config** file).

Open a text editor and follow these steps to enable the gathering of MP3 media info from Spotify:

- Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications/)
- Register a new local application, e.g.:
    - App or Hardware Name: spotify
    - App or Hardware Description: none
    - What are you building?: i don't know
- Click on the new spotify application that you created and click on *Edit the settings*
- Set 'Redirect URI' with the value 'http://localhost:4002', add and save it.
- On the spotify app dashboard, copy these info:
    - The 'Client ID' 
    - The 'Client Secret' (click on 'Show Client Secret')
- Paste the ids in the **spytify.exe.config** file at theses lines

```
<setting name=\"SpotifyAPISecretId\" serializeAs=\"String\">
    <value>
        412fa2302d864432854742ba35e869cb
    </value>
</setting>
<setting name=\"SpotifyAPIClientId\" serializeAs=\"String\">
    <value>
        c3d9c679a66641f284e6740faa479e14
    </value>
</setting>
```
