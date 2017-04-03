using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Menu : MonoBehaviour {

	//Menu Control Variables
	public GameObject menuPanel;
	public Animator menuAnimator;
	public bool isOpen = false;
	public bool isFull = false;
	public bool reset = false;
	public UnitInfo unitInfo;
	private string[] menuVars = { "isOpen", "isFull", "Reset" };

	//Shield variabels
	public Button[] shieldSides;

	//Info Panel Variables
	public InfoPanel infoPanel;

	//Content Button
	public Button contentButton;
	public Text contentButtonText;

	//Top Level Variables
	public TopPanel topPanel;

	//Mid Level Variables
	public MidPanel midPanel;

	//Low Level Variables
	public LowPanel lowPanel;


	//update
	void LateUpdate(){

		//check if unit info still valid
		if (unitInfo == null) {
			Close ();
		} else {

			//Clear Reset
			if (reset) {
				reset = false;
				OpenFull ();
			}

		}
	}


	/* ===================================================================================================================================
	 * 
	 * 									Menu Functions
	 * 			 			Function to control Menu as a whole
	 *=================================================================================================================================== */

	//Open Full Menu
	public void OpenFull(){
		Debug.Log ("MENU NOW FULLY OPEN");

		isOpen = isFull = true;
		ShieldFull ();
		SetContentButton ();
		SetMenuAnimation ();
	}

	//Open Menu
	public void Open(){

		//only open if valid unit info
		if (unitInfo != null) {
			isOpen = true;
			isFull = false;
			ShieldOpen ();
			SetInfoPanel ();
			SetContentButton ();
			SetMenuAnimation ();
		}
	}

	//Close entire panel
	public void Close(){

		//nullify unit info
		unitInfo = null;

		//TODO: Add close function for ewnetire menu
		ShieldClose();
		SetContentButton();
		topPanel.Close();


		//Close Main menu compopnents
		isOpen = isFull = false;
		SetMenuAnimation ();
	}

	//reset To top menu
	public void ResetToTop(){
		reset = true;
		SetMenuAnimation ();
	}

	//Set menu animation
	private void SetMenuAnimation(){
		menuAnimator.SetBool (menuVars [1], isFull);
		menuAnimator.SetBool (menuVars [0], isOpen);
		menuAnimator.SetBool (menuVars [2], reset);
	}

	//Set content Button
	private void SetContentButton(){
		
		//check if Unit information is null
		if(unitInfo != null){
			if (isOpen && !isFull) {

				//if open, but not full, set content button to active and set actions
				contentButton.gameObject.SetActive (true);
				contentButtonText.text = unitInfo.contentButtonText;
				contentButton.onClick.RemoveAllListeners ();
				contentButton.onClick.AddListener (() => {
					topPanel.SetTopIcons (unitInfo.topIcons);
					OpenFull();
				});

			} else {

				//Menu is full, set content button to hidden
				contentButton.gameObject.SetActive (false);
			}
		} else {

			//Menu is closed/closing reset button
			contentButtonText.text = "An Error has Occured";
			contentButton.onClick.RemoveAllListeners ();
			contentButton.gameObject.SetActive (false);
		}
	}

	/* ===================================================================================================================================
	* 
	* 									Shield Panel 
	* 			 			Functions to control and set Shield panels
	*=================================================================================================================================== */

	private void ShieldClose(){
		for (int i = 0; i < shieldSides.Length; ++i) {
			shieldSides[i].onClick.RemoveAllListeners ();
		}
	}

	private void ShieldOpen(){
		for (int i = 0; i < shieldSides.Length; ++i) {
			shieldSides[i].onClick.RemoveAllListeners ();
            shieldSides[i].onClick.AddListener (Close);
		}
	}

	private void ShieldFull(){
		for (int i = 0; i < shieldSides.Length; ++i) {
			shieldSides[i].onClick.RemoveAllListeners ();
            shieldSides[i].onClick.AddListener (Open);
		}
	}

	/* ===================================================================================================================================
	* 
	* 									Info Panel 
	* 			 			Functions to control and set info panel
	*=================================================================================================================================== */

	//Set Info Panel
	public void SetInfoPanel(){
		infoPanel.SetInfo (unitInfo);
	}


	/* ===================================================================================================================================
	* 
	* 									Top Panel 
	* 			 			Functions to control and set Top panel
	*=================================================================================================================================== */

	//Set Top panel Return
	public void ReturnTop(int i){
		topPanel.Return (i);
	}

}
