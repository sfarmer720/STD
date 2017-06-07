using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickKeys : MonoBehaviour {

	public SelectionScript selection;
	public Overlord overlord;

	void FixedUpdate(){

		//TEMP FUNCTIONS
		//TODO: Set These in a Keyboard/Controller class
		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)){

			int incDir = (Input.GetKey (KeyCode.LeftArrow)) ? -1 : 1;
			CycleUnits (incDir);
			Debug.Log ("Switching Units");
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			ClearSelection ();
		}
	}

	//Clear Current Selection
	public void ClearSelection(){
		selection.SelectionAction (selection.current);
		selection.current = null;
	}

	//Set Selection
	public void SetSelection(GameObject go){
		ClearSelection ();
		selection.current = go.transform;
		selection.SelectionAction (selection.current);
	}


	//Cycle Units
	private void CycleUnits(int i){

		//check if any previous selection
		if (selection.current != null) {
			
			//check if current selection is in playuer's unit list
			if (overlord.units.Contains (selection.current.gameObject)) {

				//is in players unit list, cycle list to next unit
				int next = overlord.units.IndexOf(selection.current.gameObject);
				int length = overlord.units.Count;

				//check if adding inc/dec will overflow
				if (next + i >= length || next + i < 0) {

					//cycle to begining or end
					next = (next + i < 0) ? length - 1 : 0;
					SetSelection (overlord.units [next]);

				} else {

					//won't break stack, inc/dec to next unit
					SetSelection (overlord.units [next + i]);
				}

			} else {

				//not in players unit list, clear selection, select first 
				if (overlord.units.Count > 0) {
					SetSelection(overlord.units[0]);
				}
			}
		} else {

			//no current selection, get first unit
			if (overlord.units.Count > 0) {
				SetSelection(overlord.units[0]);
			}
		}
	}

}
