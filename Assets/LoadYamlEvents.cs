using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

using YamlDotNet.RepresentationModel;

public class LoadYamlEvents : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("started");

		//TextAsset mydata = Resources.Load ("yamlexample") as TextAsset;
		TextAsset mytxtData = Resources.Load("yamlexample") as TextAsset;
		string txt=mytxtData.text;

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
			output.AppendLine (
				String.Format ("{0}/t{1}",
					item.Children [new YamlScalarNode ("event-description")],
					item.Children [new YamlScalarNode ("event-tag")]
				)
			);
		}
		Debug.Log (output);

			


		Debug.Log ("finished");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
