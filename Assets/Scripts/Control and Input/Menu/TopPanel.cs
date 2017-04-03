using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TopPanel : MonoBehaviour {

	public Menu menu;
	public Animator menuAnimator;
	public MidPanel mid;
	public GameObject[] topObjects;
	private TopIcon[] tops;
	private string[] topVars = { "Upgrade", "Command", "Hire" };


	//initialize
	void Awake(){

		tops = new TopIcon[topObjects.Length];

		for (int i = 0; i < topObjects.Length; ++i) {
			TopIcon ti = new TopIcon ();
			tops [i] = ti;
			tops [i].icon = i;
			tops [i].panel = topObjects [i];
			tops [i].button = tops [i].panel.transform.Find ("Images").gameObject.GetComponent<Button> ();
			tops [i].image = tops [i].button.transform.Find ("Ring Mask").transform.Find ("Icon").gameObject.GetComponent<Image> ();

			tops [i].panel.SetActive (false);
		}

		this.gameObject.SetActive (false);
	}

	//Close Top Panel
	public void Close(){
		if (mid.gameObject.activeSelf) {
			mid.Close ();
		}
		DeactivateAllIcons ();
		this.gameObject.SetActive (false);
	}

	//Set Selection Animations
	public void Selection(int i, string s){

		//set return listner
		tops[i].button.onClick.RemoveAllListeners();
		tops [i].button.onClick.AddListener (() => {
			Return (i);
		});

		//selection actions
		mid.gameObject.SetActive(true);
		mid.SetMidPos(i);
		mid.SetMidIcons (menu.unitInfo.midIcons [i]);
		menuAnimator.SetTrigger (s);

	}
	public void Return(int i){

		//clear top icon actions and add return action
		if (tops [i].panel.activeSelf) {
			tops [i].button.onClick.RemoveAllListeners ();
			tops [i].button.onClick.AddListener (() => {

			});


			//Close Mid
			mid.Close();

			//reset Top Icons
			SetTopIcons(menu.unitInfo.topIcons);
			if(menu.unitInfo.topIcons.Length > 2){
				menu.ResetToTop();
			}else{
				menuAnimator.SetTrigger ("Return");
			}

		}
	}


	//Set Top Panel
	public void SetTopIcons(TopIcon[] tis){
		//Deactivate all icons
		DeactivateAllIcons();

		//activate panel
		this.gameObject.SetActive(true);

		for (int i = 0; i < tis.Length; ++i) {

			ActivateIcon (tis [i].icon, tis [i]);
	
		}
	}

	//Activate Top Icons
	private void ActivateIcon(int i, TopIcon ti){
		
		tops [i].panel.SetActive (true);
		tops [i].image.sprite = ti.sprite;
		tops [i].button.onClick.RemoveAllListeners ();

		for (int x = 0; x < ti.actions.Count; ++x) {
			tops [i].button.onClick.AddListener (ti.actions [x]);
		}

		tops [i].button.onClick.AddListener (() => {
			Selection (i, topVars [ti.icon]);
		});
	}

	//Deactivate Top Icon
	public void DeactivateAllIcons(){
		for (int i = 0; i < tops.Length; ++i) {
			DeactivateIcon (i);
		}
	}
	private void DeactivateIcon(int i){
		tops [i].panel.SetActive (false);
	}

}


public class TopIcon{
	public GameObject panel;
	public Button button;
	public Image image;
	public Sprite sprite;
	public int icon;
	public List<UnityAction> actions = new List<UnityAction> ();
}
