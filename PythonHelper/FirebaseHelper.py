import pyrebase
import argparse
import math
import threading
from pythonosc import dispatcher
from pythonosc import osc_server
from pythonosc import osc_message_builder
from pythonosc import udp_client
import time;

def print_volume_handler(unused_addr, args, volume):
  print("[{0}] ~ {1}".format(args[0], volume))

def print_compute_handler(unused_addr, args, volume):
  try:
    print("[{0}] ~ {1}".format(args[0], args[1](volume)))
  except ValueError: pass


parser = argparse.ArgumentParser()
parser.add_argument("--ip",
    default="127.0.0.1", help="The ip to listen on")
parser.add_argument("--port",
    type=int, default=60512, help="The port to listen on")
args = parser.parse_args()

dispatcher = dispatcher.Dispatcher()
dispatcher.map("/filter", print)
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

events = db.child("events").get()
#print(events.val())

key = 1;

data = {"description": "this is a really cool test event that tim wrote right now on Sunday December 11th",
			   "name": "tims-cool-test-event",
			   "type": "peasant",
			   "choices": [{"name":"tims-cool-first-choice"}, {"name":"tims-cool-second-choice"}]}
db.child("events").child(key).set(data)

events = db.child("events").get()
#print(events.val())

a = True

while a:
	b = True

server.shutdown()