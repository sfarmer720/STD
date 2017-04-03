using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour {


	//external Classes
	private Unit unit;

	//Selection variables
	public bool isSelected = false;

	// Use this for initialization
	public void InitSelection(Unit u) {

		//set external classes
		unit = u;

	}


	/* ===================================================================================================================================
	 * 
	 * 									Active Selection
	 * 			 					Functions for ACtive Selection
	 *=================================================================================================================================== */

	//Primary Selection function
	public void Selection(int clickType){

		// check if one click, double click, or right click
		if (clickType == 0) {

			//toggle selection on/off
			SelectionToggle ();

		} else if (clickType == 1) {

		}
	}

	//Toggle Active Selection
	private void SelectionToggle(){

		//check if currently selected
		if (isSelected) {

			//already selected, unselect and reset
			//ChangeMaterial(norMats);
			isSelected = false;

		} else {

			//turn on selection
			//ChangeMaterial(outMats);
			isSelected = true;
		}
	}


	/* ===================================================================================================================================
	* 
	* 									Passive Selection
	* 			 					Functions for Passive Selection
	*=================================================================================================================================== */

	//Passive Selection
	public void Selection(Transform t, Vector3 v){

		//TODO: functions for enemy, player unit, and movement

		//Move unit to selected object
		unit.UnitToTile(t, v);

	}
		




	/* ===================================================================================================================================
	* 
	* 									Necessary Functions
	* 			 					   Functions for Selection
	*=================================================================================================================================== */


	//Set Layers of unit
	public void SetLayers(int layer){

		//get all children
		Transform[] children;
		children = this.gameObject.GetComponentsInChildren<Transform> ();

		//cycle all children, and set layers
		foreach (Transform child in children) {

			if (child.gameObject.layer != 28 && child.gameObject.layer != 13) {
				child.gameObject.layer = layer;
			}
		}

		//set main layer
		this.gameObject.layer = layer;
	}

}
