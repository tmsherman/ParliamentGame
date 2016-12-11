using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OscReceiver : MonoBehaviour {

	[HideInInspector] public List<float> messages;
	[HideInInspector] public bool newMessageThisFrame;



	public string handlerServerAddress;
	public string address;
	// Use this for initialization

	private long lastTimeStamp = 0;

	[SerializeField] private int numberOfInputs = 3;

	[SerializeField] private float defaultInputValue = 1f;

	static bool didInit = false;

	void Start () {
		if (!didInit) {
			OSCHandler.Instance.Init ();
			didInit = true;
		}
	
		messages = new List<float> ();
		for (int i = 0; i < numberOfInputs; i++) {
			messages.Add (defaultInputValue);
		}
		if (Application.isEditor) {
			Application.runInBackground = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
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
		print ("HELLO FROM PYTHON!");

		lastTimeStamp = serverLog.server.LastReceivedPacket.TimeStamp;
		messages.Clear ();
		UnityOSC.OSCPacket packet = serverLog.server.LastReceivedPacket;
		for (int i = 0; i < packet.Data.Count; i++) {
			messages.Add((int)packet.Data [i]);
			//print ((float)packet.Data [i]);
		}
		gameObject.GetComponent<OscSender> ().Send ("/volume", 11);
	}
}
