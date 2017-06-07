using UnityEngine;
using System.Collections.Generic;

public class Generator : MonoBehaviour {

	//Developer tools
	public bool devTools;
	public bool disableGeneration;


	//External Classes
	public STDMath stdMath;
	public BiomeRules biomeRules;
	public GenerateNoise generateNoise;
	public Assets assetHolder;

    public Controls controls;
	public SelectionScript selector;
	public QuickKeys quickKeys;

	//FOW plane
	public GameObject fow;

	//Map variables
	public Bounds mapBounds;
	//public Vector3 mapCenter;
	public int mapSize = 20;
	public int tileWidth = 10;

	//Tile variables
	public enum TileType {Plains, Desert, Swamp, Forest, Hills, River, Mountains, Sea};
	public TileType primeTile;
	public TileType subTile;
	//public Material[] baseMaterials;
	//public GameObject[] blocks;
	//public GameObject[] tileAssets;
	public int[] sqrts = { 1, 2, 4, 8, 16 };
	[Range(0f, 5f)] public float noiseHeight = 2.5f;
	[Range(0,10)]	public int noiseBlur = 3;
	public int numAssets = 10;

	//use random prime/sub tile
	public bool randomPrime;
	public bool randomSub;

	//World Setup variables
	[Range(2,8)]		public int numBiomes = 2;				//Determines number of generated matrix biomes
	[Range(0.1f,1f)]	public float primeInfluence=0.4f;				//Determines total number of refinements for cellular automata
	[Range(0f,1f)]		public float subInfluence=0.3f;				//Determines chance of cell to live in Cellular automata
	[Range(0f,1f)]		public float suppleInfluence=0.05f;				//Determines chance of a cell to die during cellular automata
	//public bool countCenter;								//if true, counts center cell as living in cellular automata
	//public bool countOutside;								//if true counts out of range cells as living in cullular automata
	//[Range(0.0f,1f)]	public float primeSpawnRate=0.4f;	//Determines chance of spawning a prime tile in matrix creation
	//[Range(0.0f,0.5f)]	public float primeMinFill=0.1f;		//Determines the minimum number of tiles required in matrix creation
	//[Range(0.0f,0.5f)]	public float primeMaxFill=0.065f;	//Determines the maximum number of tiles allowed in maxtrix creation

	//[Range(0.0f,1f)]	public float subSpawnRate=0.25f;	//Determines chance of spawning a prime tile in matrix creation
	//[Range(0.0f,0.5f)]	public float subMinFill=0f;			//Determines the minimum number of tiles required in matrix creation
	//[Range(0.0f,0.5f)]	public float subMaxFill=0f;			//Determines the maximum number of tiles allowed in maxtrix creation

	[Range(0f,1f)]	public float suppleSpawnRate=0.25f;		//Determines spawn chance of supplemental biomes


	//Private Variables
	private List<List<GameObject>> goMap = new List<List<GameObject>>();
	private int[,] map;
	private int seed;
	private int section;
	private float[,] mapInfluence;
	private float[,] noiseMap;
	private List<int[,]> biomes = new List<int[,]>();
	private List<int> biomeKey = new List<int>();
	private List<float[,]>biomeInfluence = new List<float[,]>();
	private Vector3[] noiseVector = new Vector3[4];

	private Texture2D noiseTex;
	private GameObject[] blocks;
	private GameObject[] tileAssets;
	private float[,] terraMap;


	private Material[] baseMaterials;


	//player / antiplayer variables
	public Player player;
	public Antiplayer antiplayer;


	// Use this for initialization
	void Start () {

		//development bool
		if (!disableGeneration) {


			//import Seed
			seed = stdMath.seed;
			Random.InitState (seed);

			//initialize arrays
			blocks = assetHolder.blocks;
			tileAssets = assetHolder.tileAssets;
			selector = GameObject.Find ("EventSystem").GetComponent<SelectionScript> ();
			quickKeys = selector.gameObject.GetComponent<QuickKeys> ();
			//	baseMaterials = assetHolder.materials;

			//initialize map and noise map
			map = new int[mapSize, mapSize];

			//create Map
			GenNoise ();
			GenMat ();
			GenMap ();

			//Set objects
			SetCam ();

			//Set Players
			GenPlayer ();


			//TEMP
			//SetUnit();

			//run dev tools
			if (devTools) {
				DevTools ();
				//OnDrawGizmos ();
			}

		}
	}
    




	//update functions
	void Update(){

		//set constant variables


		//cam.transform.position = new Vector3 (mapCenter.x - 40, 65f, mapCenter.z - 40f);

		//camCon.SetTarget (mapCenter);


	}

	/* ===================================================================================================================================
	 * 
	 * 										Generate Noise
	 * 			 					Used to generate noise texture
	 *=================================================================================================================================== */


	void GenNoise(){

		//set variables
		generateNoise.seed = seed;
		generateNoise.Reset ();
		generateNoise.resolution = (mapSize > 20) ? (mapSize / 20) * 256 : 256;

		//set vectors
		noiseVector[0] = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		noiseVector[1] = transform.TransformPoint(new Vector3(0.5f,-0.5f));
		noiseVector[2] = transform.TransformPoint(new Vector3(-0.5f,0.5f));
		noiseVector[3] = transform.TransformPoint(new Vector3(0.5f,0.5f));

		//generate noise
		noiseMap = generateNoise.GenArray (noiseVector);

		//blur noise map
		noiseMap = stdMath.Blur(noiseMap,noiseBlur,true);


		//generate tile noise, using 16x16 verts per block	3,5,9,16
		//TODO: reduce size of this based on blocks used
		//generateNoise.resolution = 2000;
		//terraMap = generateNoise.GenArray (noiseVector);

	}


	/* ===================================================================================================================================
	 * 
	 * 										Main Map
	 * 			 				Used to create and fill tile map
	 *=================================================================================================================================== */

		

	void GenMat(){

		//check for random sub
		int sub = (randomSub)?biomeRules.GetNewBiome ((int)primeTile):(int)subTile;

		//add prime and sub biomes to biome key
		biomeKey.Add((int)primeTile);
		biomeKey.Add (sub);

		//generate primary Noise map
		generateNoise.Reset();
		List<float[,]> primeTex = generateNoise.GenColorArray(noiseVector,false);

		//create influence adjuster
		float[] adj = { 
			(primeInfluence - 0.035f * (numBiomes - 2)),
			(subInfluence - 0.03f * (numBiomes - 2)),
			(suppleInfluence * (numBiomes - 2)) };

		//generate primary biome
		map = stdMath.createPrimeMatrix(primeTex, mapSize,(int)primeTile, sub, adj, numBiomes);

		//check for other biomes
		if (numBiomes > 2) {

			//initialize biome lists
			List<float[,]> biomeNoises = new List<float[,]> ();
			List<int> biomeSubs = new List<int> ();

			//cycle number of biomes
			for (int i = 0; i < numBiomes - 2; ++i) {

				//set biome noise variables
				generateNoise.octaves = Random.Range (1, 8);
				generateNoise.lacunarity = Random.Range (1f, 4f);
				generateNoise.persistance = Random.Range (0f, 1f);

				//set new sub biome and add it to list
				sub = biomeRules.GetNewBiome ((int)primeTile);
				biomeSubs.Add (sub);
				biomeKey.Add (sub);


				//create biome noise map and add to list
				float[,] biomeNoise = generateNoise.GenArray (noiseVector);
				biomeNoises.Add (biomeNoise);

				//create new biome and add to list
				int[,] biome = stdMath.createMatrix (biomeNoise, mapSize, (int)primeTile, sub, false);
				biomes.Add (biome);
			}

			//merge biomes
			int[,] bio = stdMath.mergeMats (biomeNoises, biomeSubs, mapSize);

			//combine supple and prime matrices
			map = stdMath.finalMat (map, bio, mapSize);

			//clean matrix
			//map = stdMath.cleanMatrix (map, mapSize, (int)primeTile);
		}

	}

		
	void GenMap(){

		//Initialize variables
		int tileNum = 0;
		int useBlock = 3;
		int sqrt = sqrts [useBlock];
		bool useAssets = false;

		//generate tile height map
		float[,] tileHeights = GenTileNoise(sqrt);

		//cycle map
		for(int y = 0; y < mapSize; ++y){

			//add new line to reference map
			goMap.Add(new List<GameObject>());

			for(int x = 0; x < mapSize; ++x){

				//Initialize tile variables
				string tags;
				int mat = 0;
				int tileType = map [y, x];
				float startX = Mathf.Max ((sqrt * x) - 1, 0);
				float startY = Mathf.Max ((sqrt * y) - 1, 0);
				Vector2 noiseStart = new Vector2 (sqrt * y, sqrt * x);

				//tile switch
				switch (tileType) {

				case (int)TileType.Plains:		tags = "Plains";mat = 0;break; 
				case (int)TileType.Desert:		tags = "Desert";mat = 1;break;
				case (int)TileType.Swamp:		tags = "Swamp";useAssets = true;mat = 2;break;
				case (int)TileType.Forest:		tags = "Forest";useAssets = true;mat = 3;break;
				case (int)TileType.Hills:		tags = "Hills";mat = ((int)primeTile < 3) ? (int)primeTile : 0;break;
				case (int)TileType.River:		tags = "River";mat = 4;break;
				case (int)TileType.Mountains: 	tags = "Mountain";mat = 5;break;
				case (int)TileType.Sea:			tags = "Sea";mat = 6;break;
				default:						tags = "Sea";mat = 6;break;
				}

				//create new tile
				GameObject tile =  CreateTile(
												blocks[useBlock],
												new Vector3 (x * tileWidth, 0, y * tileWidth),
												tags,tileNum
											);
				//attach tile script
				Tile t = tile.AddComponent(typeof(Tile)) as Tile;

				//setup Tile
				t.InitTile (
					stdMath, this,
					tileType, (int)primeTile, tileWidth, sqrt, new Vector2 (x, y),
					tileHeights, noiseStart,
					useAssets, numAssets, assetHolder.GetTileAssets (tags),
					assetHolder.GetTileMat(mat)
			//		assetHolder.GetTileMaterials (false), assetHolder.GetTileMaterials (true), mat,
			//		assetHolder.FOW
				);

				tile.layer = 14; //(y < 5) ? 12 : 13;

				//attach collider
				tile.AddComponent(typeof(MeshCollider));

				//add tile to reference map
				goMap[y].Add(tile);

				//increment tile number
				++tileNum;

				//reset values
				useAssets = false;

			}
		}

		//assign tile neighbors
		for (int y = 0; y < mapSize; ++y) {
			for (int x = 0; x < mapSize; ++x) {
				GetTile (x, y).GetComponent<Tile> ().SetNeighbors (this);
			}
		}
		//Bake pathfinding map
		stdMath.BakePathfinder(map);

	}

	private float[,] GenTileNoise(int sqrt){

		//generate terra map
		generateNoise.Reset();
		generateNoise.resolution = (sqrt+1) * mapSize;
		float[,] terra = generateNoise.GenArray(noiseVector);

		//modify map
		terra = TerraMod(terra,sqrt);

		//return blurred map
		return stdMath.Blur(terra,noiseBlur, false);
	}

	private float[,] TerraMod(float[,] terra, int sqrt){

		//cycle terra map and apply height mod
		for (int y = 0; y < terra.GetLength (0); ++y) {
			for (int x = 0; x < terra.GetLength (1); ++x) { 
				terra [y, x] *= (noiseHeight*0.75f);
			}
		}

		//check if any hill, river, mountain, or sea biomes
		if (biomeKey.Contains ((int)TileType.Hills) ||
		   biomeKey.Contains ((int)TileType.River) ||
		   biomeKey.Contains ((int)TileType.Mountains) ||
		   biomeKey.Contains ((int)TileType.Sea)) {

			//if any needed biomes cycle map list
			for (int y = 0; y < mapSize; ++y) {
				for (int x = 0; x < mapSize; ++x) {

					//check if correct biome
					if (map [y, x] > 3) {

						//set terra coordinates, height, and mtn
						int mtn = map [y, x];
						Vector2 start = new Vector2 (sqrt * y, sqrt * x);

						//modify terra map
						terraMap = stdMath.ModifyTerraMap (terra, start, sqrt, mtn);

					}
				}
			}
		}

		//return terra map
		return terra;
	}

	private GameObject CreateTile(GameObject block, Vector3 loc, string tag, int tileNum){

		//create new game object
		GameObject tile = Instantiate (
			block, loc, Quaternion.Euler (0, 0, 0),
			this.gameObject.transform
		) as GameObject;

		//name new tile
		tile.name = "Tile "+tileNum.ToString();

		//assign tag
		tile.tag = tag;

		//assign layer
		tile.layer = 8;

		//return tile
		return tile;
	}
			



	/* ===================================================================================================================================
	 * 
	 * 									Player  / Antiplayer
	 * 			 			Functions used to initialize players
	 *=================================================================================================================================== */

	void GenPlayer(){

		//create players
		//TODO: player currently hard coded for offense
		player = new GameObject().AddComponent<Player>();
		player.name = "Player";
		player.Initialize (stdMath, this, assetHolder, selector,true, false, false);

		antiplayer = new GameObject().AddComponent<Antiplayer>();
		antiplayer.name = "Antiplayer";
		antiplayer.Initialize (stdMath, this, assetHolder, selector, false, true, false);
		antiplayer.InitializeAI ();


		//create player holding object
	//	player = new GameObject().AddComponent<Player>();
	//	player.InitPlayer (
	//		stdMath, this, assetHolder
	//	);
	}
    


    /* ===================================================================================================================================
	 * 
	 * 									Create and Set Cameras
	 * 			 			Used to set and create both main and processing cameras
	 *=================================================================================================================================== */



    private void SetCam(){

		//set map corners
		this.gameObject.GetComponentInChildren<CameraControls>().SetCorners(
			new Vector3 (0, 0, 0),
			new Vector3 (0, 0, mapSize * tileWidth),
			new Vector3 (mapSize * tileWidth, 0, 0),
			new Vector3 (mapSize * tileWidth, 0, mapSize * tileWidth)
		);

		//set camera target
		this.gameObject.GetComponentInChildren<CameraControls>().target = mapBounds.center;
	}





	/* ===================================================================================================================================
	 * 
	 * 									Getters & Setters
	 * 			 			Used to pull variables from variaous scripts
	 *=================================================================================================================================== */

    //Get Externals
    public Controls GetControls(){
        return controls;
    }


	//Get reference tile
	public int[,] GetMap(){
		return map;
	}
	public GameObject GetTile(int x, int y){
		return goMap [y] [x];
	}
	public GameObject GetTile(Vector2 v){
		return goMap [(int)v.y] [(int)v.x];
	}
	public GameObject GetFOW(){
		return fow;
	}


	//PLAYER & ANTIPLAYER
	public int GetGameState(){
		int i = (player.onDefense) ? player.gameState : antiplayer.gameState;
		return i;
	}















	void DevTools(){
		//print completion time to console
		Debug.Log(System.DateTime.Now);

		//print matrix to console
		Debug.Log (map.Length);

		//print number of biomes
		Debug.Log(biomes.Count);

		//print tile types to console
		for (int y = 0; y < map.GetLength(0); y++) {
			for (int x = 0; x < map.GetLength(1); x++) {

				Debug.Log (map [x, y]);
			}
		}

		//Print Biome Maps
		for (int i = 0; i < biomes.Count; ++i) {

			//Debug.Log ("Biome " + i);

			for (int y = 0; y < biomes[i].GetLength (0); y++) {
				for (int x = 0; x < biomes[i].GetLength (1); x++) {

					//Debug.Log ("Biome "+i+": "+biomes [i] [x, y]);
				}
			}
		}

	}


}
