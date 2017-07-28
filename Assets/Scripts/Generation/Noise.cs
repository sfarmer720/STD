using UnityEngine;
using System.Collections;

public class Noise : MonoBehaviour {

	//Physical variables
	public int resolution = 256;
	[Range(0f,100000f)]		public float elasticity = 1.42f;
	[Range(0.001f,25000f)]	public float density = 1.2f;
	private float speed;
	private int halfRes;

	//Wave variables
	public int waveLength = 10;
	public int amplitude = 20;
	public int decibels = 86;
	public int phase= 0;

	public bool randomAttenuation = false;
	[Range(0.001f, 0.999f)]	public float attenuation = 0.5f;

	private float freq;
	private float pressure;
	private float velocity;
	private float intensity;

	//Private variables
	private Texture2D texture;
	private float[,] textureGrid;

	public Texture2D GenerateNoise(){

		//initialize variables
		speed  = Mathf.Sqrt(elasticity/density);
		freq = speed / waveLength;
		pressure = Mathf.Pow (10, (decibels / 20)) * 0.00002f;
		velocity = waveLength * freq;
		intensity = velocity * pressure;
		halfRes = resolution / 2;
		texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, true);
		textureGrid = InitGrid ();

		//apply texture
		ApplyTexture();

		//return texture
		return texture;
	}


	//Wake and create
	private void Awake(){
		speed  = Mathf.Sqrt(elasticity/density);
		freq = speed / waveLength;
		pressure = Mathf.Pow (10, (decibels / 20)) * 0.00002f;
		velocity = waveLength * freq;
		intensity = velocity * pressure;
		halfRes = resolution / 2;
		texture = new Texture2D (resolution, resolution, TextureFormat.RGB24, true);
		textureGrid = InitGrid ();
		texture.name = "Noise";
		GetComponent<MeshRenderer> ().material.mainTexture = texture;
		textureGrid = InitGrid ();
		ApplyTexture ();
	}

	//Create new texture
	public void CreateTexture(){

		//initialize grid
		textureGrid = new float[resolution, resolution];

		//convert grid

	}

	private float[,] InitGrid(){

		//create new grid
		float[,] grid = new float[resolution,resolution];

		//fill grid
		for (int j = 0; j < resolution; ++j) {
			for (int i = 0; i < resolution; ++i) {
				grid [j, i] = 0f;//Random.Range (0f, 1f);
			}
		}

		//initialize changing amplitude and coordinates

		int life = 0;

		int x = 0;
		int incX = 1;

		int y = phase+halfRes;
		float per = resolution/freq/2;
		float amp = amplitude;
		float mag = amp+Mathf.Abs(y);
		float incY = 2;

		//loop while wave has amplitude
		while(amp > 2){
			Debug.Log(x+" | "+y+" | "+amp+" | "+mag+" | "+per+" | "+incX+" | "+incY);
			//confirm coordinates are on grid
			if(x >= 0 && y >= 0 && x<resolution && y <resolution){

				//set coordinate based on intensity
				grid[y,x] = 1f;//intensity;


			}

			//modify x coordinates
			if(x+incX >= resolution || x+incX<0){
				incX = -incX;

				//reduce amplitude
				++life;
				//--amp;
				amp = amp*Mathf.Pow(2.71828f,-attenuation*x);
				mag = amp+Mathf.Abs(y);
				incY = amp/per;
				Debug.Log ("new Amp: " + amp);
			}
			x += incX;

			//modify y coordinate
			if(Mathf.Abs(y+incY)>= mag || y+incY >= resolution || y+incY < 0 ){
				incY = -incY;
			}
			y += (int)incY;

		}

		//return new grid
		return grid;
	}


	//Apply texture
	private void ApplyTexture(){
		
		for (int y = 0; y < resolution; ++y) {
			for (int x = 0; x < resolution; ++x) {

				float c = textureGrid [y, x];

				texture.SetPixel (x, y, new Color (c, c, c));
			}
		}
		texture.Apply ();
	}
}
