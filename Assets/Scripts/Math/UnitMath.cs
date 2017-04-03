using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMath{

	//Set Unit Math seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}

	//Convert Transform into Map Location
	public Vector2 TransformToMap(Transform t){

		if (t.gameObject.layer == 12 || t.gameObject.layer == 13) {
			//if selction was tile
			return t.gameObject.GetComponent<Tile> ().MapLoc;
		} else if (t.gameObject.layer == 10) {
			//selection was enemy
			return t.gameObject.GetComponent<Unit> ().Movement ().CurrentTile ();
		}else {
			return new Vector2 (-1, -1);
		}
	}

	//Get Move Vector
	public Vector3 MoveDir( Vector3 forward, float speed, float t){
		return forward * speed * t;
	}


	//Create Sight Plane
	public GameObject CreateSightPlane(int sight, Material mat){
		//create new game object with filtyer and renderer
		GameObject go = new GameObject();
		MeshFilter mf = go.AddComponent<MeshFilter> ();
		MeshRenderer mr = go.AddComponent<MeshRenderer> ();

		//create mesh, vertices, uvs, and tris
		Mesh m = new Mesh();
		Vector3[] v = new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(sight,0,0),
			new Vector3(0,0,sight),
			new Vector3(sight,0,sight)
		};

		Vector2[] uvs = new Vector2[v.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (v [i].x, v [i].z);
		}


		int[] tris = new int[]{0,2,1,2,3,1};

		//assign mesh
		m.vertices = v;
		m.uv = uvs;
		m.triangles = tris;
		mf.mesh = m;
		mr.enabled = true;
		m.RecalculateBounds ();
		m.RecalculateNormals ();

		go.layer = 12;
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;
		mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
		mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		//create texture
		mr.material = mat;
		Texture2D tex = SightTex(sight,sight*2);
		mr.material.mainTexture = tex;
		mr.material.mainTextureScale = new Vector2 ((float)1/sight, (float)1/sight);
		tex.Apply ();

		return go;
	}

	//Create sight texture
	public Texture2D SightTex(float r, int res){

		//Initialize texture
		Texture2D tex = new Texture2D (res, res, TextureFormat.RGBA32, true);
		tex.wrapMode = TextureWrapMode.Clamp;

		//Cycle texture
		for (int y = 0; y < res; ++y) {
			for (int x = 0; x < res; ++x) {
	
				//Check if inside radius
				if (InRadius (r, res*0.5f, res*0.5f, x, y)) {

					//point inside radius, color
					tex.SetPixel(x,y, Color.green);

				} else {
					
					//point inside radius, Transparent
					tex.SetPixel(x,y, new Color(0,0,0,0));
				}
			}
		}
			
		return tex;
	}

	private bool InRadius(float r, float x1, float y1, float x2, float y2){
		return(r > (Mathf.Sqrt (Mathf.Pow ((x2 - x1), 2) + Mathf.Pow ((y2 - y1), 2))));
	}


	//Movement over tile speed
	public float MovementOverTileSpeed(float Speed, float tileCost){
		return Mathf.Clamp01 ((Speed * 0.1f) * (tileCost * 0.1f));
	}

}
