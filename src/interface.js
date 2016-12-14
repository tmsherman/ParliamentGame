var config = require('./config');
var user = require('./user');
var game = require('./game');

var moment = require('moment');

module.exports.init = function() {
	// Set Twitch chat and stream based on configured channel
	$("#chat").attr("src", `https://www.twitch.tv/${config.channel}/chat?darkpopout`);
	$("#stream").attr("src", `https://player.twitch.tv/?channel=${config.channel}`);
	$("#yes-btn").click(submitVote);
	$("#no-btn").click(submitVote);
}

function submitVote(event) {
	var eventId = $(event.target).attr('name');
	var vote = $(event.target).val();
	user.vote(eventId, vote);
	disableVoteButtons();
}

function setupTimer(end) {
	var interval = setInterval(function() {
		var now = moment.utc();
		if (now > end) {
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

module.exports.addOutcome = function(data) {
	var outcome = $("<p>");
	outcome.text(data.text);
	outcome.append($('<br />'));
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
	$("#outcomebox").prepend(outcome);
}

function disableVoteButtons() {
	$("#yes-btn").prop('disabled', true);
	$("#no-btn").prop('disabled', true);
}
module.exports.disableVoteButtons = disableVoteButtons;

module.exports.setStatsLevel = function(percent) {
	$('#stats-level').height(percent + '%');
}

module.exports.setStatsSymbol = function(role) {
	var roleToSymbolMap = {
		'PEASANT': 'img/qol.png',
		'NOBLE': 'img/power.png',
		'MERCHANT': 'img/wealth.png'
	}
	$("#stats-symbol").attr('src', roleToSymbolMap[role]);
}
