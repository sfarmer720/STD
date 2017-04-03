using UnityEngine;
using System.Collections;

public class Inputer : MonoBehaviour {

	//screen variables
	public Camera cam;
	public CameraControls camCon;
	private float sectSize = 0.1f;
	private Rect screen;
	private Rect mainScreen;
	private Rect[] screenSections = new Rect[4];

	//Mouse Variables
	public Vector3 mousePos;
	private bool oneClick = false;
	private bool mousePanning = false;
	private bool mouseInSection = false;
	private int mouseSection = -1;
	private float clickTime;
	private float posTime;
	private float doubleDelay = 0.5f;



	//private vitals


	//Initialize
	void Start(){

		//set screen size & sections
		SetScreen();
	}

	//updates
	void Update(){

		//set current mouse position
		mousePos = Input.mousePosition;

		//check if mouse is in section
		InSect();

		//perform mouse actions
		MouseActions();
	}

	//set screens
	private void SetScreen(){

		//set initial screen
		screen = cam.pixelRect;

		//initialize creation variables
		float sx1 = screen.position.x;
		float sy1 = screen.position.y;
		float sw1 = screen.width;
		float sh1 = screen.height;

		float sw2 = sw1 * sectSize;
		float sh2 = sh1 * sectSize;

		float sx2 = sw1+sx1-sw2;
		float sy2 = sh1+sy1-sh2;

		float sw3 = sw1 - sw2 * 2;
		float sh3 = sh1 - sh2 * 2;

		//set sections
		screenSections [3] = new Rect (sx1,sy1,sw1,sh2);
		screenSections [2] = new Rect (sx1,sy1,sw2,sh1);
		screenSections [1] = new Rect (sx2,sy1,sw2,sh1);
		screenSections [0] = new Rect (sx1,sy2,sw1,sh2);

		//set main screen
		mainScreen = new Rect(sw2,sh2,sw3,sh3);
	}


	//Check for Double click
	public int Clicked(){
		
		//check for mouse input
		if (Input.GetMouseButtonDown (0)) {

			//check if previous click
			if (!oneClick || Time.time - clickTime > doubleDelay) {
				Debug.Log ("click");
				//no previous set true, time, and return 0
				oneClick = true;
				clickTime = Time.time;
				return 0;
			} else {
				Debug.Log ("Double click");
				//double clicked in time, reset click, time, and return 1
				oneClick = false;
				clickTime = 0f;
				return 1;
			}
		}

		//no clicks, return -1
		return -1;
	}

	//Check if mouse is in section
	private void InSect(){
		
		//check if mouse is in main screen
		if (mainScreen.Contains (mousePos)) {
			
			//mouse not in section
			mouseInSection = false;
			mouseSection = -1;
			ResetMouseActions ();
			return;
		} else if (screen.Contains (mousePos)) {
			
			//mouse on game screen, but not in main screen. Check indiv sections
			for (int i = 0; i < 4; ++i) {

				if (screenSections [i].Contains (mousePos)) {
					mouseInSection = true;
					mouseSection = i;
					return;
				}
			}
		} else {

			//mouse not on game screen set to false values
			mouseInSection = false;
			mouseSection = -1;
			ResetMouseActions ();
		}
	}

	//Mouse Actions
	private void MouseActions(){

		//confirm mouse is in section
		if(mouseInSection){
			
			//get current rect
			Rect r = screenSections[mouseSection];

			//check if mouse has hovered in section long enough to pan, or is currently panning
			if( mousePanning || (posTime !=0f && Time.time-posTime > doubleDelay)){
				
				//mouse hovered long enough to pan, or is continueing pan
				posTime = 0f;
				//TODO: pan function
				return;

			}else{
				
				//set time if not already set
				if(posTime<=0f){
					posTime = Time.time;
				}

				//check for double clicks to rotate
				if(Clicked() > 0){
					posTime = Time.time;
					//camCon.RotateCamera (mouseSection, true);
				}
			}
		}
	}

	//reset mouse actions
	private void ResetMouseActions(){
		mousePanning = false;
		posTime = 0f;
	}





}