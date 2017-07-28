using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOW : MonoBehaviour {

	public Shader cullMask;
	public Shader fogMask;
	public Shader blurShader;
	public Shader alphaMask;

	[Range(0,10)]
	public int blurAmount;

	[Range(0,4)]
	public int downSample;

	private int cullLayer1 = 12;
	private int cullLayer2 = 13;
	private Material blur = null;
	//private Material fowMask = null;
	//private Material alphaMat = null;

	//Initialize
	void Awake(){

		//create materials
		blur = new Material(blurShader);
		//fowMask = new Material (fogMask);
		//alphaMat = new Material (alphaMask);
	}

	//Create camera, render tile and assets to screen in whiteout
	void OnRenderImage(RenderTexture source, RenderTexture destination){

		//Shorthands
		int w = source.width >> downSample;
		int h = source.height >> downSample;

		//Create Temporary render textures
		RenderTexture rt = RenderTexture.GetTemporary(w,h);
		RenderTexture rt2 = RenderTexture.GetTemporary(w,h);

		//create visible tile camera
		Camera cam = createCam();
		cam.name = "FOW Camera - Visible";
		cam.targetTexture = rt;
		cam.cullingMask = 1 << 12;

		//Create visited tile camera
		Camera cam2 = createCam();
		cam2.name = "FOW Camera - Visited";
		cam2.targetTexture = rt2;
		cam2.cullingMask = 1 << 13;

		//render texture
		cam.RenderWithShader(cullMask,"");
		cam2.RenderWithShader(cullMask,"");

		//Create FOW mask
		RenderTexture fow = RenderTexture.GetTemporary(w,h);
		Material fowMask = new Material(fogMask);
		fowMask.SetTexture ("_AddTex", rt2);
		Graphics.Blit (rt, fow, fowMask);
		RenderTexture.ReleaseTemporary(rt);
		RenderTexture.ReleaseTemporary (rt2);

		//Blur FOW mask
		for (int i = 0; i < blurAmount+1; ++i) {

			RenderTexture rTemp = RenderTexture.GetTemporary (w, h);
			Graphics.Blit (fow, rTemp, blur);
			RenderTexture.ReleaseTemporary (fow);
			fow = rTemp;
		}

		//Render FOW mask
		Material fowMat = new Material(alphaMask);
		fowMat.SetTexture ("_AddTex", fow);
		Graphics.Blit (source, destination,fowMat);

		//destroy Cameras
		RenderTexture.ReleaseTemporary(fow);
		Destroy (cam.gameObject);
		Destroy (cam2.gameObject);
	}

	private Camera createCam(){
		Camera cam = new GameObject ().AddComponent<Camera> ();
		cam.transform.parent = this.gameObject.transform.parent;
		cam.transform.position = new Vector3 ();
		cam.CopyFrom (Camera.main);
		cam.enabled = false;
		cam.clearFlags = CameraClearFlags.Color;
		cam.backgroundColor = Color.black;
		return cam;
	}

	private void noeg(RenderTexture source, RenderTexture destination){
		//Set short hands
		int w = source.width;
		int h = source.height;

		//create render texture
//		RenderTexture rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
//		rt.Create ();

		//create camera
//		Camera cam = new GameObject().AddComponent<Camera>();
//		cam.name = "FOW Camera";
//		cam.transform.position = new Vector3 ();
//		cam.transform.parent = this.gameObject.transform.parent;
//		cam.CopyFrom (this.gameObject.GetComponent<Camera> ());
//		cam.enabled = false;

		//Render visible tiles
//		cam.clearFlags = CameraClearFlags.Color;
//		cam.backgroundColor = Color.black;
//		cam.targetTexture = rt;
//		cam.cullingMask = 1 << cullLayer1;
//		cam.RenderWithShader (cullMask, "");

		//render visited tiles
//		cam.clearFlags = CameraClearFlags.Color;
//		cam.backgroundColor = Color.black;
//		RenderTexture rt2 = RenderTexture.GetTemporary (w, h);
//		cam.targetTexture = rt2;
//		cam.cullingMask = 1 << cullLayer2;
//		cam.RenderWithShader (cullMask, "");

		//combine render textures
//		fowMask.SetTexture("_Addtex",rt2);
//		fowMask.SetFloat ("_Intensity", 0.5f);
//		Graphics.Blit(rt, fowMask);
//		RenderTexture.ReleaseTemporary (rt2);

		//blurr fog mask
//		for (int i = 0; i < blurAmount; ++i) {

			//create temporary render texture and blit previous into it with blur
//			RenderTexture rTemp = RenderTexture.GetTemporary(w,h);
//			Graphics.Blit (rt, rTemp, blur);
//			rt = rTemp;
//			RenderTexture.ReleaseTemporary (rTemp);

//		}

		//multiply source by fog mask
//		fowMask.SetTexture("_Addtex",rt);
//		Graphics.Blit (source, destination, fowMask);

//		Destroy (cam.gameObject);


		//Render Textures
		RenderTexture rt1 = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
		RenderTexture rt2 = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
		rt1.Create ();
		rt2.Create ();

		//create image process camera
		Camera cam1 = new GameObject().AddComponent<Camera>();
		Camera cam2 = new GameObject().AddComponent<Camera>();

		cam1.gameObject.name = "Visible Camera";
		cam2.gameObject.name = "Visited Camera";

		cam1.transform.position = new Vector3 ();
		cam2.transform.position = new Vector3 ();

		cam1.transform.parent = this.gameObject.transform.parent;
		cam2.transform.parent = this.gameObject.transform.parent;

		cam1.CopyFrom (this.gameObject.GetComponent<Camera> ());
		cam2.CopyFrom (this.gameObject.GetComponent<Camera> ());

		cam1.enabled = false;
		cam2.enabled = false;

		cam1.clearFlags = CameraClearFlags.Color;
		cam2.clearFlags = CameraClearFlags.Color;

		cam1.backgroundColor = Color.black;
		cam2.backgroundColor = Color.black;

		cam1.targetTexture = rt1;
		cam2.targetTexture = rt2;

		cam1.cullingMask = 1 << cullLayer1;
		cam2.cullingMask = 1 << cullLayer2;

		cam1.RenderWithShader (cullMask,"");
		cam2.RenderWithShader (cullMask,"");

		Material fowMask = new Material (fogMask);
		fowMask.SetTexture("_AddTex",rt2);
		Graphics.Blit(rt1, fowMask);


		//blurr fog mask
		for (int i = 0; i < blurAmount; ++i) {

			//create temporary render texture and blit previous into it with blur
			RenderTexture rTemp = RenderTexture.GetTemporary(w>>downSample,h>>downSample);
			Graphics.Blit (rt1, rTemp, blur);
			rt1 = rTemp;
			//RenderTexture.ReleaseTemporary (rTemp);

		}

		//Graphics.Blit (rt1, blur);
		//Graphics.Blit (rt2, blur);

		//multiply source by fog mask
		Material alphaMat = new Material (alphaMask);
		fowMask.SetTexture("_AddTex",rt1);
		Graphics.Blit (source, destination, fowMask);

//		Destroy (cam.gameObject);

		//blur textures
//		Graphics.Blit (rt1, blur);
//		Graphics.Blit (rt2, blur);

		//Material fowMask = new Material (fogMask);
//		fowMask.SetTexture ("_AddTex", rt2);
//		fowMask.SetTexture ("_Scene", source);
//		Graphics.Blit (rt1, destination, fowMask);

		Destroy (cam1.gameObject);
		Destroy (cam2.gameObject);


	}

}
