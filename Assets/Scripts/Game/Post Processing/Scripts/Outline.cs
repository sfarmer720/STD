using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour {
	public int cullLayer;
	public GameObject target;
	public Shader cullMask;
	public Shader outlineShader;
	public Shader blurShader;
	public Color outlineColor;

	[Range(0,10)]
	public int outlineSpread;

	[Range(0,10)]
	public int outlineWidth;

	private Material blur;

	void Awake(){
		blur = new Material (blurShader);
	}


	//Set Selected layers
	private void SetSelected(){

		//check if target exists
		if (target != null) {

			//find all selected tiles
			Tile[] children;
			children = target.GetComponentsInChildren<Tile> ();

			//cycle all children
			foreach (Tile child in children) {

				//check if child is selected
				if (child.isSelected) {

					//Duplicate tile
					GameObject go = Instantiate(child.gameObject, child.transform.parent);
					go.layer = cullLayer;

					//remove children
					Transform[] children2;
					children2 = go.GetComponentsInChildren<Transform> ();
					foreach (Transform child2 in children2) {
						Destroy (child2.gameObject);
					}


					//set tile based on render state
					//child.gameObject.layer = 31;

				}
			}
		}
	}

	//Remove duplicated objects
	private void DeleteDups(){

		if (target != null) {
			Tile[] children;
			children = target.GetComponentsInChildren<Tile> ();

			//cycle all children
			foreach (Tile child in children) {

				//check if child is selected
				if (child.gameObject.layer == cullLayer) {
					Destroy (child.gameObject);
				}
			}
		}
	}


	//Create Camera
	private Camera CreateCam(string name, int cull){
		Camera cam = new GameObject ().AddComponent<Camera> ();
		cam.gameObject.name = name;
		cam.transform.parent = this.gameObject.transform.parent;
		cam.transform.position = new Vector3 ();
		cam.CopyFrom (this.gameObject.GetComponent<Camera> ());
		cam.enabled = false;
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.black;
		cam.cullingMask = 1 << cull;
		return cam;
	}


	//Outline selected objects
	void OnRenderImage(RenderTexture source, RenderTexture destination){

		//Shorthands
		int w = source.width;
		int h = source.height;

		//check for tiles
		SetSelected();

		//Create render texture
		RenderTexture tOut = RenderTexture.GetTemporary(w,h);
		RenderTexture tOutBase = RenderTexture.GetTemporary(w,h);

		//Create Camera
		Camera tCam = CreateCam("Outline Camera "+cullLayer, cullLayer);
		tCam.targetTexture = tOutBase;
		tCam.RenderWithShader (cullMask, "");
		Graphics.Blit (tOutBase, tOut);

		for (int i = 0; i < outlineSpread + 1; ++i) {
			RenderTexture rTemp = RenderTexture.GetTemporary (w, h);
			Graphics.Blit (tOut, rTemp, blur);
			RenderTexture.ReleaseTemporary (tOut);
			tOut = rTemp;
		}

		//Add outline to Source
		Material mat = new Material(outlineShader);
		mat.SetColor ("_Color", outlineColor);
		mat.SetTexture ("_Scene", source);
		mat.SetTexture ("_Mask", tOutBase);
		mat.SetFloat ("_Width", Mathf.NextPowerOfTwo(outlineWidth));
		Graphics.Blit (tOut, destination, mat);

		//GC
		Destroy(tCam.gameObject);
		DeleteDups ();
		RenderTexture.ReleaseTemporary (tOut);
		RenderTexture.ReleaseTemporary (tOutBase);


	}

	void brtaj (RenderTexture source, RenderTexture destination){

		//Set short hands
		int w = source.width;
		int h = source.height;

		//Render Textures
		RenderTexture rt = new RenderTexture(w, h, 0, RenderTextureFormat.R8);
		rt.Create ();

		//create image process camera
		Camera cam = new GameObject().AddComponent<Camera>();
		cam.gameObject.name = "Outline Cam " + cullLayer;
		cam.transform.position = new Vector3 ();
		cam.transform.parent = this.gameObject.transform.parent;
		cam.enabled = false;
		cam.CopyFrom (this.gameObject.GetComponent<Camera> ());
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.black;
		cam.targetTexture = rt;
		cam.cullingMask = 1 << cullLayer;
		cam.RenderWithShader (cullMask,"");

		//create new material
		Material mat = new Material(outlineShader);
		mat.SetColor ("_Color", outlineColor);
		mat.SetTexture ("_Scene", source);

		//Blit to destination
		Graphics.Blit(rt,destination,mat);
		Destroy (cam.gameObject);
	}

}
