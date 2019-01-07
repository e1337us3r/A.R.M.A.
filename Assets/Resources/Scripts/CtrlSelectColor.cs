using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlSelectColor : ControlButton {

	Color buttoncolor;
	Renderer rendr;

	// Use this for initialization
	//The color of the button object is used for the ATU model, making it simple to expand the color selection by cloning a button and changing the color
	void Start () {
		rendr = GetComponentInParent<Renderer> ();
		buttoncolor = rendr.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Color of the button is passed to MainLogic which assigns it to all ATU model parts with the UpdatingMaterial script
	override public void DoStuff(){
		AccessMain().ChangeATUColor(buttoncolor);
	}
}
