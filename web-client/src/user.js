var fb = require('./firebase');

var user = null;

function setUser(snap) {
	user = snap.val();
}

module.exports.initOrLoadUser = function(username) {
	var userRef = fb.database().ref('/users/' + username);
	userRef.once('value', function(snap) {
		if (snap.val()) {
			user = snap;
		} else {
			user = {status: 'online'};
			userRef.set(user);
		}
		userRef.on('value', setUser);
	})
}


module.exports.getUser = function(cb) {
	if (user != null) {
		return cb(user);
	}
}