![image](https://user-images.githubusercontent.com/23088305/37263401-62d56ed0-257e-11e8-8eaf-102043c0196f.png)

## Parameters that you can set

| Parameter               | Description and values                 | Default value  |
|:------------------------|:---------------------------------------|:---------------|
| Output path             |                                        | `My Music`     |
| Audio quality           | From Low to High `128kbps` `160kbps (Spotify Free)`¹ `256kbps` `320kbps (Spotify Premium)`² | `160kbps`¹ |
| Minimal length          | Remove songs shorter that the time set  | `30 seconds`  |
| Audio format            | `WAV` and `MP3` (adds media info and album cover) | `MP3`    |
| Disable Ads             | `On`: Disable a whole list of ads, `Off`: Mutes all audios ads | `Off`   |
| Group artists by folder | Save all artist songs inside their own folder, but it will remove the artist of the file name `../Artist/Title.mp3` | `Off` |
| Files names with underscores | Remove from the file name any space and replace it by underscore `Artist_-_Title.mp3` | `Off` | 
| Add number infront of files | Add a recording order number infront of files name `001 Artist - Title.mp3` | `Off` |
| Replace track numbers by number | Replace the album track number in the media info by the recording order number | `Off` |
| Recording number starting position | Changing the position will take effect if one of the options above is enabled, change this number when resuming an old recording session | `001` |
| Language               | Currently supporting `English` and `French` | `English` |

> ¹ Spotify Free streams at 160kbps, so you shouldn't go above that quality.     
> ² Spotify Premium streams at 320kpbs (if enabled in your settings), so you shouldn't go above that quality.

### About the recording order number
Adding the recording order number to files `017_Artiste_-_Titre.mp3` is useful if you want to burn songs to cds and that your mp3 player (like those in cars) orders songs by files name. You will get the a cd with songs ordered in the same order than the album. If it's a playlist, order it first on Spotify and start Spytify.
