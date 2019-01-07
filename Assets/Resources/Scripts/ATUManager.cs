
using System.Collections.Generic;
using UnityEngine;

public class ATUManager : MonoBehaviour {
	//This is applied to the topmost container object of the ATU model hierarchy
	//it is intended to automatically sets up the objects below it with the correct scripts to hook into the UI ATU controls
	public GameObject[] LRFGroups = new GameObject[3];
	public int gridlimit = 4;
	private GameObject[,] Grids = new GameObject[2,4];
	private bool LateStartDone = false;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!LateStartDone) {
			LateStartDone = true;
			LateStart ();
			MainLogic.GetInst().managers.Add (this);
		}
	}

	//This is done at first update so that MainLogic.Start() has finished
	private void LateStart(){

		MapModelFromList(GetPaths(this.gameObject.transform.GetChild(0).gameObject, this.gameObject.transform.GetChild(0).gameObject.name).ToArray());

		RecursiveScriptDeployment (transform.Find("ATUPieces").gameObject);
		for (int i = 0; i < 4; i++) {
			Grids [0, i].GetComponent<Renderer> ().material = MainLogic.GetInst ().GridMats[i];
			Grids [1, i].GetComponent<Renderer> ().material = MainLogic.GetInst ().GridMats[i];
		}
		MainLogic.GetInst ().Grids.Add (Grids);
		MainLogic.GetInst().ToggleRailType ();
	}

	/*
	 * Deploys 'UpdatingMaterial' script to each GameObject with a Renderer that is not a measuring grid (as indicated by name)
	 * UpdatingMaterial enables changing color and transparency of the ATU model by creating a new color with the desired properties and delivering that to the objects
	 */
	private void RecursiveScriptDeployment(GameObject go){
		if (go.transform.childCount > 0) {
			for (int i = 0; i < go.transform.childCount; i++) {
				RecursiveScriptDeployment(go.transform.GetChild(i).gameObject);
			}
		}
		if (go.GetComponent<Renderer> () != null && !go.name.Contains ("Grid")) {
			go.GetComponent<Renderer> ().material = MainLogic.GetInst ().ATUmat;
			go.AddComponent<UpdatingMaterial> ();
		} 
	}

	/*
	 * Assigns the ATU model objects to the correct lists based on their path in the hierarchy
	 * This is set up to work with the VR Track ATU model at the time of the project
	 * A fully adaptable version would use Dictionary<string, List<GameObject> to group the objects
	 * Such a version would also require procedural generation of UI elements, hence it was not attempted for Minimum Viable Product
	 */

	public void MapModelFromList(string[] paths){
		MainLogic M = MainLogic.GetInst();
		int leftgrids = 0;
		int rightgrids = 0;
		for (int i = 0; i < paths.Length; i++) {
			if (paths[i].ToLower().Contains("mainrail")) {
				if (paths[i].ToLower ().Contains ("fill")) {
					M.MainRailFiller.Add (GetTargetPart (paths [i]));
				} else {
					M.MainRail.Add(GetTargetPart(paths[i]));
				}
			} else if (paths[i].ToLower().Contains("siderail")) {
				if (paths[i].ToLower ().Contains ("fill")) {
					M.SideRailFiller.Add(GetTargetPart(paths[i]));
				} else {
					M.SideRail.Add(GetTargetPart(paths[i]));
				}
			}else if (paths[i].ToLower().Contains("top")) {
				if (paths[i].ToLower ().Contains ("box")) {
					if (paths[i].ToLower ().Contains ("fill")) {
						M.ElectricBoxesFiller.Add(GetTargetPart(paths[i]));
					} else {
						if (paths [i].ToLower ().Contains ("tall")) {
							M.ElectricBoxesTall.Add (GetTargetPart (paths [i]));
						} else {
							M.ElectricBoxes.Add (GetTargetPart (paths [i]));
						}
					}
				} else {
					if (paths [i].ToLower ().Contains ("tall")) {
						M.ElectricBoxesTall.Add (GetTargetPart (paths [i]));
					} else if (paths [i].ToLower ().Contains ("short")){
						M.ElectricBoxes.Add (GetTargetPart (paths [i]));
					}else {
						M.Electric.Add(GetTargetPart(paths[i]));
					}
				}
			}else if (paths[i].ToLower().Contains("grid") && paths[i].ToLower().Contains("mm")) {
				if (paths[i].ToLower ().Contains ("left") && leftgrids < gridlimit) {
					Grids[0,leftgrids] = GetTargetPart(paths[i]);
					leftgrids++;
				} else if( rightgrids < gridlimit){
					Grids[1,rightgrids] = GetTargetPart(paths[i]);
					rightgrids++;
				}
			}else if (paths[i].ToLower().Contains("midfiller")) {
				LRFGroups[2] = GetTargetPart(paths[i]);
				M.MainRailFiller.Add (GetTargetPart (paths [i]));
			}else if (paths[i].ToLower().Contains("partleft") && !paths[i].ToLower().Contains("grid")) {
				LRFGroups[0] = GetTargetPart(paths[i]);
			}else if (paths[i].ToLower().Contains("partright") && !paths[i].ToLower().Contains("grid")) {
				LRFGroups[1] = GetTargetPart(paths[i]);
			}
		}
	}

	/*
	 * Retrieves the target part by 
	 */
	public GameObject GetTargetPart(string path){
		List<string> pathparts = new List<string>();
		pathparts.AddRange (path.Split ('/'));
		if (pathparts.Count > 0) {
			Transform target = transform.Find (pathparts[0]);
			pathparts.RemoveAt (0);
			while (pathparts.Count > 0) {
				target = target.transform.Find (pathparts[0]);
				pathparts.RemoveAt (0);
				//FindTargetPart(target,pathparts);
			}
			return target.gameObject;
		}
		return null;
	}

	/*
	 * Traverses a gameobject hierarchy under the given root object and returns the path to each object in that hierarchy
	 * Used to get the context of each object in the model hierarchy so it can be assigned to the right group
	 * Future versions could instead have all groupings of an object be based off its own name only
	 */
	public List<string> GetPaths(GameObject root, string pathtohere, bool addself=false){
		List<string> paths = new List<string> ();
		if (addself) {
			pathtohere += "/" + root.name;
			paths.Add (pathtohere);
			Debug.Log (pathtohere);
		}
		if (root.transform.childCount > 0) {
			for(int i=0;i<root.transform.childCount;i++)
			{
				List<string> childpaths =  GetPaths(root.transform.GetChild(i).gameObject, pathtohere, true);
				paths.AddRange (childpaths);
			}
		}
		return paths;
	}

	//past this point are old hardcoded version of automatically mapping ATU pieces to the correct collections by name

	/*/ 
	private void OldATUMapping(){
		//for toggling between the mainrail/siderail ATUs
		MainLogic.GetInst().MainRail.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUMainrailLeft"));
		MainLogic.GetInst().MainRail.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUMainrailClearanceBoxLeft"));
		MainLogic.GetInst().MainRailFiller.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUMainrailLeft/ATUMainrailFillLeft"));
		MainLogic.GetInst().MainRail.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUMainrailRight"));
		MainLogic.GetInst().MainRail.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUMainrailClearanceBoxRight"));
		MainLogic.GetInst().MainRailFiller.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUMainrailRight/ATUMainrailFillRight"));

		MainLogic.GetInst().SideRail.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUSiderailLeft"));
		MainLogic.GetInst().SideRailFiller.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUSiderailLeft/ATUSiderailFillLeft"));
		MainLogic.GetInst().SideRail.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUSiderailRight"));
		MainLogic.GetInst().SideRailFiller.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUSiderailRight/ATUSiderailFillRight"));

		//For toggling all electric rail parts of the ATU for electric/non-electric rails
		//for toggling between the different electric rail clearances as based off the speed of the train
		MainLogic.GetInst().Electric.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUTopElectricLeft"));
		MainLogic.GetInst().Electric.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUTopElectricRight"));
		MainLogic.GetInst().Electric.Add(GetTargetPart("ATUPieces/ATUMidFiller/ATUTopFiller"));

		MainLogic.GetInst().ElectricBoxes.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxTallLeft"));
		MainLogic.GetInst().ElectricBoxesFiller.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxTallLeft/ATUTopElectricBoxTallFillLeft"));
		MainLogic.GetInst().ElectricBoxes.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxShortLeft"));
		MainLogic.GetInst().ElectricBoxesFiller.Add(GetTargetPart("ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxShortLeft/ATUTopElectricBoxShortFillLeft"));

		MainLogic.GetInst().ElectricBoxes.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxTallRight"));
		MainLogic.GetInst().ElectricBoxesFiller.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxTallRight/ATUTopElectricBoxTallFillRight"));
		MainLogic.GetInst().ElectricBoxes.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxShortRight"));
		MainLogic.GetInst().ElectricBoxesFiller.Add(GetTargetPart("ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxShortRight/ATUTopElectricBoxShortFillRight"));

		//root objects for left & right parts of the ATU for width scaling with the markers above
		LRFGroups[0] = GetTargetPart("ATUPieces/ATUPartLeft");
		LRFGroups[1] = GetTargetPart("ATUPieces/ATUPartRight");
		LRFGroups[2] = GetTargetPart("ATUPieces/ATUMidFiller");
		//Sets up the updating materials
		Grids[0,0] = GetTargetPart("ATUPieces/ATUPartLeft/LeftGrids/GridPlane10mmLeft");
		Grids[0,1] = GetTargetPart("ATUPieces/ATUPartLeft/LeftGrids/GridPlane50mmLeft");
		Grids[0,2] = GetTargetPart("ATUPieces/ATUPartLeft/LeftGrids/GridPlane100mmLeft");
		Grids[0,3] = GetTargetPart("ATUPieces/ATUPartLeft/LeftGrids/GridPlane200mmLeft");
		Grids[1,0] = GetTargetPart("ATUPieces/ATUPartRight/RightGrids/GridPlane10mmRight");
		Grids[1,1] = GetTargetPart("ATUPieces/ATUPartRight/RightGrids/GridPlane50mmRight");
		Grids[1,2] = GetTargetPart("ATUPieces/ATUPartRight/RightGrids/GridPlane100mmRight");
		Grids[1,3] = GetTargetPart("ATUPieces/ATUPartRight/RightGrids/GridPlane200mmRight");
	}

	//The ATU paths for the models included in initial the application
	private string[] DefaultATUPaths = new string[32]{ 
		"ATUPieces/ATUPartLeft/ATUMainrailLeft",
		"ATUPieces/ATUPartLeft/ATUMainrailClearanceBoxLeft",
		"ATUPieces/ATUPartLeft/ATUMainrailLeft/ATUMainrailFillLeft",
		"ATUPieces/ATUPartRight/ATUMainrailRight",
		"ATUPieces/ATUPartRight/ATUMainrailClearanceBoxRight",
		"ATUPieces/ATUPartRight/ATUMainrailRight/ATUMainrailFillRight",
		"ATUPieces/ATUPartLeft/ATUSiderailLeft",
		"ATUPieces/ATUPartLeft/ATUSiderailLeft/ATUSiderailFillLeft",
		"ATUPieces/ATUPartRight/ATUSiderailRight",
		"ATUPieces/ATUPartRight/ATUSiderailRight/ATUSiderailFillRight",
		"ATUPieces/ATUPartLeft/ATUTopElectricLeft",
		"ATUPieces/ATUPartRight/ATUTopElectricRight",
		"ATUPieces/ATUMidFiller/ATUTopFiller",
		"ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxTallLeft",
		"ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxTallLeft/ATUTopElectricBoxTallFillLeft",
		"ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxShortLeft",
		"ATUPieces/ATUPartLeft/ATUTopElectricLeft/ATUTopElectricBoxShortLeft/ATUTopElectricBoxShortFillLeft",
		"ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxTallRight",
		"ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxTallRight/ATUTopElectricBoxTallFillRight",
		"ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxShortRight",
		"ATUPieces/ATUPartRight/ATUTopElectricRight/ATUTopElectricBoxShortRight/ATUTopElectricBoxShortFillRight",
		"ATUPieces/ATUPartLeft",
		"ATUPieces/ATUPartRight",
		"ATUPieces/ATUMidFiller",
		"ATUPieces/ATUPartLeft/LeftGrids/GridPlane10mmLeft",
		"ATUPieces/ATUPartLeft/LeftGrids/GridPlane50mmLeft",
		"ATUPieces/ATUPartLeft/LeftGrids/GridPlane100mmLeft",
		"ATUPieces/ATUPartLeft/LeftGrids/GridPlane200mmLeft",
		"ATUPieces/ATUPartRight/RightGrids/GridPlane10mmRight",
		"ATUPieces/ATUPartRight/RightGrids/GridPlane50mmRight",
		"ATUPieces/ATUPartRight/RightGrids/GridPlane100mmRight",
		"ATUPieces/ATUPartRight/RightGrids/GridPlane200mmRight"
	};
	//*/
}
