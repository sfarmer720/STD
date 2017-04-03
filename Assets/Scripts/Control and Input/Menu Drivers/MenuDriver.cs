using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDriver : MonoBehaviour {

	public float localF;
	public Vector3 localPos;
	public Vector3 localScale;

	//Shorthands
	private RectTransform RT;
	private RectTransform parentRT;
	private ShieldDriver[] shield = new ShieldDriver[2];
	private Vector3 shieldPos;

	//vital variables
	public float maxWidth;
	public bool fullOpen = false;
	private Vector3 currPos;
	private Vector3 currScale;
	private Vector3[] positions = new Vector3[2];
	private Vector3[] scales = new Vector3[2];

	//initialize
	void Awake(){

		//set shorthands
		RT = this.gameObject.GetComponent<RectTransform>();
		parentRT = this.transform.parent.gameObject.GetComponent<RectTransform> ();
		shield [0] = GameObject.Find ("Shield - Left").GetComponent<ShieldDriver> ();
		shield [1] = GameObject.Find ("Shield - Right").GetComponent<ShieldDriver> ();

		//set default position
		SetPosAndScale();
		parentRT.localScale = currScale = scales [0];
		parentRT.localPosition = currPos = positions [0];

	}

	//Update method
	void FixedUpdate(){

		//set max width
		SetMaxWidth();
		ScaleBackground ();
		ScaleMenu ();

		currPos = parentRT.localPosition;
		currScale = parentRT.localScale;
		localScale = Scale ();
		localPos = Position();
	}


	//Set full open
	public void FullOpen(){
		fullOpen = !fullOpen;
	}

	//Set max width
	private void SetMaxWidth(){
		maxWidth = Screen.width / 2;
	}

	//Scale background
	private void ScaleBackground(){
		shieldPos = shield [0].GetPos ();
		shieldPos.x += maxWidth - shield [0].minWidth;
		RT.offsetMin = new Vector2 (shieldPos.x, MenuBottom());
		RT.offsetMax = new Vector2 (-shieldPos.x, 0);
	}

	//scale menu bottom
	private float MenuBottom(){
		float dst = (fullOpen) ? 0 : 240;
		float curr = RT.offsetMin.y;
		return FloatPosition (curr, dst);
	}


	//Scale menu
	private void ScaleMenu(){

		//set positions and scales
		SetPosAndScale();

		//check if in position
		if (!InPosition ()) {
			parentRT.localPosition = FramePosition (currPos, Position ());
			parentRT.localScale = FramePosition (currScale, Scale ());

		}
	}

	//set positions and scales
	private void SetPosAndScale(){
		positions [1] = new Vector3 (0, 0, 0);
		positions [0] = new Vector3 (0, -Screen.height/2, 0);
		scales [0] = new Vector3 (0.7f, 0.5f, 0.5f);
		scales [1] = new Vector3 (1f, 1f, 1f);
	}

	//intended position
	private Vector3 Position(){
		int i = (fullOpen) ? 1 : 0;
		return positions [i];
	}

	//intended scale
	private Vector3 Scale(){
		int i = (fullOpen) ? 1 : 0;
		return scales [i];
	}

	//check if in position
	private bool InPosition(){
		return (currPos == Position() && currScale == Scale ());
	}

	//set lerped vectors for animation
	private Vector3 FramePosition(Vector3 curr, Vector3 dest){

		//check time
		if (shield [0].GetAnimTime() <= shield [0].GetAnimationSpeed ()) {
			return Vector3.Lerp (curr, dest, shield [0].GetAnimLerp ());
		} else {
			return dest;
		}
	}

	private float FloatPosition ( float f1, float f2){
		if (shield [0].GetAnimTime() <= shield [0].GetAnimationSpeed ()) {
			return Mathf.Lerp (f1, f2, shield [0].GetAnimLerp ());
		} else {
			return f2;
		}
	}

}
