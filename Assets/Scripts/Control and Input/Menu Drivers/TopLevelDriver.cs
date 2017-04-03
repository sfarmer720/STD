using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TopLevelDriver : MonoBehaviour {


}/*

	//Animation Struct
	public struct TopLevelButtons {
		public bool isActive;
		public bool isSelected;
		public GameObject panel;
		public GameObject ring;
		public Animator panelAni;
		public Animator buttonAni;
		public Animator ringAni;
		public Button button;
		public Image icon;
	};

	//Top Level buttons
	public GameObject menuObject;
	public GameObject[] objectsToDrive;
	private MenuControl menuCon;
	private TopLevelButtons[] buttons = new TopLevelButtons[3];

	//MidLevel Variables
	public GameObject midPanel;

	//animation variables
	private string[] aniVar = {"IsActive", "IsSelected"};

	//initialize
	void Awake(){

		//initialize menu control link
		menuCon = menuObject.GetComponent<MenuControl>();


		//initialize buttons
		for (int i = 0; i < buttons.Length; ++i) {

			//initialize structure Booleans
			buttons [i].isActive = false;
			buttons [i].isSelected = false;

			//initialize structure Game objects
			buttons [i].panel = objectsToDrive [i];
			buttons [i].button = buttons [i].panel.transform.Find ("Images").gameObject.GetComponent<Button>();
			buttons [i].ring = buttons [i].button.transform.Find ("Ring").gameObject;
			buttons [i].icon = buttons [i].button.transform.Find ("Ring Mask").transform.Find ("Icon").gameObject.GetComponent<Image> ();

			//initialize animators
			buttons[i].panelAni = buttons[i].panel.GetComponent<Animator>();
			buttons[i].buttonAni = buttons[i].button.GetComponent<Animator>();
			buttons[i].ringAni = buttons[i].ring.GetComponent<Animator>();


			//Set Buttons to inactive
			buttons[i].button.onClick = new Button.ButtonClickedEvent();
			buttons[i].panel.SetActive(false);
		}

		this.gameObject.SetActive (false);
	}

	//Reset Buttons
	public void ResetButtons(){
		for (int i = 0; i < buttons.Length; ++i) {
			buttons [i].isActive = false;
			buttons [i].isSelected = false;
			buttons [i].buttonAni.SetBool (aniVar [0], false);
			buttons [i].buttonAni.SetBool (aniVar [1], false);
			buttons [i].panelAni.SetBool (aniVar [0], false);
			buttons [i].panelAni.SetBool (aniVar [1], false);
			buttons [i].panel.SetActive (false);
		}

		//deactivate midpanel
		midPanel.SetActive (false);
	}

	//Activate Buttons
	public void ActivateButtons(){
		ActivateButtons (new int[]{ 0, 1, 2 });
	}
	public void ActivateButtons(int[] act){

		for (int i = 0; i < act.Length; ++i) {
			buttons[act[i]].panel.SetActive(true);
			buttons [act [i]].isActive = true;
			buttons [act [i]].buttonAni.SetBool (aniVar [0], true);

		}
	}
	public void DeactivateButtons(){
		DeactivateButtons (new int[]{ 0, 1, 2 });
	}
	public void DeactivateButtons(int[] dac){

		for (int i = 0; i < dac.Length; ++i) {
			buttons [dac [i]].isActive = false;
			buttons [dac [i]].buttonAni.SetBool (aniVar [0], false);
		}
	}


	//Select Buttons
	public void SetSelectActions(UnityAction upgrade, UnityAction command, UnityAction hire = null){

		buttons [0].button.onClick.RemoveAllListeners ();
		buttons [0].button.onClick.AddListener (upgrade);
		buttons [0].button.onClick.AddListener (() => {
			SelectButton (0);
		});

		buttons [2].button.onClick.RemoveAllListeners ();
		buttons [2].button.onClick.AddListener (command);
		buttons [2].button.onClick.AddListener (() => {
			SelectButton (2);
		});

		if (hire != null) {
			buttons [1].button.onClick.RemoveAllListeners ();
			buttons [1].button.onClick.AddListener (hire);
			buttons [1].button.onClick.AddListener (() => {
				SelectButton (1);
			});
		}
	}
	public void SelectButton(int i){

		//set button as selected
		buttons [i].isSelected = true;
		SetAll (i, 1, true);

		//unselect the rest
		int[] btn = AllButOne(i);
		for (int x = 0; x < btn.Length; ++x) {
			buttons [btn [x]].isSelected = false;
			buttons [btn [x]].isActive = false;
			SetAll (btn [x], 0, false);
			SetAll (btn [x], 1, false);
		}

		//Acrivates mid panel and associated buttons
		midPanel.SetActive (true);
		midPanel.GetComponent<MidLevelDriver> ().ActivateMidIcons ();
	}


	//Set boolean 
	private void SetAll(int b, int i, bool setTo){
		buttons [b].panelAni.SetBool (aniVar [i], setTo);
		buttons [b].buttonAni.SetBool (aniVar [i], setTo);
		buttons [b].ringAni.SetBool (aniVar [i], setTo);
	}

	//return all but one top level butttons
	private int[] AllButOne(int i){
		int[] init = { 0, 1, 2 };
		int[] ret = new int[2];
		for (int x = 0, y = 0; x < 3; ++x) {
			if (init [x] != i) {
				ret [y] = init [x];
				++y;
			}
		}

		return ret;
	}

	public void pressed(){
		Debug.Log ("Button was pressed");
	}

}

*/