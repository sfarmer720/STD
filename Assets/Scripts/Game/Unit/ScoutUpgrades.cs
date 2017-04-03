using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoutUpgrades : Upgrades {

	//Set on Wake
	void Awake(){

		//Set Unit ID
		unitID = new Vector3(0,0,0);

		//Set Icons
		SetTopIcons(false);
		SetMid ();
		SetLow ();
	}


	//MID ICONS
	private void SetMid(){

		//Creation Lists
		List<List<Vector2>> passV = new List<List<Vector2>> ();

		//upgrade
		List<Vector2> v1 = new List<Vector2> ();
		v1.Add (new Vector2 (2, 1));
		v1.Add (new Vector2 (3, 0));

		//hire
		List<Vector2> v2 = new List<Vector2> ();

		//command
		List<Vector2> v3 = new List<Vector2> ();
		v3.Add (new Vector2 (0, 0));
		v3.Add (new Vector2 (1, 0));
		v3.Add (new Vector2 (2, 0));
		v3.Add (new Vector2 (3, 0));
		v3.Add (new Vector2 (4, 0));
		v3.Add (new Vector2 (5, 0));


		//merge lists and create midicons
		passV.Add (v1);
		passV.Add (v2);
		passV.Add (v3);
		SetMidIconLists (passV);
	}


	//LOW ICONS//
	private void SetLow(){

		//Container list
		List<List<Vector2>> container = new List<List<Vector2>>();

		//Class 1
		container.Add(LowQuickList(6));

		//Class 2
		container.Add(LowQuickList(6));


		//Create low icons
		SetLowLists(container);
	}

	public void Test(){
		Debug.Log ("SCOUT UPGRASDE TEST");
	}
}
