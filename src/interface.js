var config = require('./config');
var moment = require('moment');

// Set Twitch chat and stream based on configured channel
$("#chat").attr("src", `http://www.twitch.tv/${config.channel}/chat?darkpopout`);
$("#stream").attr("src", `http://player.twitch.tv/?channel=colegleason`);

function setupTimer(end) {
	var interval = setInterval(function() {
		var now = moment.utc();
		if (now > end) {
			$('#timer').text('Voting over');
			clearInterval(interval);
		} else {
			var diff = end.diff(now, 'seconds');
			$('#timer').text(diff + ' seconds');
		}
	}, 1000);
}

module.exports.displayEvent = function(eventKey, event) {
	var start = moment.unix(event.utctime);
	var end = start.add(config.eventDuration, 'seconds');
	if (moment.utc() > end) {
		return;
	}
	console.log(event);
	var eventHtml = $("<p class='wordbreak'>");
	eventHtml.text(event.description);
	$("#yes-btn").attr('name', eventKey).val(event.choices[0].name).show();
	$("#no-btn").attr('name', eventKey).val(event.choices[1].name).show();
	$("#decisionbox").html(event);
	setupTimer(end);
}