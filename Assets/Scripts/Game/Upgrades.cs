using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Upgrades : MonoBehaviour {

	//Upgrade object reference
	public GameObject updateRef;
	public Vector3 unitID = new Vector3 (-1, -1, -1);

	//Default images
	public Sprite defaultImage1;
	public Sprite defaultImage2;

	//Top Icon Variables
	public TopIcon[] topIcons;
	public Sprite upgradeIcon;
	public Sprite commandIcon;
	public Sprite hireIcon;

	//Mid Icon Variables
	public List<List<MidIcon>> midIcons = new List<List<MidIcon>>();
	public List<Sprite> midUpgradeImages = new List<Sprite> ();
	public List<Sprite> midUpgradeSelected = new List<Sprite> ();
	public List<Sprite> midCommandImages = new List<Sprite> ();
	public List<Sprite> midCommandSelected = new List<Sprite> ();
	public List<Sprite> midHireImages = new List<Sprite> ();
	public List<Sprite> midHireSelected = new List<Sprite> ();

	//Low Icon Variables
	public List<List<LowIcon>> lowIcons = new List<List<LowIcon>>();
	public List<Sprite> low1Images = new List<Sprite> ();
	public List<Sprite> low1SelectionImages = new List<Sprite> ();
	public List<Sprite> low2Images = new List<Sprite> ();
	public List<Sprite> low2SelectionImages = new List<Sprite> ();
	public List<Sprite> low3Images = new List<Sprite> ();
	public List<Sprite> low3SelectionImages = new List<Sprite> ();
	public List<Sprite> low4Images = new List<Sprite> ();
	public List<Sprite> low4SelectionImages = new List<Sprite> ();
	public List<Sprite> low5Images = new List<Sprite> ();
	public List<Sprite> low5SelectionImages = new List<Sprite> ();
	public List<Sprite> low6Images = new List<Sprite> ();
	public List<Sprite> low6SelectionImages = new List<Sprite> ();






	/* ===================================================================================================================================
	 * 
	 * 									Top Icon Function
	 * 			 			Functions used to Set Top Icons in all upgrade classes
	 *=================================================================================================================================== */

	//CREATE TOP ICON ARRAY//
	public void SetTopIcons( bool showHire){

		//initialize
		topIcons = (showHire) ? new TopIcon[3] : new TopIcon[2];

		//Create Upgrade Icon
		topIcons [0] = SetTopIcon (0, SetImage(0,0,0,false));

		//Create Command Icon
		topIcons [1] = SetTopIcon (1, SetImage(0,2,0,true));

		//Create hire icon
		if (showHire) {
			topIcons [2] = SetTopIcon (2, SetImage(0,1,0,false));
		}
	}

	//SET TOP ICON//
	public TopIcon SetTopIcon(int iconNum, Sprite img){

		//Create and return new top icon
		TopIcon ti = new TopIcon();
		ti.icon = iconNum;
		ti.sprite = img;
		return ti;
	}

	//SET TOP ICON ACTIONS//
	public void SetTopIconAction(int i, List<UnityAction> acts){
		topIcons [i].actions = acts;
	}


	/* ===================================================================================================================================
	 * 
	 * 									Mid Icon Function
	 * 			 			Functions used to Set Mid Icons in all upgrade classes
	 *=================================================================================================================================== */

	//Set Mid Lists
	//List need a cooresponding int list, even if not used. 
	//Given in vect2 format Vect2(iconNumber, isActive[1true, 0false]
	public void SetMidIconLists(List<List<Vector2>> IconInfo){

		//Cycle number of icon lists to create
		for (int i = 0; i < IconInfo.Count; ++i) {

			List<MidIcon> mia = new List<MidIcon> ();

			//cycle number of icons to set in list
			for (int j = 0; j < IconInfo [i].Count; ++j) {
				
				//create new icon and add to list
				MidIcon m = SetMidIcon (
					            (int)IconInfo [i] [j].x,
					            (IconInfo [i] [j].y > 0),
					            SetImage (1, i, j, false),
					            SetImage (1, i, j, true)
				            );
				mia.Add (m);
			}

			//add lsits
			midIcons.Add (mia);
		}
	}

	//SET MID ICON//
	public MidIcon SetMidIcon(int iconNum, bool active, Sprite img1, Sprite img2){
		MidIcon mi = new MidIcon ();
		mi.icon = iconNum;
		mi.isActive = active;
		mi.sprite = img1;
		mi.selectionSprite = img2;
		mi.unitID = unitID;
		return mi;
	}

	//SET MID ACTIONS//
	public void SetMidIconAction(int listNum, int iconNum, List<UnityAction> acts){
		midIcons [listNum] [iconNum].actions = acts;
	}


	/* ===================================================================================================================================
	 * 
	 * 									Low Icon Function
	 * 			 			Functions used to Set Mid Icons in all upgrade classes
	 *=================================================================================================================================== */

	//Creat Low Lists
	/*Given in vect2 format Vect2(iconNumber, isActive[1true, 0false]
	 * List recommendations:
	 * 0-5. Upgrades
	 * 6-? Commands
	 * ?-? Hire
	 */
	public void SetLowLists(List<List<Vector2>> iconInfo){

		//Cycle number of icon lists to create
		for (int i = 0; i < iconInfo.Count; ++i) {

			List<LowIcon> lia = new List<LowIcon> ();

			//cycle number of icons to set in list
			for (int j = 0; j < iconInfo [i].Count; ++j) {

				LowIcon l = SetLowIcon (
					            (int)iconInfo [i] [j].x,
					            (iconInfo [i] [j].y > 0),
					            SetImage (2, i, j, false),
					            SetImage (2, i, j, true)
				            );
				lia.Add (l);
			}

			lowIcons.Add (lia);
		}
	}


	//SET LOW ICON//
	public LowIcon SetLowIcon(int iconNum, bool active, Sprite img1, Sprite img2){
		LowIcon li = new LowIcon ();
		li.icon = iconNum;
		li.isActive = active;
		li.sprite = img1;
		li.selectionSprite = img2;
		li.unitID = unitID;
		return li;
	}

	//SET LOW ACTIONS//
	public void SetLowIconAction(int listNum, int iconNum, List<UnityAction> acts){
		lowIcons [listNum] [iconNum].actions = acts;
	}

	//SET QUICKLIST//
	public List<Vector2> LowQuickList(int count){
		List<Vector2> v = new List<Vector2> ();
		for (int i = 0; i < count; ++i) {
			v.Add(new Vector2(i, 0));
		}
		return v;
	}
	public List<Vector2> LowQuickList(int[] iconNum, int[] actives){
		List<Vector2> v = new List<Vector2> ();
		for (int i = 0; i < iconNum.Length; ++i) {
			v.Add(new Vector2(iconNum[i], actives[i]));
		}
		return v;
	}

	/* ===================================================================================================================================
	 * 
	 * 									Additional Function
	 * 			 			Functions used to in setting Icons for all classes
	 *=================================================================================================================================== */

	//SET IMAGE OR REPLACE WITH DEFAULT IMAGE//
	private Sprite SetImage(int levelID, int ListID, int imgID, bool useSecondary){

		//Determine panel level of image
		if (levelID == 0) {

			//return top level image
			return SetImage_Top (ListID);

		} else if (levelID == 1) {

			//return mid level image
			return SetImage_Mid (ListID, imgID, useSecondary);

		} else if (levelID == 2) {

			//return low level image
			return SetImage_Low (ListID, imgID, useSecondary);
		}

		//Something went wrong, return default
		return (!useSecondary) ? defaultImage1 : defaultImage2;
	}

	//Set top level image
	private Sprite SetImage_Top(int ListID){

		//Upgrade Icon
		if (ListID == 0 && ImageExists(upgradeIcon)) {
			return upgradeIcon;
		}

		//Hire Icon
		else if (ListID == 1 && ImageExists(hireIcon)) {
			return hireIcon;
		}

		//Command Icon
		else if (ListID == 2 && ImageExists(commandIcon)) {
			return commandIcon;
		}

		//something went wrong return default image
		return defaultImage1;
	}

	//Set mid level image
	private Sprite SetImage_Mid(int ListID, int imgID, bool useSecondary){

		//Upgrade Icon
		if (ListID == 0 && midUpgradeImages.Count > 0 && midUpgradeSelected.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(midUpgradeImages[imgID], midUpgradeSelected[imgID])){
				return (!useSecondary) ? midUpgradeImages[imgID] : midUpgradeSelected[imgID];
			}
		}

		//Hire Icon
		else if (ListID == 1 && midHireImages.Count > 0 && midHireSelected.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(midHireImages[imgID], midHireSelected[imgID])){
				return (!useSecondary) ? midHireImages[imgID] : midHireSelected[imgID];
			}
		}

		//Command Icon
		else if (ListID == 2 && midCommandImages.Count > 0 && midCommandSelected.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(midCommandImages[imgID], midCommandSelected[imgID])){
				return (!useSecondary) ? midCommandImages[imgID] : midCommandSelected[imgID];
			}
		}

		//Something went wrong, return default
		return (!useSecondary) ? defaultImage1 : defaultImage2;
	}

	//Set low level images
	private Sprite SetImage_Low(int ListID, int imgID, bool useSecondary){

		//Low Icon List 1
		if (ListID == 0 && low1Images.Count > 0 && low1SelectionImages.Count > 0) {
			
			//confirm image exists, if true return
			if(ImageExists(low1Images[imgID], low1SelectionImages[imgID])){
				return (!useSecondary) ? low1Images[imgID] : low1SelectionImages[imgID];
			}
		}

		//Low Icon List 2
		if (ListID == 1 && low2Images.Count > 0 && low2SelectionImages.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(low2Images[imgID], low2SelectionImages[imgID])){
				return (!useSecondary) ? low2Images[imgID] : low2SelectionImages[imgID];
			}
		}

		//Low Icon List 3
		if (ListID == 2 && low3Images.Count > 0 && low3SelectionImages.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(low3Images[imgID], low3SelectionImages[imgID])){
				return (!useSecondary) ? low3Images[imgID] : low3SelectionImages[imgID];
			}
		}

		//Low Icon List 4
		if (ListID == 3 && low4Images.Count > 0 && low4SelectionImages.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(low4Images[imgID], low4SelectionImages[imgID])){
				return (!useSecondary) ? low4Images[imgID] : low4SelectionImages[imgID];
			}
		}

		//Low Icon List 5
		if (ListID == 4 && low5Images.Count > 0 && low5SelectionImages.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(low5Images[imgID], low5SelectionImages[imgID])){
				return (!useSecondary) ? low5Images[imgID] : low5SelectionImages[imgID];
			}
		}

		//Low Icon List 6
		if (ListID == 5 && low6Images.Count > 0 && low6SelectionImages.Count > 0) {

			//confirm image exists, if true return
			if(ImageExists(low6Images[imgID], low6SelectionImages[imgID])){
				return (!useSecondary) ? low6Images[imgID] : low6SelectionImages[imgID];
			}
		}

		//Something went wrong, return default
		return (!useSecondary) ? defaultImage1 : defaultImage2;
	}


	//Check if image is null
	private bool ImageExists(Sprite img){
		return(img != null);
	}
	private bool ImageExists(Sprite img1, Sprite img2){
		return(img1 != null && img2 != null);
	}



}
