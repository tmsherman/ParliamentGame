
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