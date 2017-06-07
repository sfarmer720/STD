using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript  : MonoBehaviour {

	//Selections
	public Transform current;
	public Transform previous;
	public Transform passive;

	//Private varaibles
	private Vector3 hitPoint;
	private Transform holder;
	private int mask;
	private Camera cam;

	//listner variables
	private bool tileIsSelected = false;
	private bool unitIsSelected = false;
	private bool enemeyIsSelected = false;

	//Layer numbers
	private int visibleTile = 12;
	private int visitedTile = 13;
	private int unitLayer = 11;
	private int enemyLayer = 10;
	private int tileSelectLayer = 31;
	private int enemySelectLayer = 30;
	private int unitSelectLayer = 29;


	//debug varibales
	public bool drawRays = false;
	private Vector3 temp;
	private Vector3 forward;


	//initialize
	void Start(){

		//create selection mask
		int[] masks = {8, 10, 11, 12, 13 };

		//combine masks
		for (int i = 0; i < masks.Length; ++i) {
			int m = 1 << masks [i];
			mask = mask | m;
		}
        Debug.Log(mask + " Old Mask");
        //assign camera
        cam = Camera.main;

	}

	//Initialize selection
//	public void InitSelection(int m, Camera c){
//		mask = m;
//		cam = c;
//	}

	//used for debuging
	void Update(){


		if (drawRays) {
			Debug.DrawRay (temp, forward, Color.red);
		}
	}


//Casting function
	public void SelectionCast(Vector3 castFrom){
		//create raycast
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay(castFrom);

		//debug variables
		temp = ray.origin;
		forward = ray.direction * 1000;

		//Raycast with mask
		if(Physics.SphereCast(ray, 1, out hit, Mathf.Infinity, mask)){

			//Check if ray cast hit anything
			if(hit.transform != null){

				//set active selection
				holder = hit.transform;
				hitPoint = hit.point;
				current = SelectionAction (hit.transform);

			}
		}

	}

	//determine action taken
	public Transform SelectionAction(Transform t){

		//confirm non null
		if (t != null) {

			//check type of selection
			if (isTile (t.gameObject)) {
				t = TileAction (t);
			} else if (isEnemy (t.gameObject)) {

			} else if (isUnit (t.gameObject)) {
				t = UnitAction (t);
			}
		}
		return t;
	}

	//Tile action
	private Transform TileAction(Transform t){

		//check for previous selection
		if (current != null) {

			//Check for reselect
			if (current == t) {
				GetTile (t).Selection ();
				return null;
			}

			//check if previous was tile
			if (isTile (current.gameObject)) {

				//update current selected tile
				GetTile (current).Selection ();
				GetTile (t).Selection ();
				return t;
			}

			//check if previous was enemy
			else if (isEnemy (current.gameObject)) {

			}

			//check if previous was unit
			else if (isUnit (current.gameObject)) {

				//Move unit to tile
				GetUnit (current).UnitToTile (t);
				return current;
			}

		}

		//set tile as selection
		GetTile(t).Selection();
		SetTileSelect();
		return t;
	}

	//Enemy action
	private void EnemyAction(Transform t){

	}

	//Unit action
	private Transform UnitAction(Transform t){

		//check for previous selection
		if (current != null) {

			//Check for reselect
			if (current == t) {
				GetUnit (t).Selection ();
				return null;
			}

			//check if previous was tile
			if (isTile (current.gameObject)) {

				//deselect tile, select unit
				GetTile(current).Selection();
			}

			//check if previous was enemy
			else if (isEnemy (current.gameObject)) {

			}

			//check if previous was unit
			else if (isUnit(current.gameObject)) {

				//update current selected unit
				GetUnit(current).Selection();
			}

		}

        //set unit to active selection
		GetUnit(t).Selection();
		SetUnitSelect ();
		
		return t;
	}




	//Primary Selection function
	public void SelectionCast(Vector3 castFrom, bool active){

		temp = castFrom;

		//create raycast
		RaycastHit hit;
		Ray ray = cam.ScreenPointToRay(castFrom);

		//debug variables
		temp = ray.origin;
		forward = ray.direction * 1000;

		//Raycast with mask
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, mask)){

			//Check if ray cast hit anything
			if(hit.transform != null){

				//determine if active or passive
				if(active){
					
					//clear passive selection
					passive = null;

					//toggle current and previous selections
					current = SelectionToggle(hit.transform,true);

				}else{

					//check if current is valid
					if (current != null) {
						PassSelect (hit.transform, hit.point);
					}
				}
			}
		}
	}

	//Active Selection
	private void ActiveSelect(Transform t){

		//clear passive selection
		passive = null;

		//toggle current and previous selections
		current = SelectionToggle(t,true);

	}



	//Get selection object
	private Transform SelectionToggle(Transform t, bool active){

		//confirm not null
		if (t != null) {
			Debug.Log (t);
			//unset current and set previous
			if(active && current != null){
				previous = SelectionToggle (current, false);
			}

			//Check if tile, enemy, or Unit
			if (isVisibleTile(t.gameObject) || isVisitedTile(t.gameObject)) {
				
				//activate selection
				//Debug.Log (t);
				t.gameObject.GetComponent<Tile>().Selection();

			} else if (isEnemy(t.gameObject)) {

			} else if (isUnit(t.gameObject)) {

				//activate selection
				//Debug.Log (t);
				//t = UnitTransform(t,active);
				//Debug.Log (t);
				t.gameObject.GetComponent<Unit> ().Selection ();
			}


		}

		//return null or modified transform
		return t;
	}

	//Pass selected object
	private void PassSelect(Transform t, Vector3 v){

		//check if null
		if (t != null) {
			
			//Check if tile, enemy, or Unit
			if (isVisibleTile(t.gameObject) || isVisitedTile(t.gameObject)) {

				//pass selection

			} else if (isEnemy(t.gameObject)) {

			} else if (isUnit(t.gameObject)) {

				//activate selection
				current.gameObject.GetComponent<Unit> ().Selection (t,v);
			}
		}
	}

	//get unit transform
	private Transform UnitTransform(Transform t, bool active){
		if (active) {
			return t.gameObject.transform.parent.transform;
		} else {
			return t;
		}
	}


	/* ===================================================================================================================================
	 * 
	 * 									Getters & Setters
	 * 			 			Used to pull variables from variaous scripts
	 *=================================================================================================================================== */


	//BOOLEAN CHECKS//
	public bool isValid(GameObject go){
		return (isUnit(go) || isEnemy(go) || isVisibleTile(go) || isVisitedTile(go));
	}
	public bool isUnit(GameObject go){
		return (go.layer == unitLayer || go.layer == unitSelectLayer || go.layer == 8);
	}
	public bool isEnemy(GameObject go){
		return (go.layer == enemyLayer || go.layer == enemySelectLayer);
	}
	public bool isTile(GameObject go){
		return(isVisibleTile (go) || isVisitedTile (go));
	}
	public bool isVisibleTile(GameObject go){
		return (go.layer == visibleTile || go.layer == tileSelectLayer);
	}
	public bool isVisitedTile(GameObject go){
		return (go.layer == visitedTile || go.layer == tileSelectLayer);
	}


	//SHORT HANDS//
	private Tile GetTile(Transform t){
		return t.gameObject.GetComponent<Tile> ();
	}
	private Unit GetUnit(Transform t){
        return t.gameObject.transform.parent.gameObject.GetComponent<Unit>();
	}

	//LISTENER CHECKS//
	public bool IsUnitSelected(){
		return unitIsSelected;
	}
	public bool IsTileSelected(){
		return tileIsSelected;
	}
	public bool IsEnemySelected(){
		return enemeyIsSelected;
	}
	public bool IsSelection(){
		return (IsUnitSelected () || IsTileSelected () || IsEnemySelected ());
	}
	public void SetUnitSelect(){
		unitIsSelected = true;
		tileIsSelected = false;
		enemeyIsSelected = false;
	}
	public void SetTileSelect(){
		unitIsSelected = false;
		tileIsSelected = true;
		enemeyIsSelected = false;
	}
	public void SetEnemySelect(){
		unitIsSelected = false;
		tileIsSelected = false;
		enemeyIsSelected = true;
	}
	public void ClearSelection(){
		unitIsSelected = tileIsSelected = enemeyIsSelected = false;
		current = null;
	}
	public void SetSelection(Transform t){
		ClearSelection ();
		SelectionAction (t);
	}
		

}
