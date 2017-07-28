using UnityEngine;
using System.Collections;

public class WorldCamX : MonoBehaviour {

	public Camera worldCam;

	public Vector3 position;
	public float zPos;
	public float panSpeed;
	public float xPanDistance;
	public float yPanDistance;

	public float zoom;
	public float minZoom;
	public float maxZoom;
	public float zoomSpeed;

	private MapGeneratorX mapGen;
	private STDMathX stdmath;
	private float tileWidth;

	private int mapSize;
	private Bounds mapBounds;
	private Vector2 centerPos;
	private Vector2 bottomPos;
	private Vector2 rightPos;
	private Vector2 leftPos;
	private Vector2 topPos;


	// Use this for initialization
	void Start () {
	
		//pass and set variables
		stdmath = GameObject.Find("Scripter").GetComponent(typeof(STDMathX)) as STDMathX;
		mapGen = GameObject.Find ("Scripter").GetComponent (typeof(MapGeneratorX)) as MapGeneratorX;
		Random.InitState (stdmath.seed);


		//set camera positions
		mapSize = mapGen.mapSize;
		mapBounds = mapGen.GetMapBounds();
		centerPos = new Vector2 (mapBounds.center.x, mapBounds.center.y);
		bottomPos = new Vector2 ();
		rightPos = new Vector2 ((mapBounds.size.x / 2), mapBounds.center.y);
		leftPos = new Vector2 ((mapBounds.size.x / -(2)), mapBounds.center.y);
		topPos = new Vector2 (mapBounds.center.x, mapBounds.size.y);

		Debug.Log ("center: " + centerPos);
		Debug.Log ("Bottom: " + bottomPos);
		Debug.Log ("right: " + rightPos);
		Debug.Log ("Left: " + leftPos);
		Debug.Log ("Top: " + topPos);
		//set camera to initial position
		SetCameraPosition(centerPos);
		SetCameraZoom (zoom);

	}
	
	// Update is called once per frame
	void Update () {
	
		//Debug.Log (Input.mouseScrollDelta);
		MouseCameraPan (Input.mousePosition);
		MouseCameraZoom (Input.mouseScrollDelta.y);
	}


	//Set camera to position
	public void SetCameraPosition(Vector2 pos){

		//convert vector2 to vector 3
		Vector3 v = new Vector3 (pos.x, pos.y, zPos);

		//check if new camera point is within map diamond
		if (CameraPositionLimit (pos)) {
			
			//set transform to new position
			gameObject.transform.position = v;
			position = v;

		}
	}

	//set camera zoom
	public void SetCameraZoom(float f){

		//confirm zoom is within range
		if (CameraZoomLimit (f)) {

			//set zoom value
			worldCam.orthographicSize = f;
			zoom = f;
		}
	}

	//limit camera pan
	private bool CameraPositionLimit(Vector3 pos){

		//check if the new coordinates fit within the cartesian square
		//return (stdmath.IsInRectangle (new Vector2(pos.x,pos.y), bottomPos, rightPos,leftPos,topPos));
		return stdmath.IsInDiamond (new Vector2 (pos.x, pos.y), topPos, rightPos, leftPos, bottomPos);

	}

	//limit camera zoom
	private bool CameraZoomLimit(float f){
		return (f >= minZoom && f <= maxZoom);
	}

	//Pan camera with mouse
	public void MouseCameraPan(Vector3 pos){

		//check if mouse position is near screen edge
		if (pos.x < xPanDistance || pos.x > worldCam.pixelWidth - xPanDistance || pos.y < yPanDistance || pos.y > worldCam.pixelHeight - yPanDistance) {

			//initialize movement vector
			float x = 0;
			float y = 0;

			//check for x pan
			if (pos.x < xPanDistance) {

				//pan camera to the left
				x -=panSpeed;

			} else if (pos.x > worldCam.pixelWidth - xPanDistance) {

				//pan camera to the right
				x +=panSpeed;
			}

			//check for y pan
			if (pos.y < yPanDistance) {

				//pan camera to the left
				y -=panSpeed;

			} else if (pos.y > worldCam.pixelHeight - yPanDistance) {

				//pan camera to the right
				y +=panSpeed;
			}

			//set new camera position
			Vector2 move = new Vector2 (position.x + x, position.y + y);
			SetCameraPosition (move);
		}
	}

	//zoom camera with mouse wheel
	public void MouseCameraZoom(float f){
		float z = zoom + f;
		SetCameraZoom (z);
	}
}
