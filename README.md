1. start Windows Docker

2. run command:

```
docker run --rm --label=jekyll --volume=.\:/srv/jekyll -it -p 4000:4000 jekyll/jekyll:4.2.0 jekyll serve --trace --force_polling -w
```

3. visit `http://localhost:4000/spy-spotify/overview.html`
