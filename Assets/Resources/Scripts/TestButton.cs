using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestButton : ControlButton {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	override public void DoStuff(){
		UnityEngine.Application.Quit();
		Debug.Log ("clicked");
	}
}
