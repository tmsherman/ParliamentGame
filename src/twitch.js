var fb = require('./firebase');

Twitch.init({clientId: 'fj6deq9ja4sqx8thrxncuzikk8s3xhx'}, function(error, status) {
  if (error) {
    // error encountered while loading
    console.log(error);
  }
  // the sdk is now loaded
  if (status.authenticated) {
    // user is currently logged in
    $('#login').hide()
    Twitch.api({method: 'channel'}, function(error, channel) {
      fb.database().ref('users/' + channel.name).set({status: 'online'});
    });
  }
});

$('.twitch-connect').click(function() {
  Twitch.login({
    scope: ['user_read', 'channel_read']
  });
})