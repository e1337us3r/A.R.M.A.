
public class TiltToggleButton : ControlButton {
	public int type = 0;
	public bool onoff=false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Updates the tilt buttons, calls for normal ATU scaling based off turn radius and the one-sided scaling for inwards tilt in turns, if any.
	//All 3 buttons use the same script, their type determines
	override public void DoStuff(){
		MainLogic.GetInst().ToggleTiltButtons(type, onoff);
		AccessMain ().WidenATU();
		AccessMain ().ATUTiltScalePrep();
	}
}
