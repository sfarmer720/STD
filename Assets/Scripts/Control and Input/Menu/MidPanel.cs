using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MidPanel : MonoBehaviour {

	public Menu menu;
	public Animator menuAnimator;
	public LowPanel lowPanel;
	public RectTransform rect;
	public GameObject[] midObjects;

	private MidIcon[] mids;
	private string[] midVars = { "MidRotation", "MidSelect", "MidReturn" };


	//Initialize
	void Awake(){

		//create array containing all mid Icons
		mids = new MidIcon[midObjects.Length];
		for (int i = 0; i < midObjects.Length; ++i) {
			MidIcon mi = new MidIcon ();
			mi.icon = i;
			mi.panel = midObjects [i];
			mi.button = mi.panel.transform.Find ("Images").gameObject.GetComponent<Button> ();
			mi.image = mi.button.transform.Find ("Ring Mask").transform.Find ("Icon").gameObject.GetComponent<Image> ();
			mi.panel.SetActive (false);
			mids [i] = mi;
		}

		//hide mid ppanel at start
		this.gameObject.SetActive(false);

	}

	//Close Mid Panel
	public void Close(){

		if (lowPanel.gameObject.activeSelf) {
			lowPanel.Close ();
		}
		DeactivateAllIcons ();
		this.gameObject.SetActive (false);
	}

	//Set selecited icon
	private void SetSelected(){
		for (int i = 0; i < mids.Length; ++i) {
			if (mids [i].isActive) {
				SetSelected (i);
				break;
			}
		}
	}
	public void SetSelected(int i){
		
		lowPanel.ResetLowIconAnimation();

		//deselect all icons
		for (int x = 0; x < mids.Length; ++x) {

			if (x != i) {

				Debug.Log(mids[x].panel + " now is inactive | " + mids [x].sprite);
				mids [x].isActive = false;
				mids [x].image.sprite = mids [x].sprite;
				mids [x].button.onClick.RemoveAllListeners ();
				CustomActions (mids [x]);
				StandardActions (mids [x]);

			} else {

				//set active
				Debug.Log(mids[i].panel + " now is selected | "+mids [x].selectionSprite);
				mids[i].isActive = true;
				mids [i].image.sprite = mids [i].selectionSprite;
				mids [i].button.onClick.RemoveAllListeners ();
				CustomActions (mids [i]);
				lowPanel.SetLowIcons(menu.unitInfo.lowIcons[mids[i].listNum]);
				lowPanel.SetLowIconAnimation (mids [i].unitID);
			}
		}
	}


	//Set Mid Position
	public void SetMidPos(int i){

		//set container starting position
		if (i == 0) {
			rect.localRotation = Quaternion.Euler (Vector3.zero);
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = new Vector2 (0.5f, 1f);
		} else if (i == 1) {
			rect.localRotation = Quaternion.Euler(new Vector3(0,0,90));
			rect.anchorMin = new Vector2 (0.125f, 1f);
			rect.anchorMax = new Vector2 (0.875f, 1f);
		} else if (i == 2) {
					rect.localRotation = Quaternion.Euler (Vector3.zero);
			rect.anchorMin = new Vector2 (0.5f, 1f);
			rect.anchorMax = Vector2.one;
		} else {
			rect.localRotation = Quaternion.Euler (Vector3.zero);
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector3.zero;
		}

		//set mid rotation
		menuAnimator.SetInteger (midVars [0], i);
	}

	//Set Mid Icons
	public void SetMidIcons(List<MidIcon> mis){

		//deactivate all previous
		DeactivateAllIcons();

		//activate only active icons
		for (int i = 0; i < mis.Count; ++i) {
			ActivateIcons (mis [i].icon, mis [i], i);
		}

		SetSelected ();
	}

	//Activate Icons
	private void ActivateIcons(int i, MidIcon mi, int arrayPos){
		
		mids [i].panel.SetActive (true);
		mids [i].isActive = mi.isActive;
		mids [i].actions = mi.actions;
		mids [i].listNum = arrayPos;
		mids [i].unitID = mi.unitID;

		mids[i].sprite = mi.sprite;
		mids [i].selectionSprite = mi.selectionSprite;
		mids [i].button.onClick.RemoveAllListeners ();


		CustomActions (mids [i]);
		StandardActions (mids [i]);
	}

	private void StandardActions(MidIcon mi){
		mi.button.onClick.AddListener (() => {

			//add actions only to inactive
			if (!mi.isActive) {
				Debug.Log (mi.panel + " can be selected");
				SetSelected (mi.icon);
			}
		});
	}

	private void CustomActions(MidIcon mi){
		for (int x = 0; x < mi.actions.Count; ++x) {
			mi.button.onClick.AddListener (mi.actions [x]);
		}
	}
	


	//Deactivate Icons
	public void DeactivateAllIcons(){
		for (int i = 0; i < mids.Length; ++i) {
			DeactivateIcon (i);
		}
	}
	private void DeactivateIcon(int i){
		mids [i].panel.SetActive (false);
	}


}

//Mid Icon Class
public class MidIcon{
	public GameObject panel;
	public Button button;
	public Image image;
	public int listNum;
	public Vector3 unitID = new Vector3 (-1, -1, -1);

	public Sprite sprite;
	public Sprite selectionSprite;
	public int icon;
	public bool isActive = false;
	public List<UnityAction> actions = new List<UnityAction> ();

}
