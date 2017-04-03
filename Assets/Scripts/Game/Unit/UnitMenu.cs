using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UnitMenu : MonoBehaviour {

	public GUIDriver gui;
	public Menu menu;
	public UnitInfo unitInfo;

	//initialize
	public void Init(GUIDriver ui){
		gui = ui;
		menu = gui.menu;
	}

	//Update
	public void UpdateUnitMenu(UnitInfo ui){
		unitInfo = ui;
		menu.unitInfo = ui;
	}

	/* ===================================================================================================================================
	 * 
	 * 										Main Menu Function
	 * 			 					Functions used to drive Main menu
	 *=================================================================================================================================== */

	//Attach unit info to menu
	public void AttachToMenu(UnitInfo ui){

		//update Unit information
		unitInfo = menu.unitInfo = ui;

		//check if menu is already open
		if (!menu.isOpen) {
			menu.Open ();
		}
	}

	//Remove Unit from menu
	public void RemoveFromMenu(UnitInfo ui){
		if (menu.unitInfo == ui) {
			menu.unitInfo = null;
		}
	}

	private void SetContentButton (UnitInfo ui){

	}

}
