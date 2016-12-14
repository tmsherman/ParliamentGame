var fb = require('./firebase');
var game = require('./game');
var interface = require('./interface');

var user = {};

// if a user node does not exist for this user, make one
// then listen for the user role to be added so we can load
// the rest of the game.
module.exports.initOrLoadUser = function(username) {
	module.exports.username = username;
	var userRef = fb.database().ref('/users/' + username);
	userRef.once('value', function(snap) {
		if (snap.val() == null) {
			userRef.set({status: 'online'});
		}
	});
	var roleListener = userRef.child('type').on('value', function(snap) {
		var role = snap.val();
		if (role) {
			user.role = role;
			interface.setStatsSymbol(user.role);
			interface.clearOutcomes();
			if (!game.isLoaded()) {
				game.loadCurrentState();
			}
		}
	});
}

// return the user object
module.exports.getUser = function() {
	return user;
}

// add a vote for the specified event
module.exports.vote = function(eventId, vote) {
	fb.database()
	.ref('/users/' + module.exports.username + '/votes/' + eventId)
	.set(vote);
}