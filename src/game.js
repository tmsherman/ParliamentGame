var fb = require('./firebase');
var interface = require('./interface');
var user = require('./user');
var config = require('./config');
var moment = require('moment');

var roleToResourceMap = {
	'PEASANT': 'happiness',
	'NOBLE': 'power',
	'MERCHANT': 'wealth'
};
module.exports.roleToResourceMap = roleToResourceMap;

var outcomesRef = fb.database().ref('/outcomes');
var eventsRef = fb.database().ref('/events');
var stateRef = fb.database().ref('/state');

module.exports.loadCurrentState = function() {
	outcomesRef.on('child_added', function(snap) {
		var outcome = snap.val();
		var userRole = user.getUser().role;
		if (userRole == outcome.type) {
			interface.addOutcome(snap.val());
		}
	});
	eventsRef.on('child_added', function(snap) {
		var event = snap.val();
		var userRole = user.getUser().role;
		var end = moment.unix(event.utctime)
		  .add(config.eventDuration, 'seconds');
		var now = moment.utc();
		console.log(event, event.type == userRole, now < end);
		if (event.type == userRole && now < end) {
			interface.displayEvent(snap.key, snap.val());
		}
	});
	stateRef.on('value', updateResources);
}

function updateResources(snap) {
	var state = snap.val();
	var userRole = user.getUser().role;
	if (userRole) {
		var desiredResource = roleToResourceMap[userRole];
		var percent = state.resources[desiredResource];
		interface.setStatsLevel(percent);
	}
}