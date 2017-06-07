using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PanZoom : MonoBehaviour {

	//external classes
	public CameraControls camCon;

	//left pan = (-x,+z)
	// Right pan (+x,-z)
	//forward pan (+x,+z)
	//back pan (-x, -z)
	//scroll forward = (0,1)
	//scroll back (0,-1)

	//pan variables
	public float panSpeed = 0.1f;
	public Vector3 panBy = new Vector3 ();
	private int panX = 0;
	private int panZ = 0;
	private bool dragPan = false;
	private Vector3 previousPos = new Vector3();

	//Zoom Variables
	public float zoomSpeed = 1f;
	public float zoom = 30f;
	public float minZoom = 20f;
	public float maxZoom = 40f;


	//Update function


	/* ===================================================================================================================================
	 * 
	 * 									Pan Functions
	 * 			 				Functions to pan and limit pan of camea
	 *=================================================================================================================================== */

	//Start Pan
	public void StartPan(string s){

		//no pan if dragging
		if (!dragPan) {

			//switch between directions and set panX and panZ
			switch (s) {
			case "forward":
				panX = panZ = 1;
				break;
			case "left":
				panX = -1;
				panZ = 1;
				break;
			case "right":
				panX = 1;
				panZ = -1;
				break;
			case "back": 
				panX = panZ = -1;
				break;
			default:
				panX = panZ = 0;
				break;
			}

			PanVector ();
		}
	}

	//End Pan
	public void EndPan(){
		//set pans to 0
		panX = panZ = 0;
		panBy = new Vector3 ();
		dragPan = false;
	}

	//Drag pan (mouse) TODO: Possibly disable., least m ake less choppy and responsive
	public void DragPan(){

		//if no previous drag set previous pos
		if (previousPos == new Vector3()) {
			previousPos = Input.mousePosition;
		}

		//set dragging to true
		dragPan = true;

		//set Pan x and PanZ
		float x = -(Input.mousePosition.x - previousPos.x);
		float z = -(Input.mousePosition.y - previousPos.y);

		if (x > 0) {
			panX = 1;
		}else if(x<0){
			panX = -1;
		}else{
			panX = 0;
		}

		if (z > 0) {
			panZ = 1;
		}else if(z<0){
			panZ = -1;
		}else{
			panZ = 0;
		}

		//set pan vector
		PanVector();

		//set new previous position
		previousPos = Input.mousePosition;

	}

	//set new pan vector TODO: build up to speed
	private void PanVector(){

		//create new float x and z
		float x = panSpeed * panX;
		float z = panSpeed * panZ;

		//set pan vector
		panBy = new Vector3 (x, 0f, z);
	}

	//jump camer with click
	public void ClickPan(){

		//camCon.SetTarget (camCon.FromScreenToWorld (Input.mousePosition));
	}

	/* ===================================================================================================================================
	 * 
	 * 									Zoom Function
	 * 			 				Functions to set camera zoom
	 *=================================================================================================================================== */

	//set zoom with mouse scroll
	public void SetZoom(){

		//get scroll delta
		float del = Input.mouseScrollDelta.y;

		//get would be zoom
		float z = zoom + (del*(-zoomSpeed));

		//check limits
		int lim = InZoomLimit(z);

		//adjust based on limit value
		if (lim > 0) {
			z = maxZoom;
		} else if (lim < 0) {
			z = minZoom;
		}

		//set zoom
		zoom = z;

	}

	//Zoom within limits
	private int InZoomLimit(float f){

		//check if greater than limit
		if (f <= maxZoom) {

			//less than max limit, check if less than limit
			if (f >= minZoom) {

				//within limit, return 0
				return 0;
			}

			//less than limit, return -1
			return -1;
		}

		//greater than limit return 1
		return 1;
	}
		

	/* ===================================================================================================================================
	 * 
	 * 									Rotate and Pitch 
	 * 			 				Functions to set camera Rotation and pitch
	 *=================================================================================================================================== */

	//Rotate Camera
	//public void RotateCam

}
