using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlToggleTransparency : ControlButton {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Calls for MainLogic to switch cycle between the different ATU model states: solid, transparent, hollow, hidden and back to solid.
	override public void DoStuff(){
		AccessMain().ChangeATUMode();
	}
}
