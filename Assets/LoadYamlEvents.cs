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
	}

	public struct StreamerEvent {
		public string eventDescription;
		public string eventTag;
		public List<Requirement> eventRequirements;
		public List<Choice> choices;
	}

	private List<StreamerEvent> streamerEvents = new List<StreamerEvent> ();


	// Use this for initialization
	void Start ()
	{
		Debug.Log ("started");

		//TextAsset mydata = Resources.Load ("yamlexample") as TextAsset;
		UnityEngine.Object[] loadedEvents = Resources.LoadAll("YamlEvents");
		print ("loadedEvents length: " + loadedEvents.Length);
		TextAsset mytxtData = Resources.Load("yamlexample") as TextAsset;
		string txt=mytxtData.text;
		LoadEventFromText (txt);

		for (int i = 0; i < loadedEvents.Length; i++) {
			TextAsset ta = (loadedEvents [i] as TextAsset);
			print (ta.name);
			LoadEventFromText (ta.text);
		}

	}

	void LoadEventFromText(string txt) {
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

		//var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
		//var events = deserializer.Deserialize<StreamerEvent


		var items = (YamlSequenceNode)mapping.Children [new YamlScalarNode ("events")];
		foreach (YamlMappingNode item in items) {
			StreamerEvent e = new StreamerEvent();
			e.eventDescription = item.Children [new YamlScalarNode ("eventDescription")].ToString();
			e.eventTag = item.Children [new YamlScalarNode ("eventTag")].ToString ();
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
					print (r.tag + ", " + r.value);
					e.eventRequirements.Add (r);
				}
			}
			e.choices = new List<Choice> ();
			var choices = (YamlSequenceNode)item.Children [new YamlScalarNode ("choices")];
			print(choices.ToString());
			foreach (YamlMappingNode choice in choices ) {
				print (choice);
				Choice c = new Choice ();
				c.choiceText = choice.Children[new YamlScalarNode ("choiceText")].ToString ();
				c.choiceTag = choice.Children [new YamlScalarNode ("choiceTag")].ToString ();
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
						print (r.tag + ", " + r.value);
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
						print (sc.key + ", " + sc.value);
						c.stateChanges.Add (sc);
					}
				}
			}

			streamerEvents.Add(e);
		}
		Debug.Log (output);




		Debug.Log ("finished");

	}

	// Update is called once per frame
	void Update ()
	{
	
	
	}
}
