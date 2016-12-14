using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;


public class LoadYamlEvents : MonoBehaviour
{

	//These are the data structures we will use to load and store all of our events.

	public enum EVENT_TYPE { STREAMER, PEASANT, MERCHANT, NOBLE, BAD };

	public struct Requirement {
		public string tag;
		public int value;
	}

	public struct StateChange {
		public string key;
		public int value;
	}

	public struct Choice {
		public string choiceText;
		public string choiceTag;
		public List<Requirement> choiceRequirements;
		public List<StateChange> stateChanges;
		public string nextEventTag;
		public string outcomeText;
	}

	public struct GameEvent {
		public string eventDescription;
		public string eventTag;
		public List<Requirement> eventRequirements;
		public List<Choice> choices;
		public EVENT_TYPE type;
	}

	public static GameEvent BAD_EVENT;

	private List<GameEvent> rulerEvents = new List<GameEvent> ();
	private List<GameEvent> peasantEvents = new List<GameEvent> ();
	private List<GameEvent> merchantEvents = new List<GameEvent> ();
	private List<GameEvent> nobleEvents = new List<GameEvent> ();


	// Use this for initialization
	void Start ()
	{
		Debug.Log ("started");

		BAD_EVENT = new GameEvent();
		BAD_EVENT.eventDescription = "BAD EVENT";
		BAD_EVENT.eventTag = "BAD_EVENT";
		BAD_EVENT.type = EVENT_TYPE.BAD;


		//load all our events.
		LoadEventsFromFolder ("RulerEvents", ref rulerEvents);
		LoadEventsFromFolder ("PeasantEvents", ref peasantEvents);
		LoadEventsFromFolder ("MerchantEvents", ref merchantEvents);
		LoadEventsFromFolder ("NobleEvents", ref nobleEvents);

		Debug.Log ("all done");

	}

	public GameEvent FetchEventByTag (string eventTag) {
		foreach (GameEvent e  in rulerEvents) {
			if (e.eventTag == eventTag) {
				return e;
			}
		}
		foreach (GameEvent e  in peasantEvents) {
			if (e.eventTag == eventTag) {
				return e;
			}
		}
		foreach (GameEvent e  in merchantEvents) {
			if (e.eventTag == eventTag) {
				return e;
			}
		}
		foreach (GameEvent e  in nobleEvents) {
			if (e.eventTag == eventTag) {
				return e;
			}
		}
		return BAD_EVENT;
	}

	//Load events from Yaml, and fill up the passed in list. There's a specific yaml format you have to follow with the events.
	//folderName is in the Resources folder.
	void LoadEventsFromFolder(string folderName, ref List<GameEvent> list) {
		UnityEngine.Object[] loadedEvents = Resources.LoadAll(folderName);
		//print ("from folder " + folderName + ", loadedEvents length: " + loadedEvents.Length);

		for (int i = 0; i < loadedEvents.Length; i++) {
			TextAsset ta = (loadedEvents [i] as TextAsset);
			print (ta.name);
			LoadEventFromText (ta.text, ref list);
		}

	}

	void LoadEventFromText(string txt, ref List<GameEvent> list) {
		StringReader input = new StringReader (txt);
		YamlStream yaml = new YamlStream ();
		yaml.Load (input);

		// Examine the stream
		var mapping =
			(YamlMappingNode)yaml.Documents [0].RootNode;

		var output = new StringBuilder ();
		foreach (var entry in mapping.Children) {
			output.AppendLine (((YamlScalarNode)entry.Key).Value);
		}

		var items = (YamlSequenceNode)mapping.Children [new YamlScalarNode ("events")];
		foreach (YamlMappingNode item in items) {
			GameEvent e = new GameEvent();
			e.eventDescription = item.Children [new YamlScalarNode ("eventDescription")].ToString();
			e.eventTag = item.Children [new YamlScalarNode ("eventTag")].ToString ();
			string type = item.Children [new YamlScalarNode ("eventType")].ToString ();
			if (type == "streamer") {
				e.type = EVENT_TYPE.STREAMER;
			} else if (type == "noble") {
				e.type = EVENT_TYPE.NOBLE;
			} else if (type == "peasant") {
				e.type = EVENT_TYPE.PEASANT;
			} else if (type == "merchant") {
				e.type = EVENT_TYPE.MERCHANT;
			} else {
				e.type = EVENT_TYPE.BAD;
			}
			e.eventRequirements = new List<Requirement>();
			var eventRequirements = new YamlSequenceNode(item.Children [new YamlScalarNode ("eventRequirements")]);
			foreach (YamlMappingNode requirement in eventRequirements ) {
				foreach(var key in requirement.Children.Keys) {
					Requirement r = new Requirement ();
					r.tag = key.ToString ();
					if (requirement.Children [key].ToString () == "true") {
						r.value = 1;
					} else if (requirement.Children [key].ToString () == "false") {
						r.value = 0;
					} else {
						r.value = Int32.Parse (requirement.Children [key].ToString ());
					}
					//print (r.tag + ", " + r.value);
					e.eventRequirements.Add (r);
				}
			}
			e.choices = new List<Choice> ();
			var choices = (YamlSequenceNode)item.Children [new YamlScalarNode ("choices")];
			//print(choices.ToString());
			foreach (YamlMappingNode choice in choices ) {
				print (choice);
				Choice c = new Choice ();
				c.choiceText = choice.Children[new YamlScalarNode ("choiceText")].ToString ();
				c.choiceTag = choice.Children [new YamlScalarNode ("choiceTag")].ToString ();
				c.outcomeText = choice.Children [new YamlScalarNode ("outcomeText")].ToString ();
				c.choiceRequirements = new List<Requirement> ();
				var choiceRequirements = new YamlSequenceNode(choice.Children [new YamlScalarNode ("choiceRequirements")]);
				foreach (YamlMappingNode requirement in choiceRequirements ) {
					foreach(var key in requirement.Children.Keys) {
						Requirement r = new Requirement ();
						r.tag = key.ToString ();
						if (requirement.Children [key].ToString () == "true") {
							r.value = 1;
						} else if (requirement.Children [key].ToString () == "false") {
							r.value = 0;
						} else {
							r.value = Int32.Parse (requirement.Children [key].ToString ());
						}
						//print (r.tag + ", " + r.value);
						c.choiceRequirements.Add (r);
					}
				}

				c.stateChanges = new List<StateChange> ();
				var stateChanges = new YamlSequenceNode(choice.Children [new YamlScalarNode ("stateChanges")]);
				foreach (YamlMappingNode stateChange in stateChanges ) {
					foreach(var key in stateChange.Children.Keys) {
						StateChange sc = new StateChange ();
						sc.key = key.ToString ();
						if (stateChange.Children [key].ToString () == "true") {
							sc.value = 1;
						} else if (stateChange.Children [key].ToString () == "false") {
							sc.value = 0;
						} else {
							sc.value = Int32.Parse (stateChange.Children [key].ToString ());
						}
						//print (sc.key + ", " + sc.value);
						c.stateChanges.Add (sc);
					}
				}
				e.choices.Add (c);
			}

			list.Add(e);
		}
		//Debug.Log (output);




		Debug.Log ("finished");

	}

	// Update is called once per frame
	void Update ()
	{
	
	
	}
}
