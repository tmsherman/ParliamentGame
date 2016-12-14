var fb = require('./firebase');
var user = require('./user');
var config = require('./config');

// Initialize the API and then fetch the user information
Twitch.init({clientId: config.twitchClientId}, function(error, status) {
  if (error) {
    // error encountered while loading
    console.log(error);
  }
  // the sdk is now loaded
  if (status.authenticated) {
    // user is currently logged in
    $('#login').hide();
    Twitch.api({method: 'channel'}, function(error, channel) {
      user.initOrLoadUser(channel.name);
    });
  }
});

// Wire up the login button
$('.twitch-connect').click(function() {
  Twitch.login({
    scope: ['user_read', 'channel_read']
  });
})