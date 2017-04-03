using UnityEngine;
using System.Collections.Generic;

public class MapGeneratorX : MonoBehaviour {

	//External Classes
	public STDMathX stdMath;
	public BiomeRulesX biomeRules;

	//set map size (used as width / height)
	public int mapSize;


	//Tile enumorator, Pirme Tile, and Sub Tile
	public enum TileType {Plains, Desert, Swamp, Forest, Hills, River, Mountains, Sea};
	public TileType primeTile;
	public TileType subTile;
	public bool useRandomSub;

	//World Setup variables
	[Range(2,8)]	public int numBiomes = 2;			//Determines number of generated matrix biomes.
	[Range(0,10)]	public int refinements;				//Determines total number of refinements for cellular automata
	[Range(1,8)]	public int birthLimit;				//Determines chance of cell to live in Cellular automata
	[Range(1,8)]	public int deathLimit;				//Determines chance of a cell to die during cellular automata
	public bool countCenter;							//if true, counts center cell as living in cellular automata
	public bool countOutside;							//if true counts out of range cells as living in cullular automata
	[Range(0.0f,1f)]	public float primeSpawnRate;		//Determines chance of spawning a prime tile in matrix creation
	[Range(0.0f,0.5f)]	public float primeMinFill;			//Determines the minimum number of tiles required in matrix creation
	[Range(0.0f,0.5f)]	public float primeMaxFill;			//Determines the maximum number of tiles allowed in maxtrix creation

	[Range(0.0f,1f)]	public float subSpawnRate;			//Determines chance of spawning a prime tile in matrix creation
	[Range(0.0f,0.5f)]	public float subMinFill;			//Determines the minimum number of tiles required in matrix creation
	[Range(0.0f,0.5f)]	public float subMaxFill;			//Determines the maximum number of tiles allowed in maxtrix creation

	[Range(0f,1f)]	public float suppleSpawnRate;		//Determines spawn chance of supplemental biomes

	//Private Variables
	private GameObject mapContainer;
	private int[,] map;
	private int seed;
	private float tileWidth;
	private float[,] mapInfluence;
	private Bounds mapBounds;
	private List<int[,]> biomes = new List<int[,]>();
	private List<int> biomeKey = new List<int>();
	private List<float[,]> biomeInfluence = new List<float[,]> ();
	private Vector2 bottom = new Vector2 ();
	private Vector2 right = new Vector2 ();
	private Vector2 left = new Vector2 ();
	private Vector2 top = new Vector2 ();

	//temp update function TODO: remove after testing
	void Update(){
		if (Input.GetKeyDown ("space")) {
			Debug.Log ("RELOAD SCENE");
			UnityEngine.SceneManagement.SceneManager.LoadScene (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex);
		}
	}

	// Use this for initialization
	void Start () {
		//initialize Math
		//stdMath = scripter.GetComponent<STDMath>();

		//get generate seed
		//seed = scripter.GetComponent<Seeder>().hashedSeed;
		seed = stdMath.seed;
		Debug.Log (seed);

		//create Map game object
		mapContainer = new GameObject();
		mapContainer.name = "Map";

		//initialize Map
		map = new int[mapSize,mapSize];

		CreateMap();

		BuildBaseTiles ();
	}

	//Create intager map
	void CreateMap(){

		//Generate primary biome and apply automata
		map = stdMath.GenerateMatrix (mapSize, primeSpawnRate);
		map = stdMath.CellularAutomata (map, refinements, birthLimit, deathLimit, countCenter, countOutside);

		//create primary influence map
		mapInfluence = stdMath.BiomeInfluenceMapper (map, 1);

		//cycle through number of biomes
		for (int i = 0; i < numBiomes-1; i++) {

			//create modified biome
			int[,] mat = stdMath.ModifiedMatrix (mapSize, subSpawnRate, refinements, birthLimit, deathLimit, mapInfluence);

			//create biomes
			mat = biomeRules.AutomataToBiome(mat,2,birthLimit,deathLimit);

			//Assign random/ set sub biome
			int sub = (useRandomSub | i > 0) ? biomeRules.GetNewBiome ((int)primeTile) : (int)subTile;
			mat = stdMath.ConvertMatrix (mat, sub);

			//apply biome rules
			mat = biomeRules.ApplyRules (mat, sub, false);

			//check if first biome created
			if (i == 0) {
				biomes.Add (mat);
				biomeKey.Add (stdMath.TilesInMatrix (mat, sub));
			} else {

				//establish number of tiles in current biome
				int tiles = stdMath.TilesInMatrix (mat, sub);

				//Not first Biome. Cycle biome list
				for (int b = 0; b < biomes.Count; b++) {

					//check if the number of tiles in current matrix is less than the number in the current element.
					if (!(tiles < biomeKey [b])) {

						//More tiles in current matrix. Insert current matrix before current element.
						biomes.Insert (b, mat);
						biomeKey.Insert (b, tiles);
						break;
					} else if (b == biomes.Count - 1) {

						//if less tiles in current matrix than any others add to the end
						biomes.Add (mat);
						biomeKey.Add (tiles);
					}
				}
			}
		}

		//cycle biomes list to create combined biome map
		int[,] biomeMap = new int[mapSize,mapSize];
		for (int i = 0; i < biomes.Count; i++) {

			Debug.Log ("Biome " + i + " has " + biomeKey [i] + " tiles");

			//cycle biome axises
			for (int x = 0; x < mapSize; x++) {
				for (int y = 0; y < mapSize; y++) {

					//if valid tile set map tile to biome tile
					if (biomes [i] [x, y] >= 0) {
						biomeMap [x, y] = biomes [i] [x, y];
					}
				}
			}
		}

		//cycle map a final time to create composite map
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {

				//check if valid tile and influence is greater than spawn
				if (map [x, y] != 0) {
					
					//Set prime tile
					biomeMap [x, y] = (int)primeTile;
				}
			}
		}

		//set final map
			map = biomeMap;

	}


		

	void MergeBiomes(){

		//initialize array to sort biome order
		List<int> sortedBiomes = new List<int>();
		List<int> sortedBiomesKey = new List<int>();

		//cycles biomes
		for(int i =0; i< biomes.Count;i++){

			//variables to hold number of tiles in array
			int num = stdMath.TilesInMatrix(biomes[i], biomeKey[i]);

			//check if sorted biomes has any elements
			if(sortedBiomes.Count<1){

				//if no elements. add element
				sortedBiomes.Add (i);
				sortedBiomesKey.Add (num);

			}else{
				//cycle sorted list
				for(int j= 0;j< sortedBiomes.Count;j++){

					//check if number of tiles is less than the current element
					if(num < sortedBiomesKey[j]){

						//if less than current elements, add before element
						sortedBiomes.Insert(j,i);
						sortedBiomesKey.Insert(j,num);
						break;


					}else if (j ==sortedBiomes.Count-1){

						//if greater than all or last in list add to end
						sortedBiomes.Add (i);
						sortedBiomesKey.Add (num);
						break;
					}
				}

			}
		}

		Debug.Log (sortedBiomes.Count + " Biomes to mege");

		//set map to prime biome
		map = biomes[0];

		//cycle the biomes list in the sorted order
		//this will add the biomes with the most tiles first
		for(int i=sortedBiomes.Count-1; i>=0;i--){

			Debug.Log ("adding Biome: " + sortedBiomes [i]);

			//process all biomes except prime
			if (biomeKey [i] != 0) {
				//cycle biome axises
				for (int x = 0; x < mapSize; x++) {
					for (int y = 0; y < mapSize; y++) {

						//set map tile to valid biome tile
						if (biomes [sortedBiomes [i]] [x, y] >= 0) {
							map [x, y] = biomes [sortedBiomes [i]] [x, y];
						}
					}
				}
			}
		}

		//check map to ensure no empty tiles. if empty tiles set it to prime tile
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {
				map[x,y]=(map[x,y]<0)?(int)primeTile:map[x,y];
			}
		}
	}

	//Create Tile Game Objects
	void BuildBaseTiles(){

		//Temp
		//map = biomes[0];

		//init counter
		int counter = 0;

		//Base tile locations
		string[] baseTiles = {"Tile Assets/0","Tile Assets/1","Tile Assets/2","Tile Assets/3","Tile Assets/4","Tile Assets/5"};


		//cycle map x & y
		for (int x = 0; x < mapSize; x++) {
			for (int y = 0; y < mapSize; y++) {

				//print out tile type
				Debug.Log(map[x,y]);

				//create new tile as game object
				GameObject tile = new GameObject();

				//name new tile
				tile.name = "Tile "+ counter.ToString();

				//Set Tag and sprite strings based on tile type
				string tag="Plains";
				string sp ="Tile Assets/0";
				switch (map [x, y]) {

				case (int)TileType.Plains:
					tag = "Plains";
					sp = baseTiles[0];
					break;
				case (int)TileType.Desert:
					tag = "Desert";
					sp = baseTiles[1];
					break;
				case (int)TileType.Swamp:
					tag = "Swamp";
					sp = baseTiles[2];
					break;
				case (int)TileType.Forest:
					tag = "Forest";
					sp = baseTiles[3];
					break;
				case (int)TileType.Hills:
					tag = "Hills";
					sp = ((int)primeTile < 3) ? baseTiles [(int)primeTile] : baseTiles [0];
					break;
				case (int)TileType.River:
					tag = "River";
					sp = baseTiles[4];
					break;
				case (int)TileType.Mountains:
					tag = "Mountains";
					sp = ((int)primeTile < 3) ? baseTiles [(int)primeTile] : baseTiles [0];
					break;
				case (int)TileType.Sea:
					tag = "Sea";
					sp = baseTiles[5];
					break;

				default:
					tag = "Sea";
					sp = baseTiles[5];
					break;
				}

				//tag new game object
				tile.tag = tag;

				// create new sprite renderer and add it to tile
				SpriteRenderer sr = tile.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

				//Set sprite of sprite renderer
				sr.sprite = Resources.Load<Sprite>(sp);

				//Set tile to child of Map game object
				tile.transform.parent = mapContainer.transform;

				//find tile width based on tile size and pixels per unit. Divided by 2 to center tiles TODO: subtract 0.02f to clear lines 
				tileWidth = sr.sprite.rect.width / (2*sr.sprite.pixelsPerUnit);

				//Get new isometric coordinates from cartisian tile map. 
				Vector2 loc = stdMath.IsoLocation (x * tileWidth, y * tileWidth);

				//Set z index to seperate tiles in z space to allow for asset placement. TODO: might set z space to 0, and use z space soley for assets and units.
				float zLoc = x + y;

				//apply location to tile
				tile.transform.position = new Vector3 (loc.x, loc.y, zLoc);

				//Attatch tile script
				TileX t = tile.AddComponent(typeof(TileX)) as TileX;

				//initialize tile script variables
				t.tileSR = sr;
				t.stdMath = stdMath;
				t.tileType = map[x,y];
				t.primeType = (int)primeTile;
				t.TileLoc = tile.transform.position;
				t.MapLoc = new Vector2 (x, y);
				t.tileWidth = tileWidth;
				t.tilePixelWidth = sr.sprite.rect.width;
				t.tilePPU = sr.sprite.pixelsPerUnit;
				t.tileSize = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
				t.zBack = zLoc+0.0001f;
				t.zFront = zLoc-0.9999f;
				t.SetupTile ();

				//increment bounds
				mapBounds.Encapsulate(sr.bounds);

				//increment counter
				counter++;
			}
		}

	}


	/* ===================================================================================================================================
	 * 
	 * 									Getters / Setters
	 * 			 				
	 *=================================================================================================================================== */

	//get tile width
	public float GetTileWidth(){
		return tileWidth;
	}

	//get map bounds
	public Bounds GetMapBounds(){
		return mapBounds;
	}

	//get map center
	public Vector2 GetMapCenter(){
		return new Vector2 (mapBounds.center.x, mapBounds.center.y);
	}
}
