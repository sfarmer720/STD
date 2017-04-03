using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIDriver : MonoBehaviour {

	//GUI Elements
	public GameObject cursor;
	public GameObject messageText;
	public Menu menu;

	//public MenuControl menu;



	/* ===================================================================================================================================
	 * 
	 * 										Menu Functions
	 * 			 					Functions used to control the menu
	 *=================================================================================================================================== */



/*	public void MenuControl(bool open){
		if (open && !menu.openSmall) {
			menu.OpenSmall ();
		} else  if(!open){
			menu.Close ();
		}
	}
*/

	/* ===================================================================================================================================
	 * 
	 * 										Message Box Functions
	 * 			 					Functions used to create screen overlay messages
	 *=================================================================================================================================== */

	//Set custom Message Text
	public void SetMessageText(string s){
		Debug.Log ("SettingMessage");
		messageText.SetActive (true);
		messageText.GetComponent<Text> ().text = s;
	}
	public void SetTimerMessage(string s, float maxTime, float currentTime){

		string str = s + ": " + TimeToString( TimeLeft (maxTime, currentTime));
		SetMessageText (str);
	}

	//Clear Message
	public void ClearMessageText(){
		messageText.GetComponent<Text> ().text ="";
		messageText.SetActive (false);
	}

	/* ===================================================================================================================================
	 * 
	 * 										Pop up Functions
	 * 			 					Functions used to create popup windows
	 *=================================================================================================================================== */



	/* ===================================================================================================================================
	* 
	* 										Various Functions
	* 			 					Functions used in various methods
	*=================================================================================================================================== */

	//TIME LEFT FUNCTIONS
	public float TimeLeft(float maxTime, float currentTime){
		return maxTime - (Time.time - currentTime);
	}
	public string TimeToString(float t){
		string min = Mathf.Floor (t / 60).ToString ("00");
		string sec = Mathf.Floor (t % 60).ToString ("00");
		string tim = min + ":" + sec;
		return tim;
	}
}
