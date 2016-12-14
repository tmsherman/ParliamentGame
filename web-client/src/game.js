var fb = require('./firebase');

// load recent outcomes
fb.database().ref('/outcomes').once(function(snap) {
	var outcomes = snap.val();
	outcomes.forEach(addOutcome);
});

// watch for new outcomes
fb.database().ref('/outcomes').on('child_added', function(snap) {
	addOutcome(snap.val());
});

fb.database().ref('/events').on('child_added', newEvent);

function submitVote(event) {
	var eventId = event.target.attr('name');
	var vote = event.target.val();
	firebase.database()
	.ref('/users/' + window.twitchUsername + '/votes/' + eventId)
	.set(vote);
}

$("#yes-btn").click(submitVote);
$("#no-btn").click(submitVote);

function newEvent(data) {
	var event = $("<p class='wordbreak'>");
	event.text(data.val().description);
	var choices = data.val().choices;
	$("#yes-btn").attr('name', data.key).val(choices[0].name).show();
	$("#no-btn").attr('name', data.key).val(choices[0].name).show();
	$("#decisionbox").html(event);
}

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