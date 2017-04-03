using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour {


}/*


	//Menu objects
	public GameObject[] menu;
	public GameObject contentButtton;
	public GameObject topPanel;
	public GameObject midPanel;

	//Bools for animation
	public bool openFull = false;
	public bool openSmall = false;

	//Top Level variables
	public TopLevelDriver topDrive;
	private bool topLevelOpen = false;
	private int[] topLevelActive;


	//initilaize 
	void Awake(){

		//Set Top Level Driver
		topDrive = topPanel.GetComponent<TopLevelDriver>();
		contentButtton.SetActive (true);

		//temp
		SetContentButton(new int[]{0,2},true);
	}


	/* ===================================================================================================================================
	 * 
	 * 									Menu Animation updates
	 * 						 			Drives Main Menu Animation state
	 *=================================================================================================================================== */
/*
	//Menu Animation calls
	public void OpenFull(){
		OpenTopLevel ();
		SetMenuAnim ();
	}
	public void OpenSmall(){
		openFull = false;
		openSmall = true;
		SetMenuAnim ();
		CloseTopLevel ();
	}
	public void Close(){
		openFull = openSmall = false;
		SetMenuAnim ();
		CloseTopLevel ();
	}

	//Menu Animimation driver
	private void SetMenuAnim(){
		
		for (int i = 0; i < menu.Length; ++i) {
			Animator ani = menu [i].GetComponent<Animator> ();
			if (ani != null) {
				ani.SetTime (0);
				ani.SetBool ("OpenSmall", openSmall);
				ani.SetBool ("OpenFull", openFull);
			}
		}
	}


	/* ===================================================================================================================================
	 * 
	 * 									Top Level updates
	 * 						 			Drives Top Level Animation
	 *=================================================================================================================================== */
/*
	//Overwrite Content Button
	public void SetContentButton(int[] act, bool showHire){
		topLevelActive = act;
		string s = (showHire) ? "Upgrade / Hire / Command" : "Upgrade / Command";
		contentButtton.transform.GetChild (0).gameObject.GetComponent<Text> ().text = s;
	}

	//Open Top level 
	public void OpenTopLevel(){

		//check if already open
		if (!topLevelOpen) {
			topLevelOpen = openFull = openSmall = true;
			contentButtton.SetActive (false);
			topPanel.SetActive (true);
			topDrive.ActivateButtons (topLevelActive);

		}
	}

	//Close Top Level
	public void CloseTopLevel(){

		//check if already closed
		if (topLevelOpen) {
			topLevelOpen = false;
			topDrive.ResetButtons ();
			contentButtton.SetActive (true);
			topPanel.SetActive (false);
		}
	}
}


*/