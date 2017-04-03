using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class MidLevelDriver : MonoBehaviour {

	//Mid Objects
	public GameObject[] midObjects;
	private MidIcon[] mids;
	private string[] aniVars = { "Location", "Selection" };

	//low reference
	public LowLevelDriver lowPanel;

	//show selection
	public bool selection = false;


}/*


	//initialize
	void Awake(){

		//create mids
		mids = new MidIcon[midObjects.Length];
		for (int i = 0; i < midObjects.Length; ++i) {
			MidIcon m = new MidIcon ();
			m.panel = midObjects [i].gameObject;
			Debug.Log (m.panel);
			m.panelAni = m.panel.GetComponent<Animator> ();
			m.track = m.panel.transform.Find ("Track").gameObject.GetComponent<Slider> ();
			m.trackAni = m.track.gameObject.GetComponent<Animator> ();
			m.iconPanel = m.track.transform.Find ("Icon Slide Area").transform.Find ("Full Icon").gameObject;
			m.iconAni = m.iconPanel.GetComponent<Animator> ();
			m.iconButton = m.iconPanel.GetComponent<Button> ();
			m.icon = m.iconPanel.transform.Find ("Ring Mask").transform.Find ("Icon").gameObject.GetComponent<Image> ();
			m.panel.SetActive (false);
			mids [i] = m;
		}

		//set inactive after setup
		this.gameObject.SetActive(false);
	}


	//set mid icons
	public void SetMidIcons(MidIcon[] mi){

		Debug.Log (mids.Length);
		Debug.Log (mids [0].panel);

		//deactivate all mid icons
		ResetAllMid();
		DeactivateAllMid();

		Debug.Log ("Someho9w I got here");

		//Set new mids
		for (int i = 0; i < mi.Length; ++i) {
			SetMid (i, mi [i]);
		}

	}

	//deactivate mid icon
	public void DeactivateMid(int i){
		mids [i].panel.SetActive (false);
	}
	public void DeactivateAllMid(){
		for (int i = 0; i < mids.Length; ++i) {
			if (mids [i].panel.activeSelf) {
				DeactivateMid (i);
			}
		}
		this.gameObject.SetActive (false);
	}

	//reset mid icon
	public void ResetMid(int i){
		mids [i].location = -1;
		mids [i].selection = false;
	}
	public void ResetAllMid(){
		for (int i = 0; i < mids.Length; ++i) {

			if (mids [i].panel.activeSelf) {
				ResetMid (i);
			}
		} 
	}

	//set mis icon
	private void SetMid(int i, MidIcon m){
		mids [i].icon.sprite = m.iconImage;
		mids [i].location = m.location;
		mids [i].selection = selection;
		mids [i].iconButton.onClick.RemoveAllListeners ();

		//TODO: Add always listners to button
		for (int x = 0; x < m.events.Length; ++x) {
			mids [i].iconButton.onClick.AddListener (m.events [x]);
		}

		//open lower panel listener (Call only after callign unique listners)
		mids[i].iconButton.onClick.AddListener(OpenLow);
	}

	//activate mid
	private void ActivateMid(int i){
		mids [i].panel.SetActive (true);
		UpdateAnimations (i);
	}
	public void ActivateMidIcons(){
		this.gameObject.SetActive (true);
		for (int i = 0; i < mids.Length; ++i) {
			if (mids [i].location >= 0) {
				ActivateMid (i);
			}
		}
	}


	//update Animations
	private void UpdateAnimations(int i){

		Debug.Log ("Mid " + i + " is being set to location " + mids [i].location);
		mids [i].panelAni.SetInteger (aniVars [0], mids [i].location);
		mids [i].panelAni.SetBool (aniVars [0], selection);

		mids [i].trackAni.SetInteger (aniVars [0], mids [i].location);
		mids [i].trackAni.SetBool (aniVars [0], selection);

	}


	public void OpenLow(){
		lowPanel.gameObject.SetActive (true);
	}


	public void ChangeSelection(){
		selection = !selection;
	}

}


//Mid Level track and Icon Control class
public class MidIcon{
	public GameObject panel;
	public GameObject iconPanel;
	public Animator panelAni;
	public Animator trackAni;
	public Animator iconAni;
	public Button iconButton;
	public Slider track;
	public Image icon;
	public Sprite iconImage;
	public int location = -1;
	public bool selection = false;
	public UnityAction[] events = new UnityAction[0];
}
*/