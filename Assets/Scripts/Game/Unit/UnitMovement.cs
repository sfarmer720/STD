using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour {

	//External Classes
	private Generator mainMap;
	private STDMath stdMath;
	private int[,] tileMap;
	private Tile currTile;
	private CharacterController con;

	//Stat Variables
	private float baseSpeed;
	private float[] moveCosts;
	private Vector2 mapLoc;

	//Movement Variables
	private Vector3 curPos;
	private Vector3 finPos; 
	private List<Vector2> movePath = new List<Vector2> ();
	private List<Vector3> moveNodes = new List<Vector3> ();
	private bool isMoving = false;

	//Path variables
	private LineRenderer pathRenderer;

	/* ===================================================================================================================================
	 * 
	 * 									Core Functions
	 * 			 				Functions used to initialize and get updates
	 *=================================================================================================================================== */


	// Use this for initialization
	public void InitMovement(
		Generator mainmap, CharacterController co,
		float speed, float[] costs, Vector2 loc
	){

		//initialize external classes
		con = co;
		mainMap = mainmap;
		tileMap = mainMap.GetMap ();
		stdMath = mainMap.stdMath;
		currTile = mainMap.GetTile (loc).GetComponent<Tile> ();
		Random.InitState (stdMath.seed);

		//Initialize base stats
		baseSpeed = speed;
		moveCosts = costs;

		//Initialize private
		curPos = this.gameObject.transform.position;
		mapLoc = loc;


		//initilaize path renderer
		pathRenderer = new GameObject().AddComponent<LineRenderer>();
		pathRenderer.gameObject.name = "Path Renderer";
		pathRenderer.gameObject.transform.parent = this.gameObject.transform;
		pathRenderer.gameObject.layer = 28;
		pathRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		pathRenderer.receiveShadows = false;
		pathRenderer.useWorldSpace = true;
		pathRenderer.SetPosition (0, this.gameObject.transform.position);


	}

	//Use this when calling for updates
	public void MovementUpdate(){

		//set current position
		curPos = this.gameObject.transform.position;

		//check if Unit needs to Move
		if (isMoving) {
			Move ();
		}

		//constantly apply gravity
		con.SimpleMove(stdMath.ApplyGravity(new Vector3()));

		//update path renderer
		UpdatePathRender();

	}

	//Main Movement function
	private void Move(){

		//Check if current position equals final position
		if (!PosReached(finPos)) {

			//Not at final position, movement still needed. Check if reached current node
			if (PosReached(moveNodes[0])) {

				//remove current node
				moveNodes.RemoveAt (0);

				//update path renderer
				UpdateNodes(moveNodes);
			} 

			//Set Look at towards next position, account for unit hieght

			//this.gameObject.transform.rotation = Quaternion.LookRotation (moveNodes [0] - curPos);

			Vector3 v = moveNodes[0];
			v.y = curPos.y;
			this.gameObject.transform.LookAt (v);
			//Debug.Log ("Looking towards " + moveNodes[0] +" with a look rotaion of "+Quaternion.LookRotation (moveNodes [0] - curPos));

			//Move towards next position
			con.Move(stdMath.MoveVec(this.gameObject.transform.forward, Speed()));

		} else {

			Debug.Log ("Unit Reached End");

			//at final position, end movement
			isMoving = false;
			movePath.Clear ();
			moveNodes.Clear ();
		}


	}


	//Initiate Movement
	public void MoveTo(Transform t, Vector3 finalPos){

		//confirm unit can or should move
		if (Speed () > 0) {

			//convert transform selection to path
			moveNodes.Clear();
			movePath = stdMath.SelectToPath(t, mapLoc,tileMap,moveCosts,false);

			//Set is moving and final Postion, and remove starting position
			isMoving = true;
			finPos = finalPos;
			movePath.RemoveAt (0);

			//Create Node list from path list
			for (int i = 0; i < movePath.Count; ++i) {
				moveNodes.Add (GetNewTile (movePath [i]).TileLoc);
			}

			//set last node to final position
			moveNodes[moveNodes.Count-1] = finPos;

			//set pathrenderer
			SetPathRenderer(moveNodes);

			Move ();
		}

	}



	//update visible path
	private void UpdatePathRender(){

		//check if moving
		if (isMoving) {

			// check if enabled and constantly update initial position
			pathRenderer.enabled = true;
			pathRenderer.SetPosition(0,this.gameObject.transform.position);

			//check if node has been reached

		} else {

			//unit is not moving, reset and disable path rendere
			pathRenderer.positionCount = 1;
			pathRenderer.enabled = false;

		}
	}

	//set Path renderer
	private void SetPathRenderer(List<Vector3> nodes){

		//enable and set nodes
		pathRenderer.enabled = true;
		UpdateNodes (nodes);
	}

	//shift path renderer down one
	private void UpdateNodes(List<Vector3> nodes){

		pathRenderer.positionCount = 1;
		pathRenderer.positionCount = nodes.Count + 1;
		pathRenderer.SetPosition(0,this.gameObject.transform.position);

		//cycle nodes list
		for (int i = 0; i < nodes.Count; ++i) {

			//set position to node position
			Vector3 v = nodes[i];
			v.y = GetNewTile (movePath [i]).highestY * 1.1f;
			pathRenderer.SetPosition(i+1,v);
		}
	}

	/* ===================================================================================================================================
	 * 
	 * 									Getters / Setters
	 * 			 				Used to get modified stats
	 *=================================================================================================================================== */

	//Is unit moving
	public bool IsMoving(){
		return isMoving;
	}
	//GET CURRENT TILE LOCATION
	public Vector2 CurrentTile(){
		return mapLoc;
	}

	//SET MAP LOCATION FROM TILE//
	public void SetNewTileLocation(Vector2 loc, Tile t){
		mapLoc = loc;
		currTile = t;

		//check if unit is moving
		if (isMoving && movePath.Count > 0) {

			//Remove previous tile from path
			movePath.RemoveAt (0);

		}
	}

	//GET MODIFIED SPEED//
	public float Speed(){

		//TODO: functions to modify speed
		return baseSpeed;
	}
	//MOVEMENT OVER TILE SPEED//
	public float SpeedOverTile(){

		return stdMath.SpeedOverTile (Speed (), CurrentMoveCost ());
	}

	//GET MODIFIED MOVE COSTS//
	public float[] MoveCosts(){

		//TODO: functions to modify move costs
		return moveCosts;
	}
	public float CurrentMoveCost(){
		float[] fa = MoveCosts ();
		return fa [GetCurrentTile ().tileType];
	}



	//Get New Tile
	private Tile GetNewTile(Vector2 v){
		return mainMap.GetTile (v).GetComponent<Tile> ();
	}
	//Get Current Tile information
	private Tile GetCurrentTile(){
		return GetNewTile (CurrentTile ());
	}

	//Check if unit has reached a position
	private bool PosReached(Vector3 v){
		return (stdMath.withinDev(curPos.x,v.x,0.1f) && stdMath.withinDev(curPos.z,v.z,0.1f)); 
	}
	//Get tile from transform
	private Tile TileFromTrigger(Collider c){
		return c.gameObject.transform.parent.gameObject.GetComponent<Tile> ();
	}


}
