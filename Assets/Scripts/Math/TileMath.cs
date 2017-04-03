using UnityEngine;
using System.Collections.Generic;

public class TileMath{

	//init Seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}


	//create terrain 		sqrt * (length+1) 
	public Vector3[] CreateTerra(Vector3[] verts, Vector2 start, float[,] noise, int sqrt){
		
		//Remove duplicate vertices
		List<Vector3> v = NonDuplicateVerts (ArrayToList (verts));

		//convert map to single array
		List<float> fA = MapToArray (noise, start, sqrt);

		//cycle original vertices
		for (int i = 0; i < verts.Length; ++i) {

			//chekc for match in original and modified
			if (v.Contains (verts [i])) {

				//if match set new height for the vertex
				verts [i].y = fA [v.IndexOf (verts [i])];
			}
		}

		//return vertices
		return verts;
	}

	//convert array to list
	public List<Vector3> ArrayToList(Vector3[] vertArray){
		List<Vector3> v = new List<Vector3> ();
		for(int i = 0; i< vertArray.Length;++i){
			v.Add (vertArray [i]);
		}
		return v;
	}

	//return sorted list of none duplicate vertices
	public List<Vector3> NonDuplicateVerts ( List<Vector3> verts){

		//List variables
		List<Vector3> vertList = new List<Vector3>();

		// create list to ignore duplicate vertices
		for (int i = 0; i < verts.Count; ++i) {

			//check if list contains vert already or if bottom vert
			if (!(vertList.Contains (verts [i])) && verts [i].y >= 0) {
				vertList.Add (verts [i]);
			}
		}

		//Sort list
		vertList.Sort(new TileCompare());

		//return sorted list
		return vertList;
	}

	//convert noise map to single array
	private List<float> MapToArray( float[,] map, Vector2 start, int sqrt){

		//create return array and end variables
		List<float> fA = new List<float> ();
		int sx = (int)(start.x) + sqrt;
		int sy = (int)(start.y) + sqrt;

		//cycle map from starting point to last vertex
		for (int y = (int)(start.y); y <= sy; ++y) {
			for (int x = (int)(start.x); x <= sx; ++x) {

				//Add to single array
				fA.Add (map [y, x]);
			}
		}

		//return array
		return fA;
	}

	//modify terra map height
	public float[,] ModifyTerra (float[,] terra, Vector2 start, int sqrt, int mtn){


		//get cycle ends
		int sx = (int)(start.x) + sqrt;
		int sy = (int)(start.y) + sqrt;

		//cycle terra and modify heights
		for (int y = (int)(start.y)+1; y <= sy; ++y) {
			for (int x = (int)(start.x)+1; x <= sx; ++x) {

				//set new height
				terra [y, x] = SetHeight (terra [y, x], mtn);

			}
		}

		//return modified map
		return terra;
	}

	//Set Height
	private float SetHeight(float f, int mtn){

		//determine if hill, river, mountain, or sea, or other
		if (mtn == 4) {

			f += Random.Range (-7.5f, 12.5f);

			//return hill height
			return f;
		} else if (mtn == 6) {

			f += Random.Range (-15f, 30f);

			//return mountain height
			return f;
		} else if (mtn == 5) {

			f -= f*Random.Range(1f,4f);

			//return River height
			return f;
		} else if (mtn == 7) {

			f -= f*Random.Range(4f,6f);

			//return Sea height
			return f;
		} else {
			return f;
		}

	}
		
	//randomly set tile location
	public Vector3 SetLocation(List<Vector3> previous, Vector3 tileloc, int width){

		// loop until new coordinate is create or increment is broken
		int i = 0;
		while (i <= 100) {
			
			//generate random x & z coordinates
			float x =  Random.Range (-width+0.5f , width-0.5f);
			float z = Random.Range (-width+0.5f, width-0.5f);

			//create new vector
			Vector3 v = new Vector3 (x, 0f, z);
			++i;
			if(previous.Contains(v)){
				++i;
			}else{
				return v;
			}
		}

		Debug.Log (i);
		//Syntax return
			return new Vector3();
	}
		
	//Generate matrix of tile neighbors
	public Vector3[,] GetNeighbors(int[,] map, Vector2 loc){

		//initialize return matrix
		Vector3[,] ret = new Vector3[3, 3];

		//cycle neighbors
		for (int y = -1; y < 2; ++y) {
			for (int x = -1; x < 2; ++x) {

				//get new coordinates
				int nx = (int)(loc.x + x);
				int ny = (int)(loc.y + y);
				Vector3 v = new Vector3 (-1, -1, -1);

				//check if neighbor is within map bounds
				if (nx >= 0 && ny >= 0 && nx < map.GetLength (1) && ny < map.GetLength (0)) {

					//within bounds set neighbor location and type
					v = new Vector3 (nx, ny, map [ny, nx]);

				}

				//set vector in array
				ret [y + 1, x + 1] = v;
			}
		}

		//return new array
		return ret;
	}


	//Gernerate matrix of vertices
	public Vector3[,] GetVertMat(Vector3[] verts){

		//Remove duplicate vertices
		List<Vector3> v = NonDuplicateVerts (ArrayToList (verts));

		//get squareroot and create new matrix
		int sq = (int)(Mathf.Sqrt (v.Count));
		Vector3[,] ret = new Vector3[sq, sq];
		int i = 0;

		//cycle and add verts to mat
		for (int y = 0; y < sq; ++y) {
			for (int x = 0; x < sq; ++x) {

				ret [y, x] = v [i];
				++i;
			}
		}

		//return 
		return ret;

	}

	//Generate FoW Quad
	public GameObject FOWQuad(float square,
		bool textureFX, int filterMode, int anisotropicLevel, Material mat){

		//create new game object with filtyer and renderer
		GameObject go = new GameObject();
		MeshFilter mf = go.AddComponent<MeshFilter> ();
		MeshRenderer mr = go.AddComponent<MeshRenderer> ();

		//create mesh, vertices, uvs, and tris
		Mesh m = new Mesh();
		Vector3[] v = new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(square,0,0),
			new Vector3(0,0,square),
			new Vector3(square,0,square)
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

		//Adjust settings
		mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		mr.receiveShadows = false;
		mr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
		mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;


		//create texture
		mr.sharedMaterial = mat;
		float matScale = (float)(1f / 256f);
		mr.sharedMaterial.mainTextureScale = new Vector2 (matScale, matScale);

		return go;

	}

	private Texture2D FOWTex(int res, bool textureFX, int filterMode, int anisotropicLevel){

		//initialize texture
		Texture2D texture = new Texture2D(res, res, TextureFormat.RGB24, true);

		//apply if true
		if (textureFX) {

			switch (filterMode) {
			case 0:
				texture.filterMode = FilterMode.Point;
				break;
			case 1:
				texture.filterMode = FilterMode.Bilinear;
				break;
			case 2:
				texture.filterMode = FilterMode.Trilinear;
				break;
			}

			texture.anisoLevel = anisotropicLevel;
		}

		//fill texture with black
		for (int y = 0; y < res; ++y) {
			for (int x = 0; x < res; ++x) {
				if (Random.Range (0f, 1f) > 0.5f) {
					texture.SetPixel (x, y, new Color (0, 0, 0,255));
				} else {
				texture.SetPixel (x, y, new Color (0, 0, 0,0));
				}
			}
		}

		//return texture
		return texture;

	}





}

public class TileCompare : IComparer<Vector3>{

		//compare vector 3s
		public int Compare(Vector3 a, Vector3 b){

		//a= new Vector3 (a.x + 5f, a.y, a.z + 5f);
		//b= new Vector3 (b.x + 5f, b.y, b.z + 5f);

				//check x argument
				if (a.x < b.x) {
					return -1;
				}

				if (Mathf.Approximately (a.x, b.x)) {

					//check z arguement
					if (a.z > b.z) {
						return 1;
					}

					if (Mathf.Approximately (a.z, b.z)) {
						return 0;
					}

					//b greater in z
					return -1;

				}

				//b greater in x
				return 1;

			}




	}


