using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RunParliamentGame : MonoBehaviour {

	private LoadYamlEvents eventStorage;

	private List<string> eventQueue = new List<string>(); //organized by tag. Hard-coded for now, but would be randomly generated in a later iteration.

	private string currentEvent = "";


	//these are the UI fields we need to interact with. public so we can assign them in editor.
	private GameObject eventDescription;
	private GameObject choice1;
	private GameObject choice2;
	private GameObject choice3; //hardcoded 3 choices for now.

	// Use this for initialization
	void Start () {
		eventDescription = GameObject.Find ("ChoiceStoryText");
		choice1 = GameObject.Find ("Choice1Text");
		choice2 = GameObject.Find ("Choice2Text");
		choice3 = GameObject.Find ("Choice3Text");

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
		GameObject currentChoice = choice1; //get the compiler to shut up.
		for (int i = 0; i < 3; i++) {
			if (i == 0) {
				currentChoice = choice1;
			} else if (i == 1) {
				currentChoice = choice2;
			} else if (i == 2) {
				currentChoice = choice3;
			}
			//hide the choice if it's not needed.
			if (i >= e.choices.Count) {
				currentChoice.SetActive (false);
				continue;//go to the next choice
			}
			currentChoice.SetActive (true);
			LoadYamlEvents.Choice c = e.choices [i];
			currentChoice.GetComponent<Text> ().text = c.choiceText;
		}

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

	public void onChoice1() {

	}

	public void onChoice2() {

	}

	public void onChoice3() {

	}
}
