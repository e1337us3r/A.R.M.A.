using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRailType : ControlButton {
	public GameObject other;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Calls for Mainlogic to switch between displaying the ATU model parts unique to mainrail and siderail, also updates its own button state,
	override public void DoStuff(){
		AccessMain().ToggleRailType();
		other.SetActive (!other.activeSelf);
		gameObject.SetActive (!gameObject.activeSelf);
	}
}
