
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainLogic : MonoBehaviour {

	public static MainLogic inst;
	public Camera arcamera;
	public Material ATUmat;
	public List<Material> GridMats = new List<Material>();
	private int grid = 4;
	public int tiltmode = 0; //none=0, left=1, right=2
	public int transparencymode =0;
	private bool transparencytoggle = false;
	private bool ATUActivetoggle = false;
	public List<UpdatingMaterial> updmats = new List<UpdatingMaterial>();
	public List<GameObject> ElectricToggleButtons = new List<GameObject>();

	public List<GameObject> MainRail = new List<GameObject>();
	public List<GameObject> MainRailFiller = new List<GameObject>();
	public List<GameObject> SideRail = new List<GameObject>();
	public List<GameObject> SideRailFiller = new List<GameObject>();
	public List<GameObject> Electric = new List<GameObject>();
	public List<GameObject> ElectricBoxes = new List<GameObject>();
	public List<GameObject> ElectricBoxesTall = new List<GameObject>();
	public List<GameObject> ElectricBoxesFiller = new List<GameObject>();
	public List<GameObject[,]> Grids =  new List<GameObject[,]>();

	public GameObject TiltLeft;
	public GameObject TiltLeftOff;
	public GameObject TiltRight;
	public GameObject TiltRightff;
	public GameObject TiltNone;
	public GameObject TiltNoneOff;

	public HashSet<ATUManager> managers = new HashSet<ATUManager>();

	public int ATUTurnScaling = 0;
	public int ATUTurnTiltScaling = 0;

	//tilt: 10mm=0, 20mm=1, ...,180mm=17 -> index= (int)((t mm/10)-1)
	//height: index= (h-1)/0.5
	private int[,] AtuTiltScalingTable = new int[18,7] 
	{{6,9,13,16,19,22,25},{13,19,25,31,38,44,50},{19,28,38,47,56,66,75},{25,38,50,63,75,88,100},{31,47,63,78,94,109,125},{38,56,75,94,113,131,150},
		{44,66,88,109,131,153,175},{50,75,100,125,150,175,200},{56,84,113,141,169,197,225},{63,94,125,156,188,219,250},{69,103,138,172,206,241,275},{75,113,150,188,225,263,300},
		{81,122,163,203,244,284,325},{88,131,175,219,263,306,350},{94,141,188,234,281,328,375},{100,150,200,250,300,350,400},{106,159,213,266,319,372,425},{113,169,225,281,338,394,450}};

	//public GameObject[,] Grids
	public bool mainrail=false;
    public int elecspeedhigh = 0; //0 = non-electric, 1 = slow, 2 = fast

    public List<GameObject> menus = new List<GameObject>();
	public TextMeshPro turnrad;
	public TextMeshPro turntilt;
	public TextMeshPro objectheight;


	// Use this for initialization
	void Start ()
    {
        inst = this;
        elecspeedhigh = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetKeyDown(KeyCode.Escape)))
		{
			UnityEngine.Application.Quit();
		}
	}


	//Allows other scripts to easily access the MainLogic
	public static MainLogic GetInst(){
		return inst;
	}

	/*
	 * Cycles between the different states of the ATU model: solid, transparent, hollow & hidden 
	 */
	public void ChangeATUMode(){
		switch (transparencymode) {
		case(0):
			ChangeATUTransparency ();
			transparencymode = 1;
			break;
		case(1):
			ChangeATUTransparency ();
			ToggleATUHollow ();
			transparencymode = 2;
			break;
		case(2):
			ToggleATUHollow ();
			ToggleATUActive ();
			transparencymode = 3;
			break;
		case(3):
			ToggleATUActive ();
			transparencymode = 0;
			break;
		}
	}

	//Toggles the entire ATU model on and off
	public void ToggleATUActive(){
		foreach(ATUManager am in managers){
			am.LRFGroups [0].SetActive (ATUActivetoggle);
			am.LRFGroups [1].SetActive (ATUActivetoggle);
			am.LRFGroups [2].SetActive (ATUActivetoggle);
		}
		ATUActivetoggle = !ATUActivetoggle;
	}

	//Toggles between fully solid and 35% solid color for the ATU model renderers
	public void ChangeATUTransparency(){
		if (transparencytoggle) {
			ATUmat.color = new Color(ATUmat.color.r,ATUmat.color.g,ATUmat.color.b,1.0f);
		} else {
			ATUmat.color = new Color(ATUmat.color.r,ATUmat.color.g,ATUmat.color.b,.35f);
		}
		transparencytoggle = !transparencytoggle;
		foreach(UpdatingMaterial umat in updmats){
			umat.UpdateMaterialColor (ATUmat.color);
		}
	}

	//
	public void ChangeATUColor(Color c){
		ATUmat.color = new Color(c.r, c.g, c.b, ATUmat.color.a);
		foreach(UpdatingMaterial umat in updmats){
			umat.UpdateMaterialColor (ATUmat.color);
		}

	}

	//Toggles which of Mainrail & Siderail objects (and their child objects) are active on the ATU model
	//The objects in each group are the parts unique to the ATU for that type of rail section
	public void ToggleRailType(){
		if (mainrail) {
			foreach (GameObject r in MainRail) {
				r.SetActive (false);
			}
			foreach (GameObject r in SideRail) {
				r.SetActive (true);
			}
			mainrail = !mainrail;
		} else {
			foreach (GameObject r in MainRail) {
				r.SetActive (true);
			}
			foreach (GameObject r in SideRail) {
				r.SetActive (false);
			}
			mainrail = !mainrail;
		}
	}

	//Toggles on and off the parts of the ATU model specific to electric rails
	public void ToggleElectricRail(bool onoff){
        foreach (GameObject r in Electric) {
			r.SetActive (onoff);
		}
			ElectricToggleButtons [0].SetActive (!onoff);
			ElectricToggleButtons [1].SetActive (onoff);
		if (!onoff) {
			ElectricToggleButtons [2].SetActive (onoff);
			ElectricToggleButtons [3].SetActive (!onoff);
			ElectricToggleButtons [4].SetActive (onoff);
			ElectricToggleButtons [5].SetActive (!onoff);
		}
	}

	//Toggles between the ATU parts for high- and low speed electric rail sections
	public void ToggleElectricRailSpeed(bool high){
        for (int i = 0; i < ElectricBoxes.Count; i++) {
			ElectricBoxes [i].SetActive (!high);
		}
		for (int i = 0; i < ElectricBoxesTall.Count; i++) {
			ElectricBoxesTall [i].SetActive (high);
		}
		ElectricToggleButtons [2].SetActive (!high);
		ElectricToggleButtons [3].SetActive (high);
		ElectricToggleButtons [4].SetActive (high);
		ElectricToggleButtons [5].SetActive (!high);
	}

	//ATU model parts have an outer shell (~5cm on full size model in correct scale) and an inner filler object to form the solid model
	//This toggles the inner object on and off to allow for viewing a hollowed ATU
	public void ToggleATUHollow(){
		for (int i = 0; i < ElectricBoxesFiller.Count; i++) {
			ElectricBoxesFiller [i].SetActive (!ElectricBoxesFiller [i].activeSelf);
		}
		for (int i = 0; i < MainRailFiller.Count; i++) {
			MainRailFiller [i].SetActive (!MainRailFiller [i].activeSelf);
		}
		for (int i = 0; i < SideRailFiller.Count; i++) {
			SideRailFiller [i].SetActive (!SideRailFiller [i].activeSelf);
		}
	}

	//Goes through the different measuring grid options, starting from 'none', then descending sizes and back to 'none'
	public void ToggleGrid(){
		grid--;
		if (grid < 0) {
			grid = 4;
		}
		foreach (GameObject[,] gridset in Grids) {
			for (int i = 0; i < 4; i++) {
				if (grid == i) {
					gridset [0, i].SetActive(true);
					gridset [1, i].SetActive (true);
				} else {
					gridset [0, i].SetActive(false);
					gridset [1, i].SetActive(false);
				}
			}
		}
	}

	//Opens and closes the menu for rail type settings
	public void ToggleRailMenu(int target){
		for (int i = 0; i < menus.Count; i++) {
			if (i != target) {
				menus[i].SetActive (false);
			}
		}
		menus[target].SetActive (!menus[target].activeSelf);
	}

	//in local scale, 1 Unity unit == 1 mm
	public void WidenATU(){
		int width;
		if (int.TryParse (turnrad.text, out width)) {
			Debug.Log ("Turn radius: "+width);
			ATUScaling (width);
			foreach (ATUManager am in managers) {
				am.LRFGroups [0].transform.localPosition = new Vector3(ATUTurnScaling+0,0,0); //Left: x>0
				am.LRFGroups [1].transform.localPosition= new Vector3(-(ATUTurnScaling+0),0,0); //Right: x<0
			}
		}
	}

	//Converts radius of turn (m) to ATU model widening (mm)
	public void ATUScaling(int turnrad){
		ATUTurnScaling = 5;
		//not ideal, but large ranges are not supported for a switch-case structure
		if (turnrad >4000 && turnrad <= 7000) {
			ATUTurnScaling = 10;
		}
		else if (turnrad>2500){
			ATUTurnScaling = 15;
		}
		else if (turnrad>1800){
			ATUTurnScaling = 20;
		}
		else if (turnrad>1500){
			ATUTurnScaling = 25;
		}
		else if (turnrad>1200){
			ATUTurnScaling = 30;
		}
		else if (turnrad>900){
			ATUTurnScaling = 35;
		}
		else if (turnrad>800){
			ATUTurnScaling = 40;
		}
		else if (turnrad>700){
			ATUTurnScaling = 45;
		}
		else if ( turnrad>600){
			ATUTurnScaling = 55;
		}
		else if ( turnrad>500){
			ATUTurnScaling = 60;
		}
		else if (turnrad>400){
			ATUTurnScaling = 75;
		}
		else if ( turnrad>300){
			ATUTurnScaling = 90;
		}
		else if (turnrad>250){
			ATUTurnScaling = 120;
		}
		else if (turnrad>200){
			ATUTurnScaling = 145;
		}
		else if (turnrad>180){
			ATUTurnScaling = 180;
		}
		else if (turnrad>150){
			ATUTurnScaling = 200;
		}
		else if (turnrad>125){
			ATUTurnScaling = 240;
		}
		else if (turnrad>100){
			ATUTurnScaling = 290;
		}
		else if (turnrad>90){
			ATUTurnScaling = 360;
		}
		else if (turnrad<=90){
			ATUTurnScaling = 450;
		}
	}

	//Attempts to parse the turn tilt values into int & float and if successfull calls ATUTiltScale to use them to modify the model
	public void ATUTiltScalePrep(){
		int tilt;
		if (int.TryParse (turntilt.text, out tilt)) {

			float height;
			if (float.TryParse (objectheight.text, out height)) {
				ATUTiltScale (tilt, height, tiltmode);
			}
		}
	}

	//Applies ATU tilt-based scaling in turns
	//Scaling based off of bothe turn radius (m) and height of object (m), using the values supplied in VR Track documentation (stored in array)
	public void ATUTiltScale(int tilt,float height, int side){
		if (tilt > 180) {
			tilt = 180;
		}
		int tiltindex = (tilt / 10)-1;
		if (tiltindex >0) {
			if (height >= 1) {
				if (height > 4) {
					height = 4;
				}
				int heightindex = (int)(height - 1) * 2;
				Debug.Log ("idx1: " + tiltindex + " idx2:" + heightindex);
				ATUTurnTiltScaling = AtuTiltScalingTable[tiltindex,heightindex];
				if (side > 0 && side < 3) {
					foreach (ATUManager am in managers) {
						switch(side){
						case(2):
							am.LRFGroups [0].transform.localPosition = new Vector3((ATUTurnScaling+ATUTurnTiltScaling),0,0);
							break;
						case(1):
							am.LRFGroups [1].transform.localPosition = new Vector3(-(ATUTurnScaling+ATUTurnTiltScaling),0,0);
							break;
						default:
							break;
						}
					}
				}	
			}
		}
	}

	//Updates ATU model tilt buttons and the tiltmode state used to handle the one-sided scaling
	public void ToggleTiltButtons (int pressed, bool onoff)
	{
		switch (pressed) {
		case(0): //left
			if(onoff){
				tiltmode = 0;
			} else {
				tiltmode = 1;
			}
			TiltLeft.SetActive (!onoff);
			TiltRight.SetActive (false);
			TiltNone.SetActive (onoff);
			TiltLeftOff.SetActive (onoff);
			TiltRightff.SetActive (true);
			TiltNoneOff.SetActive (!onoff);
			break;

		case(1): //right
			if(onoff){
				tiltmode = 0;
			} else {
				tiltmode = 2;
			}
			TiltLeft.SetActive (false);
			TiltRight.SetActive (!onoff);
			TiltNone.SetActive (onoff);
			TiltLeftOff.SetActive (true);
			TiltRightff.SetActive (onoff);
			TiltNoneOff.SetActive (!onoff);
			break;

		default: //center
			tiltmode = 0;
			if(!onoff){
				TiltLeft.SetActive (onoff);
				TiltRight.SetActive (onoff);
				TiltNone.SetActive (!onoff);
				TiltLeftOff.SetActive (!onoff);
				TiltRightff.SetActive (!onoff);
				TiltNoneOff.SetActive (onoff);
			}
			break;
		}
	}

}
