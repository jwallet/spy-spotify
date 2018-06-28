![image](https://user-images.githubusercontent.com/23088305/37263401-62d56ed0-257e-11e8-8eaf-102043c0196f.png)

## Parameters that you can set
- Output path
- Audio format : mp3 or wav
- Audio quality : low to high (128kbps, 160kbps¹, 256kbps, 320kbps²)
- Minimal length to remove songs that are too short in time or songs that you skiped.
- Save all artist songs inside their own folder, but it will remove the artist of the file name. `../Artist/Title.mp3`
- Remove from the file name any space and replace it by underscore `Artist_-_Title.mp3`
- Add a recording order number to... :
  - infront of files name. `001 Artist - Title.mp3`
  - inside files and replace the track number.

> ¹ Spotify Free streams at 160kbps, so you shouldn't go above that quality.     
> ² Spotify Premium streams at 320kpbs (if enabled in your settings), so you shouldn't go above that quality.

### About the recording order number
Adding the recording order number to files `017_Artiste_-_Titre.mp3` is useful if you want to burn songs to cds and that your mp3 player (like those in cars) orders songs by files name. You will get the a cd with songs ordered in the same order than the album. If it's a playlist, order it first on Spotify and start Spytify.