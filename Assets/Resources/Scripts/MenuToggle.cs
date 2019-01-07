using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuToggle : ControlButton {
	public int menu = 0;
	public GameObject menuobj;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	//stayon is used to keep the relevant menu button in a 'clicked' state while the menu is open
	void Update () {
		stayon = menuobj.activeSelf;
	}

	//Opens and closes setting menus
	override public void DoStuff(){
		AccessMain().ToggleRailMenu(menu);
	}
}
