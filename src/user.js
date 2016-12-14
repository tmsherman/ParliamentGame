var fb = require('./firebase');
var game = require('./game');

var user = {};

module.exports.initOrLoadUser = function(username) {
	module.exports.username = username;
	var userRef = fb.database().ref('/users/' + username);
	userRef.once('value', function(snap) {
		if (snap.val() == null) {
			userRef.set({status: 'online'});
		}
	});
	var roleListener = userRef.child('type').on('value', function(snap) {
		if (snap.val()) {
			userRef.off('value', roleListener);
			user.role = snap.val();
			game.loadCurrentState();
		}
	});
}

module.exports.getUser = function() {
	return user;
}

module.exports.vote = function(eventId, vote) {
	fb.database()
	.ref('/users/' + module.exports.username + '/votes/' + eventId)
	.set(vote);
}