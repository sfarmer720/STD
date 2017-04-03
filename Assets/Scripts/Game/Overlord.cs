﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlord : MonoBehaviour {

	//external references
	public STDMath stdMath;
	public Generator map;
	public Assets assetHolder;
	public SelectionScript selector;
	public QuickKeys quickKeys;
	protected GUIDriver guiCon;

	//Holder lists
	public List<GameObject> units = new List<GameObject> ();
	public List<GameObject> visibleTiles = new List<GameObject>();
	public List<GameObject> visitedTiles = new List<GameObject>();

	//vital variables
	public bool activePlayer;
	public bool isAI;
	public bool onDefense;
	public int gameState = -1;
	private bool transitionFrame;


	//Game Variables
	public float gameTime;
	public float setupEndTime = 10f;
	public float preGameEndTime = 60f;
	public int gold;


	//Defense variables
	private bool[,] defenseDomain = new bool[10,10];

	//Upgrade variables
	protected GameObject upgradeObject;
//	protected Upgrades upgrades;
	protected List<List<GameObject>> upgrades = new List<List<GameObject>> ();


	//Initilize
	public void Initialize(
		STDMath math, Generator gen, Assets asset, SelectionScript selec,
		bool active, bool defense, bool ai
	){

		//assign external variables
		map = gen;
		assetHolder = asset;
		selector = selec;
		stdMath = math;
		Random.InitState (stdMath.seed);
		guiCon = GameObject.Find ("UI").GetComponent<GUIDriver> ();

		//Create upgrade object
		InitUpgradeList(asset);
	
		//Sets living or AI player, and defense or offense
		activePlayer = active;
		isAI = ai;
		onDefense = defense;

		//initialzies per playertype
		if (active) {
			quickKeys = selec.gameObject.GetComponent<QuickKeys> ();
			quickKeys.overlord = this;
		}



		//set GameTime starting
		gameTime = Time.time;

	}

	private void InitUpgradeList(Assets a){
		GameObject go = Instantiate(a.upgradeObject,this.gameObject.transform);

		//create lists
		for (int i = 0; i < 3; ++i) {

			//create list
			List<GameObject> goL = new List<GameObject>();

			//get parent object
			GameObject parent;
			if (i == 0) {
				parent = go.transform.Find ("Offense").gameObject;
			} else if (i == 1) {
				parent = go.transform.Find ("Defense").gameObject;
			} else{
				parent = go.transform.Find ("General").gameObject;
			}

			//get all children in list
			Transform[] children = parent.GetComponentsInChildren<Transform>();
			foreach (Transform child in children) {

				if (child != parent.transform) {
					Debug.Log ("CHILD FOUND: " + child);
					goL.Add (child.gameObject);
				}
			}

			//add list to upgrades
			upgrades.Add(goL);

		}


	}



	//Main Update Method
	protected void FixedUpdate(){


		//Confirm Game States
		if (gameState < 0) {
			IncGameState ();
		}
		gameState = map.GetGameState();

		//Update based on Game State and play type
		//Setup
		if (gameState == 0) {
			if (onDefense) {

				StartDefense ();

			} else {
				
				StartOffense ();
			}
		} 

		//Pre Game
		else if (gameState == 1) {
			if (onDefense) {

				PreDefense ();

			} else {

				PreOffense ();

			}
		}

		//Game updates
		else if (gameState == 2) {
			if (onDefense) {

				MainDefense ();

			} else {

				MainOffense ();

			}
		}

		//set transition frame to false
		//transitionFrame = false;
	}


	/* ===================================================================================================================================
	 * 
	 * 										Setup Functions
	 * 			 					Functions used to set up defense and Offense
	 *=================================================================================================================================== */

	//run initial defense setup
	private void StartDefense(){

		Debug.Log (this.gameObject.name + " is starting defense");


		//set all tiles as visited
		if (visitedTiles.Count < 1) {
			for (int y = 0; y < map.mapSize; ++y) {
				for (int x = 0; x < map.mapSize; ++x) {
					MarkTileVisited (new Vector2 (x, y));
				}
			}
		}

		//check if keep has been placed yet
		if (units.Count < 1) {

			//if keep hasn't been placed, check alloted time
			if (Time.time - gameTime > setupEndTime) {

				//Choose a random tile and place the keep, and advanced gamestate
				Vector2 v = new Vector2 (Random.Range (0, map.mapSize), Random.Range (0, map.mapSize));
				CreateUnit (0, v);
				defenseDomain = stdMath.EstablishDomain(v,map);
				IncGameState ();

			}

		} else {

			//establish domain and advanced game state
			defenseDomain = stdMath.EstablishDomain(GetUnit(0).CurrentTileLocation(),map);
			IncGameState ();

		}

	}

	//run initial Offense
	private void StartOffense(){
		//TODO: add waiting messages while defense sets up
		Debug.Log (this.gameObject.name + " is waiting to start offense");
	}

	/* ===================================================================================================================================
	 * 
	 * 										Pre Game Functions
	 * 			 					Functions used to run pre game
	 *=================================================================================================================================== */

	private void PreDefense(){

		Debug.Log (this.gameObject.name + " is in pre defense");

		//Check pregame timer
		if (Time.time - gameTime > preGameEndTime) {

			//end pregame, increment to main game
			IncGameState();
		}
	}

	private void PreOffense(){

		Debug.Log (this.gameObject.name + " is in pre offense");

		//check if any tiles are visible
		if (visibleTiles.Count < 1) {

			Debug.Log (this.gameObject.name + " is getting potential starting locations");

			//make starting tile visible
			List<Vector2> lv2 = stdMath.GenStartingTiles(map,map.antiplayer.getDefenseDomain(), getIgnorableTiles());
			for (int i = 0; i < lv2.Count; ++i) {
				MakeTileVisible (lv2 [i]);
			}
		}



	}


	/* ===================================================================================================================================
	 * 
	 * 										Main Game Functions
	 * 			 					Functions used to run the primary game
	 *=================================================================================================================================== */

	private void MainDefense(){
		Debug.Log (this.gameObject.name + " is in main defense");
	}

	private void MainOffense(){
		Debug.Log (this.gameObject.name + " is in main offense");
		//transition frame functions
		if (transitionFrame) {

			Debug.Log (this.gameObject.name + " is clearing previous units");
			//clear all units
			for (int i = 0; i < units.Count; ++i) {
				ClearUnit (units [i]);
			}

			//clear all visible tiles
			for (int i = 0; i < visibleTiles.Count; ++i) {
				MarkTileVisited (visibleTiles [i]);
			}

			transitionFrame = false;
		}

	}


	/* ===================================================================================================================================
	 * 
	 * 										Unit Functions
	 * 			 			Functions used to order, create, and maintain units
	 *=================================================================================================================================== */

	//Create Unit
	public void CreateUnit(int unitID, Vector2 location){

		//create physical Unit
		GameObject unit = (onDefense) ? Instantiate (assetHolder.defenseAssets [unitID]) : Instantiate (assetHolder.offenseAssets [unitID]);
		unit.transform.position = new Vector3 (location.x * map.mapSize, 0, location.y * map.mapSize);
		unit.transform.parent = this.gameObject.transform;
		units.Add (unit);

		unit.GetComponent<Unit> ().Init (map, this, location);


		//temp
		if (onDefense) {
			Debug.Log ("THIS SHOULD ONLY BE CALLED ONCE");
			unit.GetComponent<Unit> ().InitSupportClasses2();
		}

		//TODO: Better unit initialization
		/*
		unit.GetComponent<Unit> ().InitUnit (
			map,
			location,
			7, new float[10]
		);
		*/
	}

	//clear unit
	public void ClearUnit(GameObject unit){

		//confirm valid unit
		if (units.Contains (unit)) {

			//remove unit from list, and destroy
			units.RemoveAt(units.IndexOf(unit));
			Destroy (unit);
		}
	}

	//click to place unit
	public void ClickToPlace(int unitID){

		//check if tile has been selected
		if (selector.IsTileSelected ()) {

			//create unit on tile
			CreateUnit(unitID, selector.current.gameObject.GetComponent<Tile>().MapLoc);
		}

	}
		


	/* ===================================================================================================================================
	* 
	* 										Map Functions
	* 			 			Functions used to interact with Game Map
	*=================================================================================================================================== */

	//Make tile visible
	public void MakeTileVisible(Vector2 v){
		GameObject go = map.GetTile (v);
		MakeTileVisible (go);
	}
	public void MakeTileVisible(GameObject go){

		//check if already visible
		if (!visibleTiles.Contains (go)) {

			//check if tile was visited
			if (visitedTiles.Contains (go)) {

				//remove from visited tiles
				visitedTiles.RemoveAt (visitedTiles.IndexOf (go));
			}

			//change tile layer & add to visible list
			if (activePlayer) {
				go.GetComponent<Tile> ().SetToVisible ();
			}

			visibleTiles.Add (go);
		}
	}

	//Mark tile visited
	public void MarkTileVisited(Vector2 v){
		GameObject go = map.GetTile (v);
		MarkTileVisited(go);
	}
	public void MarkTileVisited(GameObject go){
		
		//check if already marked visited
		if (!visitedTiles.Contains (go)) {

			//check if tile is currently visible
			if (visibleTiles.Contains (go)) {

				//remove from visible tiles
				visibleTiles.RemoveAt (visibleTiles.IndexOf (go));
			}

			//change tile layer & add to visited list
			if (activePlayer) {
				go.GetComponent<Tile> ().SetToVisited ();
			}

			visitedTiles.Add (go);
		}
	}



	/* ===================================================================================================================================
	* 
	* 										Getters & Setters
	* 			 			Functions used to pull private variables
	*=================================================================================================================================== */

	public void IncGameState(){
		transitionFrame = true;
		selector.ClearSelection ();
		++gameState;
		gameTime = Time.time;
	}
	public bool[,] getDefenseDomain(){
		return defenseDomain;
	}
	public int[] getIgnorableTiles(){
		//TODO: sest which tiles to be ignored
		int[] i = {-1,-1,-1,-1,4,5,6,7};
		return i;
	}


	//GET UNIT//
	public Unit GetUnit(int i){
		return units [i].GetComponent<Unit> ();
	}

	//Pass GUI Driver
	public GUIDriver GUI(){
		if (activePlayer) {
			return guiCon;
		} else {
			return null;
		}
	}

	//PASS UPDATE
	public GameObject GetUpgrade(int ODID, int unitID){
		Debug.Log ("GETTING UPGRADE OBJECT: " + upgrades [ODID] [unitID]);
		Debug.Log (upgrades [0] [0]);
		return upgrades [ODID] [unitID];
	}

}
