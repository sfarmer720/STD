using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InfoPanel : MonoBehaviour {

	//Info Panel Variables
	public Slider HP;
	public Slider offense;
	public Slider defense;
	public Slider speed;

	//Unit Information
	private UnitInfo info;

	//Set info - shoudl be called every frame to keep current
	public void SetInfo(UnitInfo ui){
		info = ui;
		SetInfoPanel ();
	}

	//TODO: Set Max values to compare info against. 
	//TODO: Add more sliders to show full information in full open mode
	//Set Sliders
	private void SetInfoPanel(){

		//set HP
		HP.wholeNumbers = true;
		HP.maxValue = info.maxHP;
		HP.value = info.HP;

		//Set Offense
		offense.maxValue = 50;
		offense.value = info.attack;

		//set defense
		defense.maxValue = 50;
		defense.value = info.defense;

		//set speed
		speed.maxValue = 10;
		speed.value = info.speed;
	}

}
