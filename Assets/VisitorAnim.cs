using UnityEngine;
using System.Collections;

public class VisitorAnim : MonoBehaviour {

	public bool leave = true;

	private GameObject visitor;

	//Animation variables
	private Animator anim;
	private int leaveHash = Animator.StringToHash("leave");

	// Use this for initialization
	void Start () {
		visitor = GameObject.Find ("Visitor");
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (!leave) {
			if (visitor.transform.position.y < 80) {
				transform.position = new Vector3 (transform.position.x, transform.position.y + 4, transform.position.z);
			}
		} else {
			if (visitor.transform.position.y > -386F) {
				transform.position = new Vector3 (transform.position.x, transform.position.y - 4, transform.position.z);
			}
		}
	}

	public void enter() {
		anim.SetBool (leaveHash, false);
		leave = false;
	}

	public void depart() {
		anim.SetBool (leaveHash, true);
		leave = true;
	}
}