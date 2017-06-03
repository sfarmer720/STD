using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    private int[] tileLayers = { 12, 13, 14 };

    //cross class variables
    public UnitAssets assets;
    public UnitBodyTrigger bodyTrig;
    public CharacterController con;
    protected STDMath stdMath;
	protected Overlord overlord;
	protected Generator mainMap;
    
    //Character controller
    protected UnitSight sight;
	protected UnitSelection selection;
	protected UnitMovement move;
    protected UnitHealing healing;
	protected Animator anim;

    //Unit Information
    public int defaultLayer;
    public int defaultLayerOutline;
    public int unitAI;                      //Unit AI determines Automated behaviors. 0: N/A, 1: Aggressive, 2: Protective, 3: Defensive, 4: Survivalist
    public int currentClass = 0;
    public Vector2 tileLocation;
    public Tile currentTile;


    //Actions and Stats
    public UnitInfo[] unitClasses;
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
		anim = this.gameObject.GetComponent<Animator> ();
		//con = this.gameObject.AddComponent<CharacterController> ();
		//con.slopeLimit = 90;
		//con.center = new Vector3 (0, 1, 0);

		//set sight plane size
		tileLocation = loc;

        //set default layers
        defaultLayer = overlord.defualtLayer;
        defaultLayerOutline = overlord.defualtLayerOutline;

        //set friendly/enemy status
        baseStats.overlord = overlor;
        baseStats.isEnemy = !overlor.activePlayer;
	}
	protected void InitSupportClasses(){
		Debug.Log (baseStats.type + " | " + baseStats.moveCosts);

        //Initialize Sight Class
        sight = this.gameObject.AddComponent<UnitSight>();
        Debug.Log(this+" | "+sight + " | " +overlord + " | " +assets + " | "  +mainMap + " | " +baseStats.sight);
        sight.InitSight( this, overlord, assets.sightSphere, baseStats.sight, mainMap.tileWidth);

		//Initialize Movement Class
		move = this.gameObject.AddComponent<UnitMovement> ();
		move.InitMovement(mainMap,assets, baseStats.speed,baseStats.moveCosts,tileLocation,con);

		//Initialize Selection Class
		selection = this.gameObject.AddComponent<UnitSelection> ();
		selection.InitSelection (this);

        //initialize body trigger
        bodyTrig.Init(this, mainMap);

	}

	public void InitSupportClasses2(){
        sight = this.gameObject.AddComponent<UnitSight>();
       // sight.InitSight(this, overlord, assets.sightSphere, sightRadius, mainMap.tileWidth);
        move = this.gameObject.AddComponent<UnitMovement> ();
      //  move.InitMovement(mainMap, assets, baseStats.speed, baseStats.moveCosts, tileLocation);
        selection = this.gameObject.AddComponent<UnitSelection> ();
		//selection.InitSelection (this);
	}


	//update unit on fixed schedule before physics - NOT MAIN UPDATE
	void FixedUpdate(){

		//Set selection and move if still null
	/*	if (selection == null && move == null && sight == null) {
			InitSupportClasses ();
		}
    */
        // Check if need to set layer
        if (this.gameObject.layer != defaultLayer) {
			selection.SetLayers (defaultLayer);
		}

        //update size of sight sphere
        sight.UpdateSight();

		//check for movement updates, and update tile location
		move.MovementUpdate();
        tileLocation = move.CurrentTile();



		// CALL AFTER ALL OTHER UPDATES//
		//Update Menu

		//update anim
		UpdateAnim();
	}

	//Final update before render - NOT MAIN UPDATE
	void LateUpdate(){
		//check if tile is selected
		if (selection != null && selection.isSelected) {
			selection.SetLayers (defaultLayerOutline);
		}
	}




	/* ===================================================================================================================================
	 * 
	 * 									Selection
	 * 			 			Functions to control Unit selection
	 *=================================================================================================================================== */


	public void Selection(){
		selection.isSelected = !selection.isSelected;

		//Update menu

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
    /*
	public void UnitSelection(int clickType, Transform t, Vector3 v, bool active){
		if (active) {
			selection.Selection (clickType);
		} else {
			selection.Selection (t,v);
		}
	}
    */
	//Move to tile
	public void UnitToTile(Transform t){
		Debug.Log ("Moving " + this.gameObject.name + " to tile " + t.gameObject.name);
		move.MoveTo (t);
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
	 * 									Override Functions
	 * 			 			Functions to be overriden by children classes
	 *=================================================================================================================================== */
    //get units upgrade class
    public virtual Upgrade GetUpgrades()
    {
        return null;
    }

    //Change unit classes
    public virtual void ChangeClass(int i, int cost)
    {
        //check if can afford class change
        if (currentClass != i && stdMath.CanAfford(overlord, cost))
        {
            //charge overlord, set new class
            overlord.SubtractGold(cost);

            //undo upgrades
            GetUpgrades().SetAllUpgrades(false);

            //change class
            int newHP = stdMath.UpdateHP(currentStats.HP, currentStats.maxHP, unitClasses[i].maxHP);
            currentStats = unitClasses[i];
            currentStats.HP = newHP;

            //Set upgrades
            GetUpgrades().SetClassUpgrades(currentStats.classUpgrades);
            GetUpgrades().SetAllUpgrades(true);
        }
    }

    //Take Damage - 0: unit, 1: building, 2: trap
    public virtual void Damage(int dmg, int dmgType)
    {
        //apply damage to HP
        currentStats.HP -= dmg;
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


    //Get seperate Classes
    public UnitMovement Movement()
    {
        return move;
    }
    public UnitHealing Healing()
    {
        return healing;
    }

    //Get Stats
    public UnitInfo BaseStats()
    {
        return baseStats;
    }
    public UnitInfo CurrentStats()
    {
        return currentStats;
    }

}



//Unit Structure
public class UnitInfo
{

	//Casting Information
	public int offenseDefenseID;		// uses 0 for defense & 1 for Offense
	public int unitID;					// Determines unit type
    public int unitClass;
    public int numUnits;
	public int playerID;				// Player unit is assigned to
    public bool isBuilding = false;
    public bool isKeep = false;

    //Overlord Information
    public Overlord overlord;
    public bool isEnemy;

	//Base Information
	public string type;
	public int HP;
	public int maxHP;
	public int cost;
    public int range;
	public float speed;
	public float sight;
	public float attack;
	public float defense;
	public float evade;
	public float siegeMod;
    public float detection;
	public float[] moveCosts;
	public bool isRanged;
    public bool canDetect;


    //Selection
    public bool inGroup = false;
    public SelectionGroup group;

	//Menu
	public bool showHire = false;
	public string contentButtonText;

    //upgrades
    public ClassUpgrade[] classUpgrades;

    //Combat information
    public Unit currentEnemy;
    
}
