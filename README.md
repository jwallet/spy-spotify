1. start Windows Docker

2. run command:

```
powershell >
docker pull jekyll/jekyll:3.8
docker build . -t jekyll/jekyll:3.8
docker run --rm --label=jekyll --volume=D:\dev\spy-spotify\:/srv/jekyll -it -p 4000:4000 jekyll/jekyll:3.9.2 jekyll serve --trace --force_polling -w
```

3. visit `http://localhost:4000/spy-spotify/overview.html`
