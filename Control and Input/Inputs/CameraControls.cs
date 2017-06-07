using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

	//external class
	public PanZoom panZoom;

	//camera settings
	public Camera cam;
	public Vector3 target = new Vector3 (45, 0, 45);
	public float rotationSpeed = 0.01f;
	public bool isRotating = false;


	//private vital variables
	private Vector3 currentPosition = new Vector3 ();
	private Vector3 currentRoatation = new Vector3 ();
	private Quaternion currentQuat = Quaternion.Euler(0f,0f,0f);
	private Rect camArea;
	private int cornerConfig = 0;
	public Vector3[] corners = new Vector3[4];
	private int[,] rotCorners =  new int[,]{
		{ 0, 1, 2, 3 },
		{ 2, 0, 3, 1 },
		{ 3, 2, 1, 0 },
		{ 1, 3, 0, 2 }
	};


	//camera updates
	void Update(){

		//set camera rectangle
		SetArea();

		//set camera zoom
		cam.orthographicSize = panZoom.zoom;

		//pan camera by pan vector
		Pan(PanMod(panZoom.panBy));

		//roata camear
		Rotate();

	}

	//Set Camera Rectable
	private void SetArea(){
		camArea = new Rect (
			cam.pixelWidth * 0.1f,
			cam.pixelHeight * 0.1f,
			cam.pixelWidth * 0.8f,
			cam.pixelHeight * 0.8f
		);
	}

	//Set Camera Corners
	public void SetCorners(Vector3 bottom, Vector3 left,Vector3 right,Vector3 top){
		corners [0] = top;
		corners [1] = left;
		corners [2] = right;
		corners [3] = bottom;
	}


	//Camera Pan Functions
	private void Pan(Vector3 v){
		
		//do not pan if rotating
		if (!isRotating) {
			
			//get current camera position
			Vector3 cur = this.gameObject.transform.position;

			//check for out of bounds corners
			cur += PanLimits(v);

			//set new position
			this.gameObject.transform.position = cur;
			
		}
	}

	//adjust pan for roation
	private Vector3 PanMod(Vector3 v){

		//get y float
		float y = Mathf.Abs(currentQuat.eulerAngles.y);

		//determine rotation
		if (y > 85f && y < 95f) {
			v.z *= -1;
			currentPosition = target + new Vector3 (-80f, 65f, 80f);
			cornerConfig = 1;
		} else if (y > 175f && y < 185f) {
			v.x *= -1;
			v.z *= -1;
			currentPosition = target + new Vector3 (80f, 65f, 80f);
			cornerConfig = 2;
		} else if (y > 265f && y < 275f) {
			v.x *= -1;
			currentPosition = target + new Vector3 (80f, 65f, -80f);
			cornerConfig = 3;
		} else {
			currentPosition = target + new Vector3 (-80f, 65f, -80f);
			cornerConfig = 0;
		}



		return v;
	}

	//Limit Pan
	private Vector3 PanLimits(Vector3 v){

		//set floats and vector
		float x = Mathf.Abs(v.x*3f);
		float z = Mathf.Abs(v.z*3f);
		float y = Mathf.Abs(currentQuat.eulerAngles.y);
		Vector3 lim = new Vector3();

		//find corner out of bounds
		//Top
		if (camArea.yMax >= cam.WorldToScreenPoint (corners [rotCorners[cornerConfig, 0]]).y) {
			lim = new Vector3 (-x, 0f, -z);

			//modify lim based on roation
			if (y > 85f && y < 95f) {
				lim.z *= -1;
			} else if (y > 175f && y < 185f) {
				lim.x *= -1;
				lim.z *= -1;
			} else if (y > 265f && y < 275f) {
				lim.x *= -1;
			}
		}
		//Left
		else if (camArea.xMin <= cam.WorldToScreenPoint (corners [rotCorners[cornerConfig, 1]]).x) {
			lim = new Vector3 (x, 0f, -z);
			//modify lim based on roation
			if (y > 85f && y < 95f) {
				lim.x *= -1;
			} else if (y > 175f && y < 185f) {
				lim.x *= -1;
				lim.z *= -1;
			} else if (y > 265f && y < 275f) {
				lim.z *= -1;
			}
		}
		//Right
		else if (camArea.xMax >= cam.WorldToScreenPoint (corners [rotCorners[cornerConfig, 2]]).x) {
			lim = new Vector3 (-x, 0f, z);

			//modify lim based on roation
			if (y > 85f && y < 95f) {
				lim.x *= -1;
			} else if (y > 175f && y < 185f) {
				lim.x *= -1;
				lim.z *= -1;
			} else if (y > 265f && y < 275f) {
				lim.z *= -1;
			}
		}
		//Bottom
		else if (camArea.yMin <= cam.WorldToScreenPoint (corners [rotCorners[cornerConfig, 3]]).y) {
			lim = new Vector3 (x, 0f, z);

			//modify lim based on roation
			if (y > 85f && y < 95f) {
				lim.z *= -1;
			} else if (y > 175f && y < 185f) {
				lim.x *= -1;
				lim.z *= -1;
			} else if (y > 265f && y < 275f) {
				lim.x *= -1;
			}
		} else {

			// No coordiante out of bounds return increment
			return v;
		}

		//return new vector
		return lim;
	}



	//Camera Rotate functions
	private void Rotate(){

		//check if camera needs rotation
		if(isRotating){

			Quaternion rot = this.gameObject.transform.rotation;
			rot = Quaternion.Slerp (rot, currentQuat, Time.time * rotationSpeed);
			this.gameObject.transform.rotation = rot;


			Vector3 pos = this.gameObject.transform.position;
			//Debug.Log (pos + " | " + currentPosition);
			pos = Vector3.Slerp (pos, currentPosition, Time.time * rotationSpeed);
			this.gameObject.transform.position = pos;

			if (rot == currentQuat) {
				isRotating = false;
			}

		}
	}

	//Set rotation
	public void Rotation(int i){

		//check if already rotation
		if(!isRotating){

			//set rotation to true
			isRotating = true;

			//set next rotation
			float fr = 90*i;
			currentRoatation.y += fr;
			currentQuat = Quaternion.Euler (currentRoatation);
		}
	}



	//Set Camera Target
	public void SetTarget(Vector3 v){
		Debug.Log ("Vector in: " + v);

		target = new Vector3 (v.x, 0f, v.z);

		//variables
		float y = Mathf.Abs(currentQuat.eulerAngles.y);


		//determine rotation
		if (y > 85f && y < 95f) {
			this.gameObject.transform.position = target + new Vector3 (-80f, 65f, 80f);
		} else if (y > 175f && y < 185f) {
			this.gameObject.transform.position = target + new Vector3 (80f, 65f, 80f);
		} else if (y > 265f && y < 275f) {
			this.gameObject.transform.position = target + new Vector3 (80f, 65f, -80f);
		} else {
			this.gameObject.transform.position = target + new Vector3 (-80f, 65f, -80f);
		}

		//this.gameObject.transform.position = v;

	}

	public Vector3 FromScreenToWorld(Vector3 v){
		Debug.Log ("Input in: " + v);
		return cam.ScreenToWorldPoint (v);
	}


}
