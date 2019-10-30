
run command:
```
powershell >
docker run --rm --label=jekyll --volume=D:\dev\spy-spotify\:/srv/jekyll -it -p 4000:4000 jekyll/jekyll jekyll serve --trace --force_polling -w
```

copy paste the `_site` directory to root