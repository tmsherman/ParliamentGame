using UnityEngine;
using System.Collections;

public class hoveredResource : MonoBehaviour
{
	public string stateKey;
	private RunParliamentGame rpg;
	private GameObject tooltip;

	void Start() {
		rpg = GameObject.Find ("EventRunner").GetComponent<RunParliamentGame> ();
		tooltip = GameObject.Find ("ResourceTooltip");
	}

	void OnMouseOver()
	{
		int value = rpg.getStateForKey (stateKey);
		print (gameObject.name + " " + value);

		tooltip.SetActive (true);
		tooltip.transform.position = new Vector3 (Input.mousePosition.x + 50, Input.mousePosition.y, 0);
	}
}