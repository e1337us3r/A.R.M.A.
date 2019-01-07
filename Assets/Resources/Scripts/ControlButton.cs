using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlButton : MonoBehaviour {
	/*
	 * The base class for all UI buttons, handling mouse clicks and holds using OnMouseOver() and Input.GetMouseButton().
	 * Child classes are responsible for implementing specific functionality with DoStuff() overrides.
	 * Typically DoStuff tasks the MainLogic with handling the task so each button does not need references to the same objects.
	 * This class also handles behavior common to all buttons.
	 */

	//is the button 'On click' type or 'Click and hold' type
	public bool holdable = false;

	//handles changing the look of the circular menu buttons in response to input
	public MeshRenderer rend;
	public Material[] icons = new Material[3];
	public bool stayon = false;
	public int icon = 0;
	private int iconnow = 0;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	/*
	 * Changes button textures to "hightlight" appearance on mouseover, to "click" appearance in response to mouseclick
	 * OnMouseOver means the cursor is over the button, so only this button is reacting to Input.GetMouseButton(0) state
	 * The application does not currently use click-and-hold style buttons (holdable==true) for anything
	 * Any such buttons should implement an appropriate repeat delay timer for their action in their DoStuff method, as by default it happens once per frame
	 */
	void OnMouseOver() { 
		icon = 1;
		if (Input.GetMouseButton (0)) {
			icon = 2;
			if (holdable) {
				DoStuff();
			}
		}
		if(Input.GetMouseButtonUp(0)){
			Debug.Log ("Mouseup");
			DoStuff();
			icon = 0;
		}
		if (stayon) {
			icon = 2;
		}
		if (icon != iconnow) {
			iconnow = icon;
			if (rend != null) {
				rend.material = icons [iconnow];
			}
		}
	}

	//Every UI button does their taskS by overriding this
	virtual public void DoStuff(){
	}

	//used by UI buttons to access MainLogic for their DoStuff override functionality
	protected MainLogic AccessMain(){
		return MainLogic.GetInst();
	}
}
