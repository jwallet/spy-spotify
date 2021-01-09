---
layout: page
hash: media-tags-not-found
title: "How to use Spotify API to get better (more accurate) media metadata and album cover?"
namespace: faq
---

You can switch between last.fm API to Spotify API to get more accurate metadata, but it requires an account to be able to gather media info from Spotify.

Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications/), log in using your Spotify account and register a new local application:

<p align="center"><img alt="Spotify API Dashboard - Create App" src="./assets/images/faq_spotify_api_create_app.png" /></p>

Once created, click on your new spotify application and click on _Edit the settings_

<p align="center"><img alt="Spotify API Dashboard - Edit App" src="./assets/images/faq_spotify_api_edit_app.png" /></p>

Add a _Redirect URI_ :

- By default, Spytify recommands **http://localhost:4002**, so you can use this one;
- If port `4002` is not available on your computer, you can choose another one like `5000`;
- If you have a proxy, enter your proxy URL instead of those mentionned above;

Click on **Add**, and on **Save** at the bottom.

<p align="center"><img alt="Spotify API Dashboard - Set App" src="./assets/images/faq_spotify_api_dashboard.png" /></p>

Now, copy some information to Spytfiy to be able to connect Spotify API to Spytify.

Open Spytify settings, open _Spotify API Credentials_ modal using the key 🔑 icon and copy those info from your Spotify App Dashboard :

- _Client ID_
- _Client Secret_ (expand the _Show Client Secret_ on the dashboard to reveal the key.)
- _Redirect URI_ (if you added an URI different than `http://locahost:4002`)

<p align="center"><img alt="Set Spotify API credentials in app" src="./assets/images/faq_spotify_credentials.png" /></p>
