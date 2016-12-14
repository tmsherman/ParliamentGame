import pyrebase
import argparse
import math
import threading
from pythonosc import dispatcher
from pythonosc import osc_server
from pythonosc import osc_message_builder
from pythonosc import udp_client
import time;

#THE BELOW ARE ALL OSC CALLBACKS. THEY USE THREADING, AND WE HAD SSL TROUBLES SHARING A CONNECTION BETWEEN THREADS
#SO WE MAKE A NEW CONNECTION FOR EACH CALLBACK. DOESNT SEEM TO BE TOO EXPENSIVE.

#put an event on firebase for users to vote on.
def post_event_to_firebase(unused_addr, description, eventtag, eventType, choice0tag, choice1tag, eventNum, UTCtime):
  data = {"description": description,
          "name": eventtag,
          "type": eventType,
          "choices": [{"name": choice0tag}, {"name":choice1tag}],
          "utctime": UTCtime
  }
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("events").child(eventNum).set(data)
  print (data)

#send the current resource state to firebase to update the stats.
def post_resources_to_firebase(unused_addr, military, magic, diplomacy, wealth, power, happiness):
  data = {"military": military,
          "magic": magic,
          "diplomacy": diplomacy,
          "wealth": wealth,
          "power": power,
          "happiness": happiness
  }
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("state").child("resources").set(data)

  print (data)

#send a user their role through firebase.
def post_user_type_to_firebase(unused_addr, username, userType):
  #data = {"type": userType}
  print (username + " " + userType)
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("users").child(username+"/type").set(userType)

#look up votes for an event on firebase.
def query_votes_from_firebase(unused_addr, eventNum, choice0tag, choice1tag):
  print("querying votes for " + str(eventNum) + " - 0: " + choice0tag + " - 1: " + choice1tag)
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  users = db.child("users").get();
  choice0votes = 0
  choice1votes = 0

  for pair in users.each():
    u = pair.val()
    print (u)
    if "votes" in u:
      if str(eventNum) in u["votes"]:
        voteTag = u["votes"][str(eventNum)]
        if voteTag == choice0tag:
          choice0votes += 1
        elif voteTag == choice1tag:
          choice1votes += 1

  print("0: " + str(choice0votes) + " 1: " + str(choice1votes))
  parser = argparse.ArgumentParser()
  parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
  parser.add_argument("--port", type=int, default=11050, help="The port the OSC server is listening on")
  args = parser.parse_args()
  client = udp_client.SimpleUDPClient(args.ip, args.port)
  votes = [choice0votes, choice1votes]
  client.send_message("/votes", votes)

#send the result text after a vote has finished to firebase.
def post_vote_outcome_to_firebase(unused_addr, numEvents, outcomeText, happinessChange, wealthChange, powerChange, eventType):
  #quick and dirty hack to fix some OSC library unsigning some ints
  if happinessChange > 0x7fffffff:
    happinessChange = happinessChange - 4294967296
  if wealthChange > 0x7fffffff:
    wealthChange = wealthChange - 4294967296
  if powerChange > 0x7fffffff:
    powerChange = powerChange - 4294967296

  data = {"text": outcomeText,
          "changes": {"happiness": happinessChange, "wealth": wealthChange, "power": powerChange},
          "type" : eventType
  }
  print(data)
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("outcomes").child(numEvents).set(data);


#END OF OSC CALLBACKS

#this is how you setup the python-osc for whatever reason.
parser = argparse.ArgumentParser()
parser.add_argument("--ip",
    default="127.0.0.1", help="The ip to listen on")
parser.add_argument("--port",
    type=int, default=60512, help="The port to listen on")
args = parser.parse_args()

#map osc addresses to callback functions
dispatcher = dispatcher.Dispatcher()
dispatcher.map("/newEvent", post_event_to_firebase)
dispatcher.map("/resources", post_resources_to_firebase)
dispatcher.map("/usertype", post_user_type_to_firebase)
dispatcher.map("/getvotes", query_votes_from_firebase)
dispatcher.map("/votechoiceoutcome", post_vote_outcome_to_firebase)

server = osc_server.ForkingOSCUDPServer(
    (args.ip, args.port), dispatcher)
print("Serving on {}".format(server.server_address))
server_thread = threading.Thread(target=server.serve_forever)
server_thread.start()

time.sleep(1)

#make an osc client to send to unity
parser = argparse.ArgumentParser()
parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
parser.add_argument("--port", type=int, default=11050, help="The port the OSC server is listening on")
args = parser.parse_args()

client = udp_client.SimpleUDPClient(args.ip, args.port)

#config for firebase
config = {
  "apiKey": "AIzaSyBtGj_zMzADQ3vRUnZH29rbIZJpv6YCgDw",
  "authDomain": "parliament-6f09c.firebaseapp.com",
  "databaseURL": "https://parliament-6f09c.firebaseio.com",
  "storageBucket": "parliament-6f09c.appspot.com"
}

firebase = pyrebase.initialize_app(config)

db = firebase.database()

#we want to initialize the game a bit.

db.child("events").remove()
db.child("outcomes").remove()
db.child("state").remove()
users = db.child("users").get();
for pair in users.each():
  u = pair.val()
  if "votes" in u:
    db.child("users").child(pair.key()).child("votes").remove()


#OSC FUNCTIONS TO SEND TO UNITY, AND WATCH FOR NEW USERS ON FIREBASE
def send_username(username):
  print(username)
  parser = argparse.ArgumentParser()
  parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
  parser.add_argument("--port", type=int, default=11050, help="The port the OSC server is listening on")
  args = parser.parse_args()
  client = udp_client.SimpleUDPClient(args.ip, args.port)
  client.send_message("/getusertype", username)

def send_usernames(usernames):
  for username in usernames:
    print(username)
  parser = argparse.ArgumentParser()
  parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
  parser.add_argument("--port", type=int, default=11050, help="The port the OSC server is listening on")
  args = parser.parse_args()
  client = udp_client.SimpleUDPClient(args.ip, args.port)
  client.send_message("/getusertype", usernames)


#this function does some complicated string parsing and it really doesn't matter why it works the way it does.
def user_stream_handler(message):
  print(message["event"])
  print(message["path"])
  if message["path"].count("/") > 1:
    return

  usernames = []
  print(message["data"])
  if message["data"] is not None:
    for key in message["data"].keys():
      username = key
      if username == "status":
        username = message["path"][1:]
      usernames.append(username)
    send_usernames(usernames)
  else:
    username = message["path"][1:]
    if username is "":
     return
    send_username(username)

#END OF OSC FUNCTIONS AND STREAM CALLBACK

#the stream function lets us watch a firebase area for events, so we can see when new users join.
my_stream = db.child("users").stream(user_stream_handler, stream_id="new_users")

#loop forever.
while True:
  a = True;

my_stream.close()
server.shutdown()