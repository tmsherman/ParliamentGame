using UnityEngine;
using System.Collections;

public class moveUpScript : MonoBehaviour {

	public bool activated = false;

	private GameObject mage;

	// Use this for initialization
	void Start () {
		mage = GameObject.Find ("Mage");
	}

	// Update is called once per frame
	void Update () {
		if (!activated)
			return;
		else if (mage.transform.position.y < 1.56)
			transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
	}

	public void Activate() {
		activated = true;
	}
}
