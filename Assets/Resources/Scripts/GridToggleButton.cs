using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridToggleButton : ControlButton {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Calls for the MainLogic to cycle through the different measuring grid options: none, from largest to smallest and back to none.
	override public void DoStuff(){
		MainLogic.GetInst ().ToggleGrid ();
	}
}
