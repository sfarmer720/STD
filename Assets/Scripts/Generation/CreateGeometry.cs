using UnityEngine;
using System.Collections.Generic;

public class CreateGeometry : MonoBehaviour {

	//set seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}

	//create box
	public GameObject CreateBox(Vector3[] v,float h, Material mat){

		//create return object
		GameObject go = new GameObject("Box");

		//create mesh filter and renderer
		MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
		MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

		//create mesh,vertices, uvs,and tris
		Mesh m = new Mesh();
		Vector3[] verts;

		//Manually create 8 corners
		int sq = (int)Mathf.Sqrt(v.Length);
		verts = new Vector3[]{
			new Vector3(v[0].x,v[0].y,v[0].z),
			new Vector3(v[sq-1].x,v[sq-1].y,v[sq-1].z),
			new Vector3(v[(sq-1)*sq].x,v[(sq-1)*sq].y,v[(sq-1)*sq].z),
			new Vector3(v[sq*sq-1].x,v[sq*sq-1].y,v[sq*sq-1].z),

			new Vector3(v[0].x,v[0].y+h,v[0].z),
			new Vector3(v[sq-1].x,v[sq-1].y+h,v[sq-1].z),
			new Vector3(v[(sq-1)*sq].x,v[(sq-1)*sq].y+h,v[(sq-1)*sq].z),
			new Vector3(v[sq*sq-1].x,v[sq*sq-1].y+h,v[sq*sq-1].z),
		};

		int[] tris = new int[]{
			0,2,1,2,3,1,	//Top
			4,6,5,6,7,5,	//Bottom
			3,1,5,7,3,5,	//Right
			6,4,0,2,6,0,	//Left
			6,2,3,3,7,6,	//Front
			0,4,1,4,5,1		//Back
		};

		//Complete & return object
		m.vertices = verts;
		mf.mesh = m;
		m.uv = GenUV(verts);
		m.triangles = tris;
		mr.enabled = true;
		m.RecalculateBounds();
		m.RecalculateNormals();
		mr.material = mat;

		return go;
	}

	//Create plane
	public GameObject CreatePlane(float x, float y, float square, int subdivide,
		float yMod, float[,] texture, Material mat, int MntHill){

		//create returning game object
		GameObject go = new GameObject("Plane");

		//create mesh filter and renderer
		MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
		MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;


		//create mesh,vertices, uvs,and tris
		Mesh m = new Mesh();
		Vector3[] verts;

		//create vertices
		if(subdivide>0){

			//create sub divide float
			float s= subdivide*2;
			float sub = square/s;

			//create vertex list
			List<Vector3> vlist = new List<Vector3>();

			//cycle x & y
			for(float i=0f; i <= square;){
				for(float j=0f; j <= square;){

					//modede coordinates
					float mx = square*x;
					float my = square * y;

					//create coordinates
					float vx = i+mx;
					float vz = j+my;
					float vy = texture [(int)vx, (int)vz];
					//apply ymod base don if mountain or hill
					if (MntHill>0) {

						//establish always negative coordinates
						float hs = square/2;
						float hx = -(Mathf.Abs(j-hs));
						float hy = -(Mathf.Abs(i-hs));

						//get difference between coordinates
						float hd = hx-hy;

						//find increment
						float hm = ((hx+hs)*(hy+hs))/2;
						float hi= hx-(hy+hd)+hm;

						//increment height
						if (MntHill == 1) {
							vy = vy * (yMod + Random.Range (0f, hm * 0.45f));
						} else {
							vy = vy * (yMod + Random.Range (hm * 0.5f, hm * 1.25f));
						}

					} else {
						vy = vy*yMod;
					}

					//add new vertex
					vlist.Add(new Vector3(vx,vy,vz));
					//increment by sub
					j+=sub;
				}
				i+=sub;
			}

			//convert list to array
			verts = vlist.ToArray();

		}else{

			//if no subdivisions create 4 corner points
			verts = new Vector3[]{
				new Vector3(x*square,0f,y*square),
				new Vector3(x*square+square,0f,y*square),
				new Vector3(x*square,0f,y*square+square),
				new Vector3(x*square+square,0f,y*square+square)
			};
		}

		//Complete & return object
		m.vertices = verts;
		mf.mesh = m;
		m.uv = GenUV(verts);
		m.triangles = GenTri(subdivide);
		mr.enabled = true;
		m.RecalculateBounds();
		m.RecalculateNormals();
		mr.material = mat;

		return go;

	} 

	//Set UV's
	private Vector2[] GenUV(Vector3[] verts){
		Vector2[] uvs = new Vector2[verts.Length];
		for (int i = 0; i < uvs.Length; i++) {
			uvs [i] = new Vector2 (verts [i].x, verts [i].z);
		}
		return uvs;
	}

	//Set tris
	private int[] GenTri(int size){

		int[] tris;

		//check if subdivided
		if(size>0){
			
			size = size*2;
			tris = new int[size*size*6];

			for (int ti = 0, vi = 0, ty = 0; ty < size; ty++, vi++) {
				for (int tx = 0; tx < size; tx++, ti += 6, vi++) {
					tris[ti] = vi;
					tris[ti + 4] = tris[ti + 1] = vi + 1;
					tris[ti + 3] = tris[ti + 2] = vi + size + 1;
					tris[ti + 5] = vi + size + 2;
				}
			}
		}else{
			tris = new int[]{0,2,1,2,3,1};
		}
		return tris;
	}
}
