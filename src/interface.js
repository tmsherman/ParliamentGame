var config = require('./config');
var user = require('./user');
var game = require('./game');

var moment = require('moment');

// initialize the interface by setting up chat, stream, etc.
module.exports.init = function() {
	// Set Twitch chat and stream based on configured channel
	$("#chat").attr("src", `https://www.twitch.tv/${config.channel}/chat?darkpopout`);
	$("#stream").attr("src", `https://player.twitch.tv/?channel=${config.channel}`);
	$("#yes-btn").click(submitVote);
	$("#no-btn").click(submitVote);
}

// handle vote button click
function submitVote(event) {
	var eventId = $(event.target).attr('name');
	var vote = $(event.target).val();
	user.vote(eventId, vote);
	disableVoteButtons();
}

// setup timer countdown for voting
function setupTimer(end) {
	var interval = setInterval(function() {
		var now = moment.utc();
		if (now > end) {
			// time's up! reset ui elements
			$('#timer').text('Voting Over');
			$("#decisionbox").html(null);
			disableVoteButtons();
			clearInterval(interval);
		} else {
			var diff = end.diff(now, 'seconds');
			$('#timer').text(diff + ' seconds');
		}
	}, 1000);
}

// change event description text, vote buttons, and then start timer
module.exports.displayEvent = function(eventKey, event) {
	var eventHtml = $("<p>");
	eventHtml.text(event.description);
	$("#yes-btn").attr('name', eventKey)
	  .val(event.choices[0].name)
	  .prop('disabled', false);
	$("#no-btn").attr('name', eventKey)
	  .val(event.choices[1].name)
	  .prop('disabled', false);
	$("#decisionbox").html(eventHtml);
	var start = moment.unix(event.utctime);
	var end = start.add(config.eventDuration, 'seconds');
	setupTimer(end);
}

// add a new outcome to the left sidebar
module.exports.addOutcome = function(data) {
	var outcome = $("<p>");
	outcome.text(data.text);
	outcome.append($('<br />'));
	// create colorful resource change indications
	// only for our resources
	Object.keys(data.changes).forEach(function(key) {
		var val = data.changes[key];
		var userRole = user.getUser().role;
		if (val != 0 && key == game.roleToResourceMap[userRole]) {
			var change = $("<span>");
			change.addClass(val < 0 ? 'neg-change' : 'pos-change');
			change.text((val < 0 ? '' : '+') + val + " " + key);
			outcome.append(change);
		}
	});
	// add to top of list
	$("#outcomebox").prepend(outcome);
}

// make vote buttons unclickable
function disableVoteButtons() {
	$("#yes-btn").prop('disabled', true);
	$("#no-btn").prop('disabled', true);
}
module.exports.disableVoteButtons = disableVoteButtons;

// change the percent bar level
module.exports.setStatsLevel = function(percent) {
	$('#stats-level').height(percent + '%');
}

// based on the user's role, change resource icon and role text
module.exports.setStatsSymbol = function(role) {
	var roleToSymbolMap = {
		'PEASANT': 'img/qol.png',
		'NOBLE': 'img/power.png',
		'MERCHANT': 'img/wealth.png'
	}
	$("#role-text").html($('<p>').text(role));
	$("#stats-symbol").attr('src', roleToSymbolMap[role]);
}
