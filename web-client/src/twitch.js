Twitch.init({clientId: 'fj6deq9ja4sqx8thrxncuzikk8s3xhx'}, function(error, status) {
  if (error) {
    // error encountered while loading
    console.log(error);
  }
  // the sdk is now loaded
  if (status.authenticated) {
    // user is currently logged in
	$('#login').hide()
  }
});

$('.twitch-connect').click(function() {
  Twitch.login({
    scope: ['user_read', 'channel_read']
  });
})