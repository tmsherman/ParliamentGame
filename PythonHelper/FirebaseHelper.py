import pyrebase
import argparse
import math
import threading
from pythonosc import dispatcher
from pythonosc import osc_server
from pythonosc import osc_message_builder
from pythonosc import udp_client
import time;
from multiprocessing import Queue;

def print_volume_handler(unused_addr, args, volume):
  print("[{0}] ~ {1}".format(args[0], volume))

def print_compute_handler(unused_addr, args, volume):
  try:
    print("[{0}] ~ {1}".format(args[0], args[1](volume)))
  except ValueError: pass

def post_event_to_firebase(unused_addr, args, description, eventtag, eventType, choice0tag, choice1tag, eventNum, UTCtime):
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
  #args[0].put(data)
  #args[0].put(eventNum)
  #db.child("events").child(eventNum).set(data)

def post_resources_to_firebase(unused_addr, args, military, magic, diplomacy, wealth, power, happiness):
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
  #args[0].put(data)
  #print(args[0].qsize())
  #db.child("state").child("resources").set(data)

def post_user_type_to_firebase(unused_addr, username, userType):
  #data = {"type": userType}
  print (username + " " + userType)
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("users").child(username+"/type").set(userType)

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
    if "votes" in u:
      if eventNum in u["votes"]:
        voteTag = u[votes][eventNum]
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


def post_vote_outcome_to_firebase(unused_addr, numEvents, outcomeText, happinessChange, wealthChange, powerChange, eventType):
  data = {"text": outcomeText,
          "changes": {"happiness": happinessChange, "wealth": wealthChange, "power": powerChange},
          "type" : eventType
  }
  global config
  firebase = pyrebase.initialize_app(config)
  db = firebase.database()
  db.child("outcomes").child(numEvents).set(data);



eventsQ = Queue()
resourcesQ = Queue()

parser = argparse.ArgumentParser()
parser.add_argument("--ip",
    default="127.0.0.1", help="The ip to listen on")
parser.add_argument("--port",
    type=int, default=60512, help="The port to listen on")
args = parser.parse_args()

dispatcher = dispatcher.Dispatcher()
dispatcher.map("/newEvent", post_event_to_firebase, eventsQ)
dispatcher.map("/resources", post_resources_to_firebase, resourcesQ)
dispatcher.map("/usertype", post_user_type_to_firebase)
dispatcher.map("/getvotes", query_votes_from_firebase)
dispatcher.map("/volume", print_volume_handler, "Volume")
dispatcher.map("/logvolume", print_compute_handler, "Log volume", math.log)

server = osc_server.ForkingOSCUDPServer(
    (args.ip, args.port), dispatcher)
print("Serving on {}".format(server.server_address))
server_thread = threading.Thread(target=server.serve_forever)
server_thread.start()

time.sleep(1)

parser = argparse.ArgumentParser()
parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
parser.add_argument("--port", type=int, default=11050, help="The port the OSC server is listening on")
args = parser.parse_args()

client = udp_client.SimpleUDPClient(args.ip, args.port)
#    client.send_message("/filter", random.random())

client.send_message("/hello", 1)

config = {
  "apiKey": "AIzaSyBtGj_zMzADQ3vRUnZH29rbIZJpv6YCgDw",
  "authDomain": "parliament-6f09c.firebaseapp.com",
  "databaseURL": "https://parliament-6f09c.firebaseio.com",
  "storageBucket": "parliament-6f09c.appspot.com"
}

firebase = pyrebase.initialize_app(config)

db = firebase.database()

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
    send_username(username)

my_stream = db.child("users").stream(user_stream_handler, stream_id="new_users")

while True:
  #print(resourcesQ.qsize() + time.Now())
  while not resourcesQ.empty():
    data = resourcesQ.get()
    print("out of q: " + data)
    db.child("state").child("resources").set(data)
  while not eventsQ.empty():
    data = eventsQ.get()
    num = eventsQ.get()
    db.child("events").child(num).set(data)

my_stream.close()
server.shutdown()