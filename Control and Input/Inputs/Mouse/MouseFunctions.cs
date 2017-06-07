using UnityEngine;
using System.Collections;

public class MouseFunctions : MonoBehaviour {

	//External Varaiables
	private STDMath stdMath;
	public Camera cam;
	public GameObject CustomCursor;

	//mouse variables
	public float mouseMoveTolerance = 5f;
	public Vector3 mousePosition = new Vector3 ();
	private Vector3 previousPosition = new Vector3();
	private bool mouseMoving = false;

	//click variables
	public Vector3 clickPosition = new Vector3();
	private Vector3 downPosition = new Vector3();
	public int clickType = 0;
	private float doubleDelay = 0.5f;
	private float clickTime;
	private bool oneClick = false;
	private bool doubleClick = false;
	//private bool rightClick = false;

	//drag variables
	public Vector3 dragBy = new Vector3();

	//selection variables
	public SelectionScript selection;
	public bool activeSelection = true;
	public Transform currSelection;
	public Transform prevSelection;
	public Transform passSelection;
	private int mask = 0;	//System.Convert.ToInt32("01101000000",2);

	//initialize
	void Start(){

		//initlize seleciton
		//selection = this.gameObject.AddComponent (typeof(SelectionScript)) as SelectionScript;

		//create selection mask
		int[] masks = {	10, 11, 12, 13 };

		//combine masks
		for (int i = 0; i < masks.Length; ++i) {
			int m = 1 << masks [i];
			mask = mask | m;
		}

		//selection.InitSelection (mask, cam);

		//set stdMath
		stdMath = cam.transform.parent.parent.GetComponent<STDMath>() as STDMath;


		//Set up custom cursor
		//Cursor.visible = false;

	}

	// Update is called once per frame
	void FixedUpdate () {

		//track custom cursor to Mouse position
		UpdateCursor();

		//Check mouse movements
		MouseMoved();

		//check for clicks
		MouseClicks();

		//check for drag TODO: Circular closed logic?
		MouseDrag();


	
	}

	private void UpdateCursor(){

		//move quad to mouse position
		//CustomCursor.transform.position = Input.mousePosition;
	}

	//Move Movement
	private void MouseMoved(){

		//check if mouse has moved
		if (mousePosition != Input.mousePosition) {

			//update mouse position
			previousPosition = mousePosition;
			mousePosition = Input.mousePosition;

		}
	}

	//Mouse clicks
	private void MouseClicks(){

		//set down position of click
		if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (0)) {
			downPosition = Input.mousePosition;
		}


		//confirm left click event in area of down (events occur on up not down) 
		if (Input.GetMouseButtonUp (0) && stdMath.mouseTol(downPosition,mousePosition,mouseMoveTolerance)) {


			//check if previous click
			if (!oneClick || doubleClick || Time.time - clickTime > doubleDelay) {

				//no previous clicks
				oneClick = true;
				doubleClick = false;
				//rightClick = false;
				clickTime = Time.time;
				clickType = 0;

				//perform one click functions
				OneClickFunction ();

			} else{

				//previous click within time
				oneClick = false;
				//rightClick = false;
				doubleClick = true;
				clickType = 1;

				//perform double click functions
				DoubleClickFunction ();
			}

			//set clicked coordinates
			clickPosition = mousePosition;

			//reset drag
			ResetMouseDrag();

		}else if(Input.GetMouseButtonUp(1) && stdMath.mouseTol(downPosition,mousePosition,mouseMoveTolerance)){

			//right click selections
			oneClick = false;
			doubleClick = false;
			//rightClick = true;
			clickType = 2;

			//perform right click functions
			RightClickFunction ();
		}
	
	}

	//Click functions
	private void OneClickFunction(){

		Debug.Log ("Perform One CLick");

		//Perform mouse selection
		//Selection(Input.mousePosition);
		selection.SelectionCast(Input.mousePosition);
		//selection.SelectionCast(Input.mousePosition,activeSelection);

	}

	//Click functions
	private void DoubleClickFunction(){
		Debug.Log ("Perform Double CLick");

		//Confirm not null
		if (selection.current != null) {

			//check if unit
			if (selection.isUnit(selection.current.gameObject)) {
				activeSelection = false;
			}
		}
	}

	//Right click functions
	private void RightClickFunction(){
		Debug.Log ("Perform Right CLick");

		//Perform mouse selection
		//Selection(Input.mousePosition);
	}

	//Mouse Drag
	private void MouseDrag(){

		//check that mouse is held and moving
		if (mouseMoving && Input.GetMouseButton (0)) {

			//get the difference between previous and current mouse positions
			dragBy = mousePosition - previousPosition;
			Debug.Log (dragBy);
		}
	}
		
	//Reset Drag
	private void ResetMouseDrag(){
		dragBy = new Vector3 ();
	}


}
