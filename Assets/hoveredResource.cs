using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class hoveredResource : MonoBehaviour
{
	public string stateKey;
	public int numInorder;
	private RunParliamentGame rpg;
	private GameObject tooltip;

	void Start() {
		rpg = GameObject.Find ("EventRunner").GetComponent<RunParliamentGame> ();
		tooltip = GameObject.Find ("ResourceTooltip");
	}
	//show a tooltip when we mouseover a resource, and hide it when we mouseoff.
	void OnMouseOver()
	{
		int value = rpg.getStateForKey (stateKey);

		tooltip.SetActive (true);
		tooltip.transform.position = new Vector3 (110 * numInorder, -210, 0);
		tooltip.GetComponentInChildren<Text> ().text = stateKey.Substring (0, 1).ToUpper () + stateKey.Substring (1) + ": " + value;
	}

	void OnMouseExit() {
		tooltip.SetActive (false);
	}
}