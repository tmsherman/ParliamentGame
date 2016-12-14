using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OscReceiver : MonoBehaviour {

	//public members so we can access data from OSC in other places.
	[HideInInspector] public List<object> messages;
	[HideInInspector] public bool newMessageThisFrame;



	public string handlerServerAddress;
	public string address;

	private long lastTimeStamp = 0;

	[SerializeField] private int numberOfInputs = 3;

	[SerializeField] private float defaultInputValue = 1f;

	static bool didInit = false;

	void Start () {
		if (!didInit) {
			OSCHandler.Instance.Init ();
			didInit = true;
		}
	
		messages = new List<object> ();
		for (int i = 0; i < numberOfInputs; i++) {
			messages.Add (defaultInputValue);
		}
		if (Application.isEditor) {
			Application.runInBackground = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		newMessageThisFrame = false;
		OSCHandler.Instance.UpdateLogs();
	
		ServerLog serverLog;
		OSCHandler.Instance.Servers.TryGetValue (handlerServerAddress, out serverLog);

		if (serverLog.server.LastReceivedPacket == null)
			return;
		if (serverLog.server.LastReceivedPacket.TimeStamp == lastTimeStamp)
			return;
		if (serverLog.server.LastReceivedPacket.Address != address)
			return;

		//new message received! do stuff!
		newMessageThisFrame = true;

		lastTimeStamp = serverLog.server.LastReceivedPacket.TimeStamp;
		messages.Clear ();
		UnityOSC.OSCPacket packet = serverLog.server.LastReceivedPacket;
		for (int i = 0; i < packet.Data.Count; i++) {
			messages.Add(packet.Data [i]);
			print (packet.Data [i]);
		}
	}
}
