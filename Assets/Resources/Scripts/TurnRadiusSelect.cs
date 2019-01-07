using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnRadiusSelect : ControlButton {
	public TMP_InputField tmpi;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	override public void DoStuff(){
		tmpi.ActivateInputField();
		tmpi.MoveToStartOfLine(false,false);
	}
}
