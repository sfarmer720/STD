using UnityEngine;
using System.Collections.Generic;

public class GenerateNoise : MonoBehaviour {

	//critical varibles
	public int seed;
	private bool seeded = false;

	//texture variables
	public int resolution = 256;
	public bool miniMapping = true;
	public bool clamp = true;
	public enum Filter{ None, Point, Bilinear, Trilinear};
	public Filter filter;

	[Range(1,9)]
	public int anisotropicLevel = 9;

	//Noise variables
	public enum NoiseType {Value, Perlin}
	public NoiseType noiseType;
	public bool randomMix = false;
	public int hashSize = 255;
	public float frequency=32f;
	[Range(1,9)]
	public int octaves = 3;
	[Range(1f,4f)]
	public float lacunarity = 2f;
	[Range(0f,1f)]
	public float persistance = 0.5f;
	[Range(1,3)]
	public int dimensions = 3;
	public bool interpolateNoise = true;


	//private variables
	private int[] nHash;

	private float sqr = Mathf.Sqrt(2f);
	private float[] grad1 = {-1f,1f};
	private Vector2[] grad2 = {
		new Vector2 ( 1f,  0f),
		new Vector2 (-1f,  0f),
		new Vector2 ( 0f,  1f),
		new Vector2 ( 0f, -1f),
		new Vector2 ( 1f,  1f).normalized,
		new Vector2 (-1f,  1f).normalized,
		new Vector2 ( 1f, -1f).normalized,
		new Vector2 (-1f, -1f).normalized,
		new Vector2 (-1f, -1f).normalized
	};

	private Vector3[] grad3 = {
		new Vector3 ( 1f,  1f,  0f),
		new Vector3 (-1f,  1f,  0f),
		new Vector3 ( 1f, -1f,  0f),
		new Vector3 (-1f, -1f,  0f),
		new Vector3 ( 1f,  0f,  1f),
		new Vector3 (-1f,  0f,  1f),
		new Vector3 ( 1f,  0f, -1f),
		new Vector3 (-1f,  0f, -1f),
		new Vector3 ( 0f,  1f,  1f),
		new Vector3 ( 0f, -1f,  1f),
		new Vector3 ( 0f,  1f, -1f),
		new Vector3 ( 0f, -1f, -1f),

		new Vector3 (1f, 1f, 0f),
		new Vector3 (-1f, 1f, 0f),
		new Vector3 (0f, -1f, 1f),
		new Vector3 (0f, -1f, -1f)

	};

	//Set Random Seed
	public void Reset(){
		resolution = 256;
		miniMapping = true;
		clamp = true;
		filter = Filter.None;
		noiseType = NoiseType.Perlin;
		randomMix = false;
		hashSize = 255;
		frequency = 32f;
		octaves = 3;
		lacunarity = 2;
		persistance = 0.5f;
		dimensions = 3;
		interpolateNoise = true;
	}



	private void SetSeed(){
		if(!seeded && seed > -1 ){
			Random.InitState (seed);
			seeded = true;
		}
	}

	//generate texture array
	public float[,] GenArray(Vector3[] coor){
		return FillTexture (coor);
	}

	public List<float[,]> GenColorArray(Vector3[] coor, bool alpha){

		float[,] r = GenArray (coor);
		float[,] g = GenArray (coor);
		float[,] b = GenArray (coor);

		List<float[,]> rgb = new List<float[,]> ();
		rgb.Add (r);
		rgb.Add (g);
		rgb.Add (b);

		if(alpha){
			float[,] a = FillTexture (coor);
			rgb.Add (a);
		}

		return rgb;
	}
		

	//Generate Texture
	public Texture2D GenTex(Vector3[] coor, bool textureFX, bool rgb, bool alpha){

		//initialize return texture
		Texture2D texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, miniMapping);

		//apply if true
		if (textureFX) {
			
			//set texture variables
			texture.wrapMode = (clamp) ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;

			switch (filter) {
			case Filter.Point:
				texture.filterMode = FilterMode.Point;
				break;
			case Filter.Bilinear:
				texture.filterMode = FilterMode.Bilinear;
				break;
			case Filter.Trilinear:
				texture.filterMode = FilterMode.Trilinear;
				break;
			}

			texture.anisoLevel = anisotropicLevel;
		}

		//fill texture
		texture = ColorTexture(texture, coor, rgb, alpha);

		//return texture
		return texture;
	}

	private Texture2D ColorTexture(Texture2D texture, Vector3[] coor, bool rgb, bool alpha){

		//initiate arrays
		float[,] r,g,b,a;
		r = g = b = a = GenArray (coor);

		//generate arrays
		if (rgb) {
			g = GenArray (coor);
			b = GenArray (coor);

			if (alpha) {
				a = GenArray (coor);
			}
		}


		//cycle texture
		for(int y = 0; y<resolution; ++y){
			for(int x = 0; x<resolution;++x){

				//set texture
				if (rgb) {
					if (alpha) {
						texture.SetPixel (x, y, new Color (r [y, x], g [y, x], b [y, x], a [y, x]));
					} else {
						texture.SetPixel (x, y, new Color (r [y, x], g [y, x], b [y, x]));
					}
				} else {
					texture.SetPixel (x, y, Color.white * r [y, x]);
				}
			}
		}

		//return colored texture
		return texture;
	}

	private float[,] FillTexture(Vector3[] coor){

		//set seed
		SetSeed();

		//initialize float array
		float[,] fa = new float[resolution,resolution];

		//generate hash
		nHash = GenHash(hashSize+1);

		//initialize step size
		float step = 1f/resolution;

		//cycle texture
		for (int y = 0; y < resolution; ++y) {

			//interpolate coordinates
			Vector3 p0 = Vector3.Lerp(coor[0],coor[2],(y+0.5f)*step);
			Vector3 p1 = Vector3.Lerp(coor[1],coor[3],(y+0.5f)*step);

			for (int x = 0; x < resolution; ++x) {

				//generate noise point
				Vector3 p = Vector3.Lerp(p0,p1,(x+0.5f)*step);

				//generate flaot value, account for perlin or value
				float f = NoiseValue(p,frequency);

				//set texture color
				fa[y,x] = f;

			}
		}

		//return float array
		return fa;
	}

	//get Noise value
	private float NoiseValue(Vector3 p, float freq){

		//initial noise
		float n = NoiseSwitch(p,freq);
		float amp = 1f;
		float range = 1f;

		//cycle octaves
		for (int i = 1; i < octaves; ++i) {

			//modify frequency, amp, and range
			freq *= lacunarity;
			amp *= persistance;
			range += amp;

			//add new noise ontop of previous
			n += NoiseSwitch (p,freq) * amp;
		}

		//return final value
		return n / range;
	}

	//Noise Value Switch
	private float NoiseSwitch(Vector3 p, float freq){
		if (!randomMix) {
			switch (noiseType) {
			case NoiseType.Value:
				return ValueNoise (p,freq);

			case NoiseType.Perlin:
				return PerlinNoise (p,freq)*0.5f+0.5f;
			}
		} else {
			if (Random.Range (0, 9) < 5) {
				return PerlinNoise (p,freq)*0.5f+0.5f;
			} else {
				return ValueNoise (p,freq);
			}
		}

		//syntax return
		return Random.Range(0f,1f);
	}

	//Value Noise Generator
	private float ValueNoise(Vector3 p, float freq){
		//initialize values
		int hx0, hx1, hy0, hy1, hy2, hy3, hz0, hz1, hz2, hz3, hz4, hz5, hz6, hz7;
		hx0 = hx1 = hy0 = hy1 = hy2 = hy3 = hz0 = hz1 = hz2 = hz3 = hz4 = hz5 = hz6 = hz7 = 0;

		//multiply point by frequency
		p*=freq;

		//floor vectors
		int x = Mathf.FloorToInt( p.x);
		int y = Mathf.FloorToInt( p.y); 
		int z = Mathf.FloorToInt( p.z);

		//assign interpolation floats
		float fx = SmoothingCurve(p.x-x);
		float fy = SmoothingCurve(p.y-y);
		float fz = SmoothingCurve(p.z-z);

		//bitwise vectors
		x &= hashSize;
		y &= hashSize;
		z &= hashSize;

		//initialize hash, vector, & gradient values
			hx0 = nHash [x];
			hx1 = nHash [x + 1];

		if (dimensions > 1) {
			hy0 = nHash [hx0 + y];
			hy1 = nHash [hx1 + y];
			hy2 = nHash [hx0 + y +1];
			hy3 = nHash [hx1 + y +1];
		}

		if (dimensions > 2) {
			hz0 = nHash [hy0 + z];
			hz1 = nHash [hy1 + z];
			hz2 = nHash [hy2 + z];
			hz3 = nHash [hy3 + z];
			hz4 = nHash [hy0 + z + 1];
			hz5 = nHash [hy1 + z + 1];
			hz6 = nHash [hy2 + z + 1];
			hz7 = nHash [hy3 + z + 1];
		}


		//switch based on return deminsion
		switch(dimensions){
		case 1: 
			if(interpolateNoise){
				return Mathf.Lerp (hx0, hx1, fx);
			}else{
				return nHash[x]*(1f/hashSize);
			}

		case 2: 
			if(interpolateNoise){
				return Mathf.Lerp (
					Mathf.Lerp (hy0, hy1, fx),
					Mathf.Lerp (hy2, hy3, fx),
					fy) * (1f / hashSize);
			}else{
				return nHash[(nHash[x]+y)&hashSize]*(1f/hashSize);
			} 

		case 3: 
			
			if(interpolateNoise){
				return Mathf.Lerp (
					Mathf.Lerp (Mathf.Lerp (hz0, hz1, fx), Mathf.Lerp (hz2, hz3, fx), fy),
					Mathf.Lerp (Mathf.Lerp (hz4, hz5, fx), Mathf.Lerp (hz6, hz7, fx), fy),
					fz) * (1f / hashSize);
			}else{
				return nHash[(nHash[(nHash[x]+y)&hashSize]+z)&hashSize]*(1f/hashSize);
			} 
		}

		//syntax return random
		return Random.Range (0f, 1f);
	}

	//Perlin Noise Generator
	private float PerlinNoise(Vector3 p, float freq){

		//initialize values, points, and floats
		int hx0, hx1, hy0, hy1, hy2, hy3;
		hx0 = hx1 = hy0 = hy1 = hy2 = hy3 = 0;
		Vector3 px0, px1, py0, py1, pz0, pz1, pz2, pz3;
		px0 = px1 = py0 = py1 = pz0 = pz1 = pz2 = pz3 = new Vector3 ();
		float vx0, vx1, vy0, vy1, vz0, vz1, vz2, vz3;
		vx0 = vx1 = vy0 = vy1 = vz0 = vz1 = vz2 = vz3 = 0f;

		//multiply point by frequency
		p*=freq;

		//floor vectors
		int x = Mathf.FloorToInt( p.x);
		int y = Mathf.FloorToInt( p.y); 
		int z = Mathf.FloorToInt( p.z);

		//assign interpolation floats
		float fx = p.x-x;
		float fy = p.y-y;
		float fz = p.z-z;

		//bitwise vectors
		x &= hashSize;
		y &= hashSize;
		z &= hashSize;

		//initialize hash values
		hx0 = nHash [x];
		hx1 = nHash [x + 1];

		if (dimensions > 2) {
			hy0 = nHash [hx0 + y];
			hy1 = nHash [hx1 + y];
			hy2 = nHash [hx0 + y +1];
			hy3 = nHash [hx1 + y +1];
		}


		//smoothing
		float xs = SmoothingCurve(fx);
		float ys = SmoothingCurve(fy);
		float zs = SmoothingCurve(fz);

		//switch based on return deminsion
		switch(dimensions){
		case 1:
			
			// set point values
			px0.x = grad1 [nHash [hx0] & 1];
			px1.x = grad1 [nHash [hx0] & 1];

			//set float vectors
			vx0 = px0.x * fx;
			vx1 = px1.x * (fx - 1f);

			//return interpolation
			return Mathf.Lerp(vx0,vx1,xs)*2f;

		case 2: 

			//set point values
			px0 = grad2 [nHash [hx0 + y] & 7];
			px1 = grad2 [nHash [hx1 + y] & 7];
			py0 = grad2 [nHash [hx0 + y + 1] & 7];
			py1 = grad2 [nHash [hx1 + y + 1] & 7];

			//set float vectors
			vx0 = DotProduct (px0, fx, fy);
			vx1 = DotProduct (px1, fx-1f, fy);
			vy0 = DotProduct (py0, fx, fy-1f);
			vy1 = DotProduct (py1, fx-1f, fy-1f);

			//return interpolation
			return Mathf.Lerp(Mathf.Lerp(vx0,vx1,xs),Mathf.Lerp(vy0,vy1,xs),ys)*sqr;

		case 3: 

			//set point values
			px0 = grad3 [nHash [hy0 + z] & 15];
			px1 = grad3 [nHash [hy1 + z] & 15];
			py0 = grad3 [nHash [hy2 + z] & 15];
			py1 = grad3 [nHash [hy3 + z] & 15];
			pz0 = grad3 [nHash [hy0 + z + 1] & 15];
			pz1 = grad3 [nHash [hy1 + z + 1] & 15];
			pz2 = grad3 [nHash [hy2 + z + 1] & 15];
			pz3 = grad3 [nHash [hy3 + z + 1] & 15];

			//set float vectors
			vx0 = DotProduct (px0, fx, fy, fz);
			vx1 = DotProduct (px1, fx - 1f, fy, fz);
			vy0 = DotProduct (py0, fx, fy - 1f, fz);
			vy1 = DotProduct (py1, fx - 1f, fy - 1f, fz);
			vz0 = DotProduct (pz0, fx, fy, fz - 1f);
			vz1 = DotProduct (pz1, fx - 1f, fy, fz - 1f);
			vz2 = DotProduct (pz2, fx, fy - 1f, fz - 1f);
			vz3 = DotProduct (pz3, fx - 1f, fy - 1f, fz - 1f);

			//return interpolation
			return Mathf.Lerp (
				Mathf.Lerp (Mathf.Lerp (vx0, vx1, xs), Mathf.Lerp (vy0, vy1, xs), ys),
				Mathf.Lerp (Mathf.Lerp (vz0, vz1, xs), Mathf.Lerp (vz2, vz3, xs), ys),
				zs);
		}

		//syntax return random
		return Random.Range (0f, 1f);
	}

	//Smoothing curve
	private float SmoothingCurve(float f){
		return ((6 * Mathf.Pow (f, 5)) - (15 * Mathf.Pow (f, 4)) + (10 * Mathf.Pow (f, 3)));
	}

	//Dot product
	private float DotProduct(Vector2 v, float x, float y){return v.x * x + v.y * y;}
	private float DotProduct(Vector3 v, float x, float y, float z){	return v.x * x + v.y * y+v.z*z;}

	//Create 2x size hash
	private int[] GenHash(int size){
		int[] hash = new int[size*2];

		for (int i = 0; i < size; ++i) {
			hash [i] = Random.Range (0, hashSize);
		}

		for (int i = size; i < size*2; ++i) {
			hash [i] = hash[i-size];
		}

		return hash;
	}

}
