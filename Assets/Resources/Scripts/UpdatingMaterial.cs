using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatingMaterial : MonoBehaviour {

	private MainLogic ml;
	private Renderer rend;

	// Use this for initialization
	//The script adds itself to a list in the MainLogic which is used to deliver the the new color to all ATU model parts
	void Start () {
		ml = FindObjectOfType<MainLogic> ();
		ml.updmats.Add (this);
		Debug.Log (ml.updmats.Count);
		rend = GetComponentInParent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//used from MainLogic to assign the new color to all ATU model renderers
	public void UpdateMaterialColor(Color col){
		Debug.Log ("mat updated");
		rend.material.color = col;
	}
}
