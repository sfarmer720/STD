using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovement : MonoBehaviour {

    /*
     * Move functions
     * Used to control units placement in the world.
     * Individual units move within a tile only, world movement is controled by the master game object.
     * 
     */


    //External Classes
    private Unit unit;
    private Overlord overlord;
    private UnitFormation unitFormation;
    private UnitAssets assets;
	private Generator mainMap;
	private STDMath stdMath;
	private int[,] tileMap;
	private Tile currTile;
    private CharacterController mainCon;

	//Stat Variables
	private float baseSpeed;
	private float[] moveCosts;
	private Vector2 mapLoc;

    //Movement Variables
    private float startTime;
    private List<Vector3> movePositions = new List<Vector3>();
	private Vector3 curPos;
    private Vector3 initPos;
	private Vector3 finPos; 
	private List<Vector2> movePath = new List<Vector2> ();
	private List<Vector3> moveNodes = new List<Vector3> ();
	private bool isMoving = false;

    //Patrol Variables
    public bool isPatrolling;
    private bool settingPatrol;
    private bool patrolSet_T1;
    private Tile T1, T2;
    private List<Vector2> patrolPath = new List<Vector2>();
    private List<Vector3> patrolNodes = new List<Vector3>();

	//Path variables
	private LineRenderer pathRenderer;
    
 

    /* ===================================================================================================================================
	 * 
	 * 									Core Functions
	 * 			 				Functions used to initialize and get updates
	 *=================================================================================================================================== */


    // Use this for initialization
    public void InitMovement(
		Generator mainmap, UnitAssets ass, Overlord over, Unit u,
		float speed, float[] costs, Vector2 loc,
        CharacterController mainController
	){

        //initialize external classes
        overlord = over;
        unit = u;
        assets = ass;
		mainMap = mainmap;
		tileMap = mainMap.GetMap ();
		stdMath = mainMap.stdMath;
		currTile = mainMap.GetTile (loc).GetComponent<Tile> ();
        mainCon = mainController;
		Random.InitState (stdMath.seed);

		//Initialize base stats
		baseSpeed = speed;
		moveCosts = costs;

		//Initialize private
		curPos = this.gameObject.transform.position;
		mapLoc = loc;

        //Initialize Formation
      //  unitFormation = this.gameObject.AddComponent<UnitFormation>();
       // unitFormation.Init(stdMath);
        

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

        //check if a patrol is being set
        SetPatrol();

		//check if Unit needs to Move
		if (isMoving) {
			Move ();
		}

        //Check unit is in formation
       // unitFormation.CheckFormation(Speed());

        //constantly apply gravity to each unit
        ApplyGravity();

		//update path renderer
		UpdatePathRender();

	}

     //Apply Gracity to all units and set forwards
     private void ApplyGravity()
    {
        for(int i = 0; i < assets.unitInfo.Length; ++i)
        {
            if (assets.unitInfo[i].con != null)
            {
                assets.unitInfo[i].con.SimpleMove(stdMath.ApplyGravity(new Vector3()));
            }
        }

        mainCon.SimpleMove(stdMath.ApplyGravity(new Vector3()));
    }

	//Main Movement function
	private void Move(){

        //Check for move nodes, and if current position is final position
        if (moveNodes.Count > 0 && !PosReached(moveNodes[moveNodes.Count - 1]))
        {
            
            //check if next node has been reached
            if (PosReached(moveNodes[0]))
            {

                Debug.Log("Unit has reached node " + moveNodes[0]);

                //Set init position and remove node
                initPos = moveNodes[0];
                moveNodes.RemoveAt(0);

                //update path renderer
                UpdateNodes(moveNodes);

                //Start new timer
                startTime = Time.time;

                Debug.Log("Moving to new node");
            }
            
            //Set lookat and move
            Vector3 v = moveNodes[0];
            v.y = this.gameObject.transform.position.y;
            this.gameObject.transform.LookAt(v);

            mainCon.Move(this.gameObject.transform.forward * Speed() * Time.smoothDeltaTime);
        }
        else{
            Debug.Log("Unit Reached End");

            //Reached end check if patroling
            if (isPatrolling)
            {
                UpdatePatrolPath();
            }
            else
            {
                //end movement
                StopMovement();
            }
        }
	}


	//Initiate Movement
	public void MoveTo(Transform t, Vector3 finalPos, int crash){

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
            Debug.Log(moveNodes.Count);
			Move ();
		}
	}
    public void MoveTo(Transform t)
    {
        //confirm unit can or should move
        if (Speed() > 0)
        {

            Debug.Log("Unit is starting at " + this.gameObject.transform.position);

            //convert transform selection to path
            moveNodes.Clear();
            movePath = stdMath.SelectToPath(t, mapLoc, tileMap, moveCosts, false);

            //Set is moving and remove starting position
            isMoving = true;
            movePath.RemoveAt(0);

            //Create Node list from path list
            for (int i = 0; i < movePath.Count; ++i)
            {
                moveNodes.Add(GetNewTile(movePath[i]).TileLoc);
            }

            //set pathrenderer
            SetPathRenderer(moveNodes);

            //set initial move positions
            //SetMovePostions();

            //Set init position and move units
            initPos = this.gameObject.transform.position;
            startTime = Time.time;
            Move();
        }
    }


    //Set patrol route
    public void SetPatrol(bool startSet = false)
    {
        //check if starting to set patrol
        if(startSet || settingPatrol)
        {
            settingPatrol = true;

            //TODO: Check if exit button has been pressed
            if (true)
            {
                //check if a first tile has been set
                if (patrolSet_T1)
                {
                    //check if 2nd tile has been selected
                    if (overlord.selector.IsSelection())
                    {
                        if (overlord.selector.IsTileSelected())
                        {
                            //tile is selcted, set to 2nd tile
                            T2 = overlord.selector.current.GetComponent<Tile>();
                        }
                        else if (overlord.selector.IsEnemySelected() || overlord.selector.IsUnitSelected())
                        {
                            //enemy or unit selected, pull and set tile
                            T2 = overlord.selector.current.GetComponent<Unit>().currentTile;
                        }

                        //check if 1st tile matches 2nd
                        if(T1 != T2)
                        {
                            //tiles do not match, clear set patrol
                            ResetPatrol(true);
                            isPatrolling = true;

                            //pathfind between t1 and t2
                            patrolPath = stdMath.GetTilePath(T1.MapLoc, T2.MapLoc, tileMap, moveCosts, false);

                            //create patrol nodes
                            for (int i = 0; i < patrolPath.Count; ++i)
                            {
                                patrolNodes.Add(GetNewTile(movePath[i]).TileLoc);
                            }

                            //check if unit is currently at T1
                            if (unit.currentTile == T1)
                            {
                                //unit is currently on 1st tile, update patrol
                                UpdatePatrolPath();
                            }
                            else
                            {
                                //move to T1, and patrol from there
                                MoveTo(T1.gameObject.transform);
                            }                            
                            
                        }
                        else
                        {
                            //tiles match, move to tile, reset patrol
                            MoveTo(T1.gameObject.transform);
                            ResetPatrol();
                        }
                    }
                }
                else
                {
                    //check if first tile has been selected
                    if (overlord.selector.IsSelection())
                    {
                        if (overlord.selector.IsTileSelected())
                        {
                            //tile is selcted, set to first tile
                            T1 = overlord.selector.current.GetComponent<Tile>();
                        }else if(overlord.selector.IsEnemySelected() || overlord.selector.IsUnitSelected())
                        {
                            //enemy or unit selected, pull and set tile
                            T1 = overlord.selector.current.GetComponent<Unit>().currentTile;
                        }
                    }
                }
            }
            else
            {
                //exit button pressed, reset patrol
                ResetPatrol();
            }
        }
    }

    private void UpdatePatrolPath()
    {
        //check which end of the patrol path unit is at
        if(unit.tileLocation != patrolPath[0])
        {
            //unit is not at path beginning, reverse patrol path
            patrolPath.Reverse();
            patrolNodes.Reverse();
        }

        //Set movement nodes to patrol nodes
        movePath = patrolPath;
        moveNodes = patrolNodes;
        movePath.RemoveAt(0);
        
        //set pathrenderer
        SetPathRenderer(moveNodes);
        
        //Set init position and move units
        initPos = this.gameObject.transform.position;
        startTime = Time.time;
        Move();
    }

    //Set Move positions for LERP and LOOKAT
    private void SetMovePostions()
    {
        //check number of remaining nodes
        if(moveNodes.Count > 0)
        {
            //Add lerp origin, destination, and set look at target to destination
            movePositions.Add(moveNodes[0]);
            movePositions.Add(moveNodes[1]);
            movePositions.Add(moveNodes[1]);

            //check for look at target
            if (moveNodes.Count > 1)
            {
                //Change look at target to next tile
                movePositions[2] = moveNodes[2];
            }
        }

        //set move start time
        startTime = Time.time;
    }

    //shift path renderer down one
    private void UpdateNodes(List<Vector3> nodes)
    {

        pathRenderer.positionCount = 1;
        pathRenderer.positionCount = nodes.Count + 1;
        pathRenderer.SetPosition(0, this.gameObject.transform.position);

        //cycle nodes list
        for (int i = 0; i < nodes.Count; ++i)
        {

            //set position to node position
            Vector3 v = nodes[i];
            v.y = GetNewTile(movePath[i]).highestY * 1.1f;
            pathRenderer.SetPosition(i + 1, v);
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

	
    

    /* ===================================================================================================================================
	 * 
	 * 									Getters / Setters
	 * 			 				Used to get modified stats
	 *=================================================================================================================================== */

    //Is unit moving
    public bool IsMoving(){
		return isMoving;
	}

    //reset Movement
    public void StopMovement()
    {
        isMoving = false;
        movePositions.Clear();
        movePath.Clear();
        moveNodes.Clear();
    }

    //Reset Patrol
    public void ResetPatrol(bool ignoreT = false)
    {
        isPatrolling = false;
        settingPatrol = false;
        patrolPath.Clear();
        patrolNodes.Clear();
        patrolSet_T1 = false;

        if (!ignoreT)
        {
            T1 = T2 = null;
        }
    }

	//GET CURRENT TILE LOCATION
	public Vector2 CurrentTile(){
		return mapLoc;
	}

	//SET MAP LOCATION FROM TILE//
	public void SetNewTileLocation(Vector2 loc, Tile t){

        //check if location is different
        if (loc != mapLoc)
        {
            //set new location
            mapLoc = loc;
            currTile = t;

            //check if unit is moving
            if (isMoving && movePath.Count > 0)
            {

                //Remove previous tile from path
               // movePath.RemoveAt(0);

            }
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

    //SET UNIT FORMATION//
    public void SetUnitFormation(bool[,] form, int i)
    {
        unitFormation.SetFormation(form, i);
    }
    //GET UNIT FORMATION//
    public bool[,] GetUnitFormation(int i)
    {
        return unitFormation.GetFormation(i);
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
