using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldDriver : MonoBehaviour {

	public float test;

	//driver variables
	private int sideMod;
	private bool isOpen = false;
	private Vector3 currPos;
	private Vector3[] positions = new Vector3[2];
	private float animationSpeed = 1.5f;
	private float animTime = 0f;


	//ShortHands
	public float maxWidth;
	public float minWidth;
	private RectTransform RT;
	private RectTransform parentRT;

	//Initialize
	void Awake(){

		//set shorthands
		RT = this.gameObject.GetComponent<RectTransform>();
		parentRT = this.gameObject.transform.parent.gameObject.GetComponent<RectTransform> ();

		//set side mod
		sideMod = (RT.localPosition.x >0) ? 1:-1;

		//set widths and positions
		SetWidths();
		SetPositions ();

		//set defual position
		currPos = positions[0];
		RT.localPosition = currPos;
	}

	//Update method
	void Update(){

		//constantly set widths and positions
		SetWidths();
		SetPositions ();

		//check if in position
		if (!InPosition ()) {

			//animate towards destination
			RT.localPosition = FramePostion (currPos, Position ());
		} else {

			animTime = 0f;
		}

		currPos = RT.localPosition;
	}


	//Set max and min widths
	private void SetWidths(){
		minWidth = RT.rect.width / 2;
		maxWidth = (Screen.width / 2) - minWidth;
	}

	//set positions based on screen size
	private void SetPositions(){
		positions [0] = new Vector3 (minWidth * sideMod, 0, 0);
		positions [1] = new Vector3 ((Screen.width/2 - minWidth )  * sideMod, 0, 0);
	}

	//Set open or closed
	public void OpenClose(){
		isOpen = !isOpen;
	}

	//get intended position
	private Vector3 Position(){
		int i = (isOpen) ? 1 : 0;
		return positions[i];
	}

	//check if current position matches set position
	private bool InPosition(){
		return (currPos == Position ());
	}

	private Vector3 FramePostion(Vector3 curr, Vector3 dest){

		//check time
		if (animTime <= animationSpeed) {
			
			animTime += Time.deltaTime;
			curr.x = Mathf.Lerp (curr.x, dest.x, GetAnimLerp());
			return curr;

		} else {

			animTime = 0f;
			return dest;
		}
	}

	//get current position
	public Vector3 GetPos(){
		return currPos;
	}

	//get animation time
	public float GetAnimTime(){
		return animTime;
	}

	//get animation speed
	public float GetAnimationSpeed(){
		return animationSpeed;
	}

	public float GetAnimLerp(){
		return animTime / animationSpeed;
	}

}
