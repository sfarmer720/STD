using UnityEngine;
using System.Collections;

public class prohelp : MonoBehaviour {

	public GenerateNoise gn;

	// Use this for initialization
	void Start () {

		Vector3[] coor = new Vector3[4];
		coor[0] = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		coor[1] = transform.TransformPoint(new Vector3(0.5f,-0.5f));
		coor[2] = transform.TransformPoint(new Vector3(-0.5f,0.5f));
		coor[3] = transform.TransformPoint(new Vector3(0.5f,0.5f));

		Texture2D t = gn.GenTex (coor, true,true, false);
		t.Apply ();

		GetComponent<MeshRenderer> ().material.mainTexture = t;
	}

}
