var firebase = require('firebase');
var config = {
    apiKey: "AIzaSyBtGj_zMzADQ3vRUnZH29rbIZJpv6YCgDw",
    authDomain: "parliament-6f09c.firebaseapp.com",
    databaseURL: "https://parliament-6f09c.firebaseio.com",
    storageBucket: "parliament-6f09c.appspot.com",
    messagingSenderId: "366765716997"
};
var app = firebase.initializeApp(config);

var database = firebase.database();

firebase.database().ref('/state').once('value').then(function(snapshot) {
  var snap = snapshot.val();
  console.log(snap)
  $('#game').text(snap.status);
});