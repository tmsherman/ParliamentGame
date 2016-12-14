var fb = require('./firebase');
var interface = require('./interface');
var user = require('./user');
var config = require('./config');
var moment = require('moment');

// Map of user roles/classes and the resources they need
var roleToResourceMap = {
	'PEASANT': 'happiness',
	'NOBLE': 'power',
	'MERCHANT': 'wealth'
};
module.exports.roleToResourceMap = roleToResourceMap;

// Firebase references to important nodes
var outcomesRef = fb.database().ref('/outcomes');
var eventsRef = fb.database().ref('/events');
var stateRef = fb.database().ref('/state');

// Load the current state of the game, used to init game after
// Twitch login and Unity server assigns role.
module.exports.loadCurrentState = function() {
	// Listen for new outcomes, and if relevant, add to interface.
	outcomesRef.on('child_added', function(snap) {
		var outcome = snap.val();
		var userRole = user.getUser().role;
		console.log(outcome, userRole,userRole == outcome.type)
		if (userRole == outcome.type) {
			interface.addOutcome(outcome);
		}
	});
	// Listen for new events, and if relevant, add to interface.
	eventsRef.on('child_added', function(snap) {
		var event = snap.val();
		var userRole = user.getUser().role;
		var end = moment.unix(event.utctime)
		  .add(config.eventDuration, 'seconds');
		var now = moment.utc();
		console.log(event, now, end, now < end);
		if (event.type == userRole && now < end) {
			interface.displayEvent(snap.key, snap.val());
		}
	});
	// Listen for changes to game state and update user resources level
	stateRef.on('value', function(snap) {
		var state = snap.val();
		var userRole = user.getUser().role;
		var desiredResource = roleToResourceMap[userRole];
		var percent = state.resources[desiredResource];
		interface.setStatsLevel(percent);
	});
}