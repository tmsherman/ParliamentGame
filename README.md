To play the game as is, for oroq's stream, go to [the hosted client](https://tmsherman.github.io/ParliamentGame/).

Because this is using Firebase and Twitch APIs, you will have to setup new API information with them if you want to run this web-client elsewhere. See `src/config.js` as an example. You should also go here to change the stream and chat that are embedded.

Making changes / Developing
===========================
To install dependencies: `npm install`
Then to run: `webpack-dev-server --progress --colors`
Open http://localhost:8080/webpack-dev-server/

Deploying to prod (gh-pages)
============================
In `web-client`:
```
webpack
git add ... && git commit ...
cd ..
git subtree push --prefix web-client origin gh-pages
```