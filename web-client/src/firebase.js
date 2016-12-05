var firebase = require('firebase');
var config = {
    apiKey: "AIzaSyBtGj_zMzADQ3vRUnZH29rbIZJpv6YCgDw",
    authDomain: "parliament-6f09c.firebaseapp.com",
    databaseURL: "https://parliament-6f09c.firebaseio.com",
    storageBucket: "parliament-6f09c.appspot.com",
    messagingSenderId: "366765716997"
};
var app = firebase.initializeApp(config);
module.exports = app;
var database = firebase.database();

firebase.database().ref('/events').once('value').then(function(snapshot) {
  var events = snapshot.val();
  if (events.length > 0) {
  	displayEvent(events[0], '#game');
  }
});

function displayEvent(event, parentSelector) {
	var eventHtml = $('<div>');
	console.log(event);
	eventHtml.text(event.description);
	event.choices.forEach(function(choice) {
		var choiceHtml = $('<button>');
		choiceHtml.text(choice.text);
		choiceHtml.val(choice.name);
		choiceHtml.click(function() {
			firebase.database().ref('/users/colegleason/votes/' + event.name).set(choice.name)
			$(parentSelector).hide();
		})
		eventHtml.append(choiceHtml);
	})
	$(parentSelector).append(eventHtml);
}