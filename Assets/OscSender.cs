using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OscSender : MonoBehaviour {

	public string handlerServerAddress;

	// Use this for initialization
	void Start () {
		/*HandController handController = GetComponentInParent<HandController> ();
		handController.GetLeapController ().SetPolicy (Leap.Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);*/
	}
	
	// Update is called once per frame
	void Update () {
		/*HandController handController = GetComponentInParent<HandController> ();

		List<object> values = new List<object>();


		HandModel[] hands = handController.GetAllGraphicsHands ();
		if (hands.Length == 0)
			return;

		HandModel hand = hands [0];

		string valString = "";
		//there should be 5 fingers...
		for (int i = 0; i < hand.fingers.Length; i++) {
			FingerModel finger = hand.fingers [i];
			Vector3 tip = finger.GetTipPosition();
			values.Add (tip.x);
			values.Add (tip.y);
			values.Add (tip.z);
			valString += " " + tip.x + " " + tip.y + " " + tip.z;
		}
			

		Vector3 pos = hand.GetPalmPosition ();

		values.Add (pos.x);
		values.Add (pos.y);
		values.Add (pos.z);

		Quaternion q = hand.GetPalmRotation ();

		values.Add (q.w);
		values.Add (q.x);
		values.Add (q.y);
		values.Add (q.z);



		//print (valString);

		//print (values.Count);
*/
		//OSCHandler.Instance.SendMessageToClient(handlerServerAddress, "/wek/inputs", values);
	}

	public void Send(string address, int value) {
		OSCHandler.Instance.SendMessageToClient (handlerServerAddress, address, value);
	}

	public void Send(string address, List<object> values) {
		OSCHandler.Instance.SendMessageToClient (handlerServerAddress, address, values);

	}
}
