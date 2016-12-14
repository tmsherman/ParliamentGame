var fb = require('./firebase');
var interface = require('./interface');
var user = require('./user');

// load recent outcomes
fb.database().ref('/outcomes').once('value', function(snap) {
	var outcomes = snap.val();
	outcomes.forEach(addOutcome);
});

// watch for new outcomes
fb.database().ref('/outcomes').on('child_added', function(snap) {
	addOutcome(snap.val());
});
var eventsRef = fb.database().ref('/events');
eventsRef.on('child_added', function(snap) {
	var event = snap.val();
	interface.displayEvent(snap.key, event);
});

fb.database().ref('/state').once('value', updateResources);
fb.database().ref('/state').on('value', updateResources);

function updateResources(snap) {
	var state = snap.val();
	var roleToResourceMap = {
		'peasent': 'happiness',
		'noble': 'power',
		'merchant': 'wealth'
	};
	user.getUser(function(user) {
		var desiredResource = roleToResourceMap[user.role];
		var percent = state.resources[desiredResource];
		$('#stats-level').width(percent + '%');
	});
}

function submitVote(event) {
	var eventId = $(event.target).attr('name');
	var vote = $(event.target).val();
	fb.database()
	.ref('/users/' + window.twitchUsername + '/votes/' + eventId)
	.set(vote);
	$("#yes-btn").attr('disabled', true);
	$("#no-btn").attr('disabled', true);
}

$("#yes-btn").click(submitVote);
$("#no-btn").click(submitVote);

function addOutcome(data) {
	var outcome = $("<p>");
	outcome.text(data.text);
	Object.keys(data.changes).forEach(function(key) {
		var val = data.changes[key];
		var change = $("<span>");
		change.addClass(val < 0 ? 'neg-change' : 'pos-change');
		change.text((val < 0 ? '' : '+') + val + " " + key);
		outcome.append(change);
	});
	$("#outcomebox").prepend(outcome);
}