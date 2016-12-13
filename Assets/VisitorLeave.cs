using UnityEngine;
using System.Collections;

public class VisitorLeave : MonoBehaviour {

	public bool activated = false;

	private GameObject visitor;

	// Use this for initialization
	void Start () {
		visitor = GameObject.Find ("Visitor");
	}

	// Update is called once per frame
	void Update () {
		if (!activated)
			return;
		if (visitor.transform.position.y > -5.5F)
			transform.position = new Vector3 (transform.position.x, transform.position.y - 0.04F, transform.position.z);
		else
			activated = false;
	}

	public void Activate() {
		activated = true;
	}
}
