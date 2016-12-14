using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class RunParliamentGame : MonoBehaviour {

	private LoadYamlEvents eventStorage;

	private List<string> eventQueue = new List<string>(); //organized by tag. Hard-coded for now, but would be randomly generated in a later iteration.

	private string currentEvent = "";

	//we'll use this to store all the game state.
	private Dictionary<string, int> state = new Dictionary<string, int>();

	//these are the UI fields we need to interact with. public so we can assign them in editor.
	private GameObject eventDescription;
	private GameObject choice1;
	private GameObject choice2;
	private GameObject choice3;
	private GameObject choice1Text;
	private GameObject choice2Text;
	private GameObject choice3Text; //hardcoded 3 choices for now.

	private GameObject timerText;

	private GameObject visitor;

	private bool waitingForDecision = false;

	private int numUsers = 0;
	private int lastClass = 0;
	private string[] userClasses = { "PEASANT", "MERCHANT", "NOBLE" };

	//we use these to communicate with our python helper (and thus Firebase)
	private OscReceiver voteReceiver;
	private OscReceiver userReceiver;

	private OscSender sender;

	private int numEvents = -1;

	private float voteTime = 0f;

	private float voteTimeLimit = 30f;

	private bool sentForVotes = false;

	// Use this for initialization
		void Start () {
		eventDescription = GameObject.Find ("ChoiceStoryText");
		choice1Text = GameObject.Find ("Choice1Text");
		choice2Text = GameObject.Find ("Choice2Text");
		choice3Text = GameObject.Find ("Choice3Text");
		choice1 = GameObject.Find ("Choice1");
		choice2 = GameObject.Find ("Choice2");
		choice3 = GameObject.Find ("Choice3");
		timerText = GameObject.Find ("TimerText");
		timerText.SetActive (false);
		visitor = GameObject.Find ("Visitor");

		voteReceiver = GameObject.Find("VoteReceiver").GetComponent<OscReceiver> ();
		userReceiver = GameObject.Find("UserReceiver").GetComponent<OscReceiver> ();

		sender = gameObject.GetComponent<OscSender> ();

		eventQueue.Add ("haunting1");
		eventQueue.Add ("war1");
		eventQueue.Add ("black-market");
		eventQueue.Add ("gossip");
		eventQueue.Add ("propose-tax");
		eventQueue.Add ("change-tax");
		eventQueue.Add ("approve-tax-increase");
		eventQueue.Add ("approve-tax-decrease");
		eventQueue.Add ("popular-strike");
		eventQueue.Add ("dragon");
		eventQueue.Add ("streamer-death");

		//set up initial state:
		state.Add ("military", 20);
		state.Add ("diplomacy", 20);
		state.Add ("magic", 20);
		state.Add ("happiness", 30);
		state.Add ("wealth", 30);
		state.Add ("power", 30);

		//we'll cycle through our array to give out classes, but start on a random one.
		lastClass = UnityEngine.Random.Range (0, 3);
	}

	void LoadEvent(LoadYamlEvents.GameEvent e) {
		if (e.type == LoadYamlEvents.EVENT_TYPE.BAD) return;

		numEvents++;

		if (e.type == LoadYamlEvents.EVENT_TYPE.STREAMER) {
			LoadStreamerEvent (e);
		}

		if (e.type == LoadYamlEvents.EVENT_TYPE.MERCHANT ||
			e.type == LoadYamlEvents.EVENT_TYPE.NOBLE ||
			e.type == LoadYamlEvents.EVENT_TYPE.PEASANT) {
			LoadCrowdEvent (e);
		}

	}

	void LoadStreamerEvent(LoadYamlEvents.GameEvent e) {
		print ("load streamer event " + e.eventTag + " choice count: " + e.choices.Count);
		eventDescription.GetComponent<Text> ().text = e.eventDescription;
		GameObject currentChoiceText = choice1Text; //get the compiler to shut up.
		GameObject currentChoice = choice1;
		for (int i = 0; i < 3; i++) {
			if (i == 0) {
				currentChoiceText = choice1Text;
				currentChoice = choice1;
			} else if (i == 1) {
				currentChoiceText = choice2Text;
				currentChoice = choice2;
			} else if (i == 2) {
				currentChoiceText = choice3Text;
				currentChoice = choice3;
			}
			//hide the choice if it's not needed.
			if (i >= e.choices.Count) {
				currentChoice.SetActive (false);
				continue;//go to the next choice
			}
			currentChoice.SetActive (true);
			LoadYamlEvents.Choice c = e.choices [i];
			currentChoiceText.GetComponent<Text> ().text = c.choiceText;
		}
		waitingForDecision = true;
		visitor.GetComponent<VisitorAnim> ().enter ();
	}

	void LoadCrowdEvent(LoadYamlEvents.GameEvent e) {
		print ("load crowd event " + e.eventTag + " choice count: " + e.choices.Count);
		string typeText = "";
		if (e.type == LoadYamlEvents.EVENT_TYPE.MERCHANT) {
			typeText = "Merchants";
		} else if (e.type == LoadYamlEvents.EVENT_TYPE.NOBLE) {
			typeText = "Nobles";
		} else if (e.type == LoadYamlEvents.EVENT_TYPE.PEASANT) {
			typeText = "Peasants";
		} else {
			typeText = "ERROR: BAD TYPE";
		}
		eventDescription.GetComponent<Text> ().text = "The " + typeText + " are faced with a decision...\n\n" + e.eventDescription;
		List<object> data = new List<object> ();
		data.Add (e.eventDescription);
		data.Add (e.eventTag);
		data.Add (e.type.ToString());
		data.Add (e.choices [0].choiceTag);
		data.Add (e.choices [1].choiceTag);
		data.Add (numEvents);
		data.Add ((Int32)(DateTime.UtcNow.Subtract (new DateTime (1970, 1, 1))).TotalSeconds);

		//hide the streamer buttons
		choice1.SetActive (false);
		choice2.SetActive (false);
		choice3.SetActive (false);

		//show the timer
		timerText.SetActive (true);
		timerText.GetComponent<Text> ().text = voteTime.ToString("F1");
		voteTime = voteTimeLimit;
		sender.Send("/newEvent", data);
		sentForVotes = false;
	}

	// Update is called once per frame
	void Update () {
		//first event of the game
		if (currentEvent == "") {
			eventStorage = gameObject.GetComponent<LoadYamlEvents> ();
			advanceEvent ();
		}

		//if there's a timer, update it
		if (voteTime > 0) {
			voteTime -= Time.deltaTime;
			timerText.GetComponent<Text> ().text = voteTime.ToString("F1");
			if (voteTime <= 0) {
				timerText.GetComponent<Text> ().text = "0.0";
				if (!sentForVotes) {
					List<object> data = new List<object> ();
					data.Add (numEvents);
					LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
					data.Add (e.choices [0].choiceTag);
					data.Add (e.choices [1].choiceTag);
					sender.Send ("/getvotes", data);
					sentForVotes = true;
				}
			}
		}

		//SUPER SECRET DEBUG COMMAND!
		if (Input.GetKeyDown (KeyCode.Space)) {
			advanceEvent ();
		}
	
		//if we received a vote total this frame, send an outcome out and make the choice locally.
		if (voteReceiver.newMessageThisFrame) {
			int choice0votes = (int)voteReceiver.messages [0];
			int choice1votes = (int)voteReceiver.messages [1];
			//yes wins in the event of a tie
			LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
			List<object> data = new List<object> ();
			if (choice0votes >= choice1votes) {
				LoadYamlEvents.Choice c = e.choices[0];
				data.Add (numEvents);
				data.Add (c.outcomeText);
				int happinessChange = 0;
				int wealthChange = 0;
				int powerChange = 0;
				for (int i = 0; i < c.stateChanges.Count; i++) {
					if (c.stateChanges [i].key == "happiness") {
						happinessChange = c.stateChanges [i].value;
					}
					if (c.stateChanges [i].key == "wealth") {
						wealthChange = c.stateChanges [i].value;
					}
					if (c.stateChanges [i].key == "power") {
						powerChange = c.stateChanges [i].value;
					}
				}
				data.Add (happinessChange);
				data.Add (wealthChange);
				data.Add (powerChange);
				data.Add (e.type.ToString());
				sender.Send ("/votechoiceoutcome", data);
				onChoice (0);
			} else {
				LoadYamlEvents.Choice c = e.choices[1];
				data.Add (numEvents);
				data.Add (c.outcomeText);
				int happinessChange = 0;
				int wealthChange = 0;
				int powerChange = 0;
				for (int i = 0; i < c.stateChanges.Count; i++) {
					if (c.stateChanges [i].key == "happiness") {
						happinessChange = c.stateChanges [i].value;
					}
					if (c.stateChanges [i].key == "wealth") {
						wealthChange = c.stateChanges [i].value;
					}
					if (c.stateChanges [i].key == "power") {
						powerChange = c.stateChanges [i].value;
					}
				}
				data.Add (happinessChange);
				data.Add (wealthChange);
				data.Add (powerChange);
				data.Add (e.type.ToString());
				sender.Send ("/votechoiceoutcome", data);
				onChoice (1);
			}
		}
		//if we got new users this frame, give each one a new usertype.
		//BUG - sometimes this doesnt give every user a new type. No idea why. It's just added randomness for now, game still functions.
		if (userReceiver.newMessageThisFrame) {
			while (userReceiver.messages.Count > 0) {
				object username = userReceiver.messages [0];
				string usertype = pickClassForNewUser ();
				List<object> data = new List<object> ();
				data.Add (username);
				data.Add (usertype);
				sender.Send ("/usertype", data);
				userReceiver.messages.RemoveAt (0);
			}
		}
	}

	//load the next event in the queue.
	void advanceEvent() {
		currentEvent = eventQueue [0];
		eventQueue.RemoveAt (0);
		LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
		LoadEvent (e);

	}

	//button callback, and the OK button during an outcome
	public void onChoice1() {
		if (!waitingForDecision) {
			advanceEvent ();
		} else {
			onChoice (0);
		}
	}
	//button callback
	public void onChoice2() {
		onChoice (1);
	}
	//button callback
	public void onChoice3() {
		onChoice (2);
	}

	//returns -1 if no key found. in the future, this will let us check if certain events have happened or choices have been made previously, to determine if certain events and choices are eligible ones.
	public int getStateForKey (string key) {
		if (!state.ContainsKey (key)) return -1;
		else return state [key];
	}


	//this changes the game to account for a choice being made. - shows outcome, saves values, makes state changes.
	void onChoice(int choiceNum) {
		LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
		LoadYamlEvents.Choice c = e.choices [choiceNum];
		state.Add (c.choiceTag, 1);
		if (c.choiceTag == "quit-game") { 
			Application.Quit ();
		}

		for (int i = 0; i < c.stateChanges.Count; i++) {
			LoadYamlEvents.StateChange sc = c.stateChanges [i];
			if (state.ContainsKey (sc.key)) {
				state [sc.key] = state [sc.key] + sc.value;
			} else {
				state.Add (sc.key, sc.value);
			}
		}

		//in the future we'll have different choices lead into certain events using what's in YAML, but we hardcoded it here because it only works for certain events.
		if (c.choiceTag == "propose-tax-yes") {
			eventQueue.Insert (0, "change-tax");
		}
		if (c.choiceTag == "change-tax-increase") {
			eventQueue.Insert (0, "approve-tax-increase");
		}
		if (c.choiceTag == "change-tax-decrease") {
			eventQueue.Insert (0, "approve-tax-decrease");
		}

		postResourcesToFirebase ();

		//if its not a streamer outcome, we want to tell the streamer and viewers that.
		string typeText = "";
		if (e.type == LoadYamlEvents.EVENT_TYPE.MERCHANT) {
			typeText = "Merchants";
		} else if (e.type == LoadYamlEvents.EVENT_TYPE.NOBLE) {
			typeText = "Nobles";
		} else if (e.type == LoadYamlEvents.EVENT_TYPE.PEASANT) {
			typeText = "Peasants";
		} else {
			typeText = "ERROR: BAD TYPE";
		}
		if (e.type != LoadYamlEvents.EVENT_TYPE.STREAMER) {
			eventDescription.GetComponent<Text> ().text = "The outcome of the " + typeText + " decision...\n\n" + e.eventDescription;
			eventDescription.GetComponent<Text> ().text = c.outcomeText;
		} else { //it is a streamer outcome, just show the outcome text.
			eventDescription.GetComponent<Text> ().text = c.outcomeText;
		}
		//OK to advance through the outcome text. hide other buttons.
		choice1Text.GetComponent<Text> ().text = "OK";
		choice1.SetActive (true);
		choice2.SetActive (false);
		choice3.SetActive (false);
		waitingForDecision = false;
		voteTime = 0;
		timerText.SetActive (false);
		visitor.GetComponent<VisitorAnim> ().depart ();
	}



	private string pickClassForNewUser() {
		numUsers++;
		lastClass += 1;
		if (lastClass >= userClasses.Length) lastClass = 0;
		return userClasses [lastClass];
	}

	private void postResourcesToFirebase() {
		List<object> data = new List<object> ();
		data.Add (state["military"]);
		data.Add (state["magic"]);
		data.Add (state["diplomacy"]);
		data.Add (state["wealth"]);
		data.Add (state["power"]);
		data.Add (state["happiness"]);

		sender.Send("/resources", data);

	}
}
