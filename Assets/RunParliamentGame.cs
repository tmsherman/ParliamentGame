using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

	private bool waitingForDecision = false;

	private int numUsers = 0;
	private int lastClass = 0;
	private string[] userClasses = { "peasant", "merchant", "noble" };

	// Use this for initialization
		void Start () {
		eventDescription = GameObject.Find ("ChoiceStoryText");
		choice1Text = GameObject.Find ("Choice1Text");
		choice2Text = GameObject.Find ("Choice2Text");
		choice3Text = GameObject.Find ("Choice3Text");
		choice1 = GameObject.Find ("Choice1");
		choice2 = GameObject.Find ("Choice2");
		choice3 = GameObject.Find ("Choice3");

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
		lastClass = Random.Range (0, 3);
	}

	void LoadEvent(LoadYamlEvents.GameEvent e) {
		if (e.type == LoadYamlEvents.EVENT_TYPE.BAD) return;

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
		eventDescription.GetComponent<Text> ().text = "The " + typeText + " are faced with a decision...\n" + e.eventDescription;
	}

	// Update is called once per frame
	void Update () {
		//first event of the game
		if (currentEvent == "") {
			eventStorage = gameObject.GetComponent<LoadYamlEvents> ();
			advanceEvent ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			advanceEvent ();
		}
	
	}

	void advanceEvent() {
		currentEvent = eventQueue [0];
		eventQueue.RemoveAt (0);
		LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
		LoadEvent (e);

	}

	//these are triggered when you click on one of the choices.

	public void onChoice1() {
		if (!waitingForDecision) {
			advanceEvent ();
		} else {
			onChoice (0);
		}
	}

	public void onChoice2() {
		onChoice (1);
	}

	public void onChoice3() {
		onChoice (2);
	}

	//returns -1 if no key found.
	public int getStateForKey (string key) {
		if (!state.ContainsKey (key)) return -1;
		else return state [key];
	}

	void onChoice(int choiceNum) {
		LoadYamlEvents.GameEvent e = eventStorage.FetchEventByTag (currentEvent);
		LoadYamlEvents.Choice c = e.choices [choiceNum];
		state.Add (c.choiceTag, 1);
		for (int i = 0; i < c.stateChanges.Count; i++) {
			LoadYamlEvents.StateChange sc = c.stateChanges [i];
			if (state.ContainsKey (sc.key)) {
				state [sc.key] = state [sc.key] + sc.value;
			} else {
				state.Add (sc.key, sc.value);
			}
		}
		eventDescription.GetComponent<Text> ().text = c.outcomeText;
		choice1Text.GetComponent<Text> ().text = "OK";
		choice1.SetActive (true);
		choice2.SetActive (false);
		choice3.SetActive (false);
		waitingForDecision = false;
	}


	private string pickClassForNewUser() {
		numUsers++;
		lastClass += 1;
		if (lastClass >= userClasses.Length) lastClass = 0;
		return userClasses [lastClass];
	}
}
