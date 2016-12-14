using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OscSender : MonoBehaviour {

	public string handlerServerAddress;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Send(string address, int value) {
		OSCHandler.Instance.SendMessageToClient (handlerServerAddress, address, value);
	}

	public void Send(string address, List<object> values) {
		OSCHandler.Instance.SendMessageToClient (handlerServerAddress, address, values);

	}
}
