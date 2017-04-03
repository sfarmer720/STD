using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	//cross class variables
	protected STDMath stdMath;
	protected Overlord overlord;
	protected Generator mainMap;
	protected Upgrades baseUpgrades;
	protected GameObject upgradeObject;

	//Character controller
	protected UnitSelection selection;
	protected UnitMovement move;
	protected UnitMenu menu;
	protected CharacterController con;
	protected Animator anim;
	protected SphereCollider sight;

	//Unit Information
	public Vector2 tileLocation;
	protected float sightRadius = 7;
	protected UnitInfo baseStats = new UnitInfo();
	protected UnitInfo currentStats = new UnitInfo();
	protected UnityAction upgradeAction = null;
	protected UnityAction commandAction = null;
	protected UnityAction hireAction = null;

	/*TODO: 
	 * 
	 * Slection currently hard coded for only player units, needs to work for enemy too
	 * 
	 * 
	 * 
	 * 
	 * 
	 */


	//Class Constructor
	public virtual void Init(Generator mainmap, Overlord overlor, Vector2 loc){
		//set cross variables
		mainMap = mainmap;
		overlord = overlor;
		stdMath = mainmap.stdMath;
		Random.InitState (stdMath.seed);

		//Initialize Character Controller
		sight = this.gameObject.GetComponentInChildren<SphereCollider> ();
		anim = this.gameObject.GetComponent<Animator> ();
		con = this.gameObject.AddComponent<CharacterController> ();
		con.slopeLimit = 90;
		con.center = new Vector3 (0, 1, 0);

		//set sight plane size
		tileLocation = loc;
	}
	protected void InitSupportClasses(){
		Debug.Log (baseStats.type + " | " + baseStats.moveCosts);

		//Initialize Movement Class
		move = this.gameObject.AddComponent<UnitMovement> ();
		move.InitMovement(mainMap,con,baseStats.speed,baseStats.moveCosts,tileLocation);

		//Initialize Selection Class
		selection = this.gameObject.AddComponent<UnitSelection> ();
		selection.InitSelection (this);

		//initialize menu class
		menu = this.gameObject.AddComponent<UnitMenu>();
		menu.Init (overlord.GUI ());

	}

	public void InitSupportClasses2(){
		Debug.Log (baseStats.type + " | " + baseStats.moveCosts);
		move = this.gameObject.AddComponent<UnitMovement> ();
		move.InitMovement(mainMap,con,baseStats.speed,baseStats.moveCosts,tileLocation);
		selection = this.gameObject.AddComponent<UnitSelection> ();
		selection.InitSelection (this);
	}


	//update unit on fixed schedule before physics - NOT MAIN UPDATE
	void FixedUpdate(){

		//Set selection and move if still null
		if (selection == null && move == null) {
			InitSupportClasses ();
		}

		// Check if need to set layer
		if (this.gameObject.layer != 11) {
			selection.SetLayers (11);
		}

		//update size of sight sphere
		if (sight != null) {
			sight.radius = sightRadius * mainMap.tileWidth * 0.5f;
		}

		//check for movement updates
		move.MovementUpdate();



		// CALL AFTER ALL OTHER UPDATES//
		//Update Menu
		if (selection.isSelected) {
			menu.UpdateUnitMenu (currentStats);
		}

		//update anim
		UpdateAnim();
	}

	//Final update before render - NOT MAIN UPDATE
	void LateUpdate(){
		//check if tile is selected
		if (selection != null && selection.isSelected) {
			selection.SetLayers (29);
		}
	}

	//Initialize Unit
	public void InitUnit(Generator mainmap,	Vector2 loc, float speed, float[] mcosts){

		//set cross variables
		mainMap = mainmap;
		stdMath = mainmap.stdMath;
		Random.InitState (stdMath.seed);

		//Initialize Character Controller
		sight = this.gameObject.GetComponentInChildren<SphereCollider>();
		anim = this.gameObject.GetComponent<Animator>();
		con = this.gameObject.AddComponent<CharacterController>();
		con.slopeLimit = 90;
		con.center = new Vector3 (0, 1, 0);

		//Selection - Attach and Init
		selection = this.gameObject.AddComponent<UnitSelection>();
		selection.InitSelection (this);

		//Movement - Attach and Init
		move = this.gameObject.AddComponent<UnitMovement>();
		move.InitMovement (mainmap, con, speed,mcosts,loc);

		//set sight plane size
		tileLocation = loc;

	}


	/* ===================================================================================================================================
	 * 
	 * 									Selection
	 * 			 			Functions to control Unit selection
	 *=================================================================================================================================== */


	public void Selection(){
		selection.isSelected = !selection.isSelected;

		//Update menu
		if (selection.isSelected) {

			//unit is now selected
			menu.AttachToMenu(currentStats);

		} else {

			//unit no longer selected
			menu.RemoveFromMenu(currentStats);
		}

		/*
		//open Menu
		GUIDriver gui = overlord.GUI();
		if (gui != null) {

			//If Selction is true, set content button
			if (selection.isSelected) {
				if (hireAction == null) {
					upgrades.SetContentButton (false, upgradeAction, commandAction);
				} else {
					upgrades.SetContentButton (true, upgradeAction, commandAction, hireAction);
				}
			}

			gui.MenuControl (selection.isSelected);
		}
		*/
	}

	public void Selection(Transform t, Vector3 v){
		move.MoveTo (t, v);
	}

	public void UnitSelection(int clickType, Transform t, Vector3 v, bool active){
		if (active) {
			selection.Selection (clickType);
		} else {
			selection.Selection (t,v);
		}
	}

	//Move to tile
	public void UnitToTile(Transform t, Vector3 v){
		Debug.Log ("Moving " + this.gameObject.name + " to tile " + t.gameObject.name);
		move.MoveTo (t, v);
	}


	/* ===================================================================================================================================
	 * 
	 * 									Animation updates
	 * 						 			Set animation states
	 *=================================================================================================================================== */

	//Main update method
	private void UpdateAnim(){

		//confirm animator is attached
		if (anim != null) {

			//set anim variables
			anim.SetBool ("IsMoving", move.IsMoving ());
			anim.SetFloat ("MoveSpeed", move.SpeedOverTile ());

		}
	}


	/* ===================================================================================================================================
	 * 
	 * 									Getters & Setters
	 * 			 			Used to pull variables from variaous scripts
	 *=================================================================================================================================== */

	//GET MOVEMENT VARIABLES//
	public Vector2 CurrentTileLocation(){
		return move.CurrentTile ();
	}
	public UnitMovement Movement(){
		return move;
	}

	//SET LOCATION FROM TILE TRIGGER//
	public void SetTileFromTrigger(Vector2 loc, Tile t){
		move.SetNewTileLocation (loc, t);
	}
		

}



//Unit Structure
public class UnitInfo
{

	//Casting Information
	public int offenseDefenseID;		// uses 0 for defense & 1 for Offense
	public int unitID;					// Determines unit type and class
	public int playerID;				// Player unit is assigned to

	//Base Information
	public string type;
	public int HP;
	public int maxHP;
	public int cost;
	public float speed;
	public float sight;
	public float attack;
	public float defense;
	public float evade;
	public float siegeMod;
	public float[] moveCosts;
	public bool isRanged;

	//Menu
	public bool showHire = false;
	public string contentButtonText;
	public TopIcon[] topIcons;
	public List<List<MidIcon>> midIcons = new List<List<MidIcon>> ();
	public List<List<LowIcon>> lowIcons = new List<List<LowIcon>> ();
}
