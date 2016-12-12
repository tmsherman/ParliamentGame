﻿using UnityEngine;
using System.Collections;

public class moveDownScript : MonoBehaviour {

	public bool activated = false;

	private GameObject guyINeedToTouchOrReferenceOrWhatever;

	// Use this for initialization
	void Start () {
		guyINeedToTouchOrReferenceOrWhatever = GameObject.Find ("Mage");
	}
	
	// Update is called once per frame
	void Update () {
		if (!activated)
			return;
		if (guyINeedToTouchOrReferenceOrWhatever.transform.position.y < 0)
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
	}

	public void Activate() {
		activated = true;
	}
}
