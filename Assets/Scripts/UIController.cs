using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	// Use this for initialization
	public RectTransform currentTab;

	void Start () {
		makeTabSelected(currentTab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void makeTabSelected(RectTransform tab) {
		// Deselect current tab
		currentTab.gameObject.GetComponent<Image> ().color = new Color (0.8f, 0.95f, 0.97f, 1);
		currentTab.gameObject.transform.GetChild (0).GetComponent<Text> ().color = new Color(0.125f, 0.125f, 0.125f, 1);

		// Select the input tab
		tab.gameObject.GetComponent<Image> ().color = new Color (0.45f, 0.55f, 0.75f, 1);
		tab.gameObject.transform.GetChild (0).GetComponent<Text> ().color = Color.white;

		// Save a reference to the input tab
		currentTab = tab;
	}
}
