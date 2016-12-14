This game has 3 components: The Unity client, the python helper, and the web client.

The unity client is in the zip file. The python helper is FirebaseHelper.py
The web client is located at
https://tmsherman.github.io/ParliamentGame/

The python helper is a python3 script with dependencies. These are:
Pyrebase
python-osc

These can be installed with
pip3 install pyrebase
pip3 install python-osc

INSTRUCTIONS TO PLAY:
The Unity game should be launched first.
Then the python script should be launched with
python3 FirebaseHelper.py
in whatever directory it has been placed.

You should also open your stream now, if you’re going to stream the game.

Next, your audience members (or you if you’re just testing it) should open the web client.
The web client can be found at https://tmsherman.github.io/ParliamentGame/
It requires you to connect your twitch account to login, do this when the Unity and Python apps are open. When you do so, and every time you run the unity/python combo afterwards, you will be assigned a random role.

Each time you want to start the game over, you should relaunch the unity game and the python helper, in that order.

EXPLANATION OF SUBMISSION:
The Unity client is the streamer’s game. The web client is the audience’s game. The python helper helps them talk to eachother.

The prototype is a  proof of concept and will always present the same sequence of events, which lack the narrative requirements and links that would exist in the final game.
When the game is launched, it will immediately present an event to the streamer (but there is no time limit, so feel free to get everything else setup)
Some events are not for the streamer, but for the audience.
When the unity client says, for example, “The Nobles have a decision to make…”, it means that any users with the noble role will have a vote to make on the web client.
Whenever a choice is made, the kingdom stats get adjusted. You can highlight these in the unity game to view their values. They are usually hidden because showing the value of a stat on stream is meant to be a bargaining chip for the streamer to use. Web client users can at all times see the value stat associated with their role.
You should expect our prototype to run through the sequence of events, displaying an appropriate outcome for each of your choices, and send choices to the web client for voting at certain points
The branching choices are demonstrated with the noble event where they can propose a tax. If the nobles approve the tax (they will by default if there are no votes, as ties break in favor of YES), the king can change the tax or approve it. If he changes it, the nobles can approve his changes. These approval events won’t trigger if the nobles turn down the tax proposal, or if the king approves the tax as-is.

There is a convenient debug/playtest feature: You can hit space bar to skip an event without waiting for the timer or making a choice. If this causes things to break, oops! it is a secret debug feature after all.

Libraries this project uses:
Jorge Garcia Martin’s UnityOSC (although the OscSender and OscReceiver classes are not part of this package)
YamlDotNet for Unity by Fredrik Ludvigsen
Pyrebase by thisbejim on github
python-osc by attwad on github
moment.js (http://momentjs.com/)
webpack https://webpack.github.io/
