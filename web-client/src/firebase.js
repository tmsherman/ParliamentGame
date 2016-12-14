var config = require('./config');

var firebase = require('firebase');

// Export firebase connection to database
module.exports = firebase.initializeApp(config.firebaseConfig);