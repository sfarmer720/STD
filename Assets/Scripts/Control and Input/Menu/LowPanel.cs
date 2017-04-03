using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LowPanel : MonoBehaviour {

	public Menu menu;
	public Animator menuAnimator;
	public RectTransform rect;
	public GameObject[] lowObjects;
	private LowIcon[] lows;


	private string[] lowVars = { "LowDefense", "LowUnitID", "LowUnitState", "LowReturn" };


	void Awake(){

		//create low icon list
		lows = new LowIcon[lowObjects.Length];
		for (int i = 0; i < lowObjects.Length; ++i) {
			LowIcon li = new LowIcon ();
			li.icon = i;
			li.panel = lowObjects [i];
			li.button = li.panel.transform.Find ("Images").gameObject.GetComponent<Button> ();
			li.image = li.button.transform.Find ("Ring Mask").transform.Find ("Icon").gameObject.GetComponent<Image> ();
			li.panel.SetActive (false);
			lows [i] = li;
		}

		this.gameObject.SetActive (false);
	}

	//Clsoe Low Panel
	public void Close(){
		DeactivateAllIcons ();
		this.gameObject.SetActive (false);
	}

	//reset animation
	public void ResetLowIconAnimation(){
		menuAnimator.SetTrigger (lowVars [3]);
	}

	//Set low icons animation info
	public void SetLowIconAnimation(Vector3 ID){
		menuAnimator.SetBool (lowVars [0], (ID.x > 0));
		menuAnimator.SetInteger (lowVars [1], (int)ID.y);
		menuAnimator.SetInteger (lowVars [2], (int)ID.z);
	}
		


	//Set Low Icons
	public void SetLowIcons(List<LowIcon> lis){
		this.gameObject.SetActive (true);
		DeactivateAllIcons ();
		for (int i = 0; i < lis.Count; ++i) {
			ActivateIcons (lis [i].icon, lis [i]);
		}
	}

	//Activate Icons
	private void ActivateIcons(int i, LowIcon li){

		//set standard icon information
		lows [i].panel.SetActive (true);
		lows [i].isActive = li.isActive;
		lows[i].sprite = li.sprite;
		lows [i].selectionSprite = li.selectionSprite;
		lows [i].image.sprite = (!li.isActive) ? li.sprite : li.selectionSprite;
		lows [i].button.onClick.RemoveAllListeners ();

		//Add variable icon actions
		for (int x = 0; x < li.actions.Count; ++x) {
			lows [i].button.onClick.AddListener (li.actions [x]);
		}

		//Add Standard actions (most actitons should be variable at this level)
		lows[i].button.onClick.AddListener (() => {

			//TODO: TEMP TEST FUNCTIONS
			Debug.Log(lows[i].panel + " is selected");
			lows [i].image.sprite = li.selectionSprite;
		});

	}


	//Deactivate Icons
	public void DeactivateAllIcons(){
		for (int i = 0; i < lows.Length; ++i) {
			DeactivateIcon (i);
		}
	}
	private void DeactivateIcon(int i){
		lows [i].panel.SetActive (false);
	}
}


//Low Icon Class
public class LowIcon{
	public GameObject panel;
	public Button button;
	public Image image;
	public Vector3 unitID = new Vector3 (-1, -1, -1);

	public Sprite sprite;
	public Sprite selectionSprite;
	public int icon;
	public bool isActive;
	public List<UnityAction> actions = new List<UnityAction> ();
}
