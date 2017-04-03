using UnityEngine;
using System.Collections;

public class TileX : MonoBehaviour {

	//Tile Information variables
	public STDMathX stdMath;
	public SpriteRenderer tileSR;	//The Tiles sprite renderer
	public int primeType;			//the tile type that is prime
	public int tileType;			//type of tile
	public bool surrounded;			//is tile surrounded

	//Physical variables
	public Vector3 TileLoc;			//3D world location
	public Vector2 MapLoc;			//2D Matrix location
	public Vector3 tileCenter;		//3D world tile center
	public Vector3 tileTop;			//Top point of tile
	public Vector3 tileRight;		//Right point of tile
	public Vector3 tileLeft;		//Left point of tile
	public Vector3 tileBottom;		//Bottom point of tile
	public float tileWidth;			//Size of the rendered tile
	public float tileSize;			//Tile Size in Units
	public float tilePPU;			//Tile pixels per unit
	public float tilePixelWidth;	//Width of tile sprite 
	public float zBack;				//z location directly infront of tile
	public float zFront;			//furthest z location before next tile

	//Asset Variables
	private int assetRangeMin;
	private int assetRangeMax;
	public int numAssets;


	public void SetupTile(){
		//set random state
		//Random.InitState (stdMath.seed);

		//set tile coordinates
		SetCoordinatePoints();

		//Build Tile Assets
		BuildAssets ();
	}




	//Set tile coordinate points
	private void SetCoordinatePoints(){

		//set tile center
		tileCenter = new Vector3 (tileSR.sprite.bounds.center.x, tileSR.sprite.bounds.center.y + tileSize / 4, tileSR.sprite.bounds.center.z);

		//set tile Top
		tileTop = new Vector3 (tileSR.sprite.bounds.center.x, tileSR.sprite.bounds.center.y + tileSize / 2, tileSR.sprite.bounds.center.z);

		//set tile bottom
		tileBottom = tileSR.sprite.bounds.center;

		//set tile left
		tileLeft = new Vector3 (tileSR.sprite.bounds.center.x - tileSize / 2, tileSR.sprite.bounds.center.y + tileSize / 4, tileSR.sprite.bounds.center.z);

		//set tile Right
		tileRight = new Vector3 (tileSR.sprite.bounds.center.x + tileSize / 2, tileSR.sprite.bounds.center.y + tileSize / 4, tileSR.sprite.bounds.center.z);

	}

	/*=====================================================================================//
	 * ===================================== TILE ASSET FUNCTIONS============================//
	 * ====================================================================================*/

	//Build Tile Assets
	public void BuildAssets(){



		//switch between tile types and apply asserts based on type
		switch (tileType) {

		//Plains
		case 0:
			break;

		//Desert
		case 1:
			break;

		//Swamp
		case 2:

			//set tile variables
			assetRangeMin = 24;
			assetRangeMax = 27;
			numAssets = 7;

			//randomly decided whether to add swamp puddle
			if (Random.Range (0f, 1f) < 0.4) {

				//randomly choose one of the swamps
				int r = Random.Range(19,21);
				AddAsset ("Puddle", r,1f, true, 0.001f);
			}

			//Add swamp trees
			AddTrees();

			break;

		//Forest
		case 3:
			//set tile variables
			assetRangeMin = 28;
			assetRangeMax = 30;
			numAssets = 22;

			//Add trees
			AddTrees();

			break;

		//Hills
		case 4:
			//set variables based on tile type
			if (primeType == 1) {			//Desert
				assetRangeMin = 9;
				assetRangeMax = 11;
			} else if (primeType == 2) {		//Swamp
				assetRangeMin = 12;
				assetRangeMax = 14;
			} else {						//Plains
				assetRangeMin = 6;
				assetRangeMax = 8;
			}

			//TODO: Set universal location
			AddAsset ("Hill", Random.Range (assetRangeMin, assetRangeMax + 1),0.8f,true,-1f);
			break;

		//River
		case 5:
			break;

		//Mountain
		case 6:
			//set tile variables
			assetRangeMin = 15;
			assetRangeMax = 18;

			//TODO: Set universal location
			AddAsset ("Mountain", Random.Range (assetRangeMin, assetRangeMax + 1),1.125f,true, -1f);
			break;

		//Sea
		case 7:
			break;
			
		}
	}

	//Add trees
	private void AddTrees(){

		//Cycle the number of assets
		for (int i = 0; i < numAssets; i++) {

			//generate random number based on tile asset type
			int r = Random.Range(assetRangeMin,assetRangeMax+1);

			//add asset at random position
			string s = "Tree " + r.ToString();
			AddAsset (s, r,1f, false,-1f);
		}
	}

	//Add Asset
	private void AddAsset(string name, int assetLoc, float scale, bool center, float centerZ){

		//create game object, and assign name, tag, and parent
		GameObject asset = new GameObject ();
		asset.name = name;
		asset.tag = "Tile Asset";
		asset.transform.parent = gameObject.transform;

		//create sprite renderer for asset
		SpriteRenderer sr = asset.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

		//set asset sprite
		string s = "Tile Assets/"+assetLoc.ToString();
		sr.sprite = Resources.Load<Sprite> (s);

		//set asset scale
		asset.transform.localScale = new Vector3(scale,scale, 1);

		//asset location variables
		Vector3 loc;

		//check if asset is centered or randomly placed
		if (center) {

			//get modified sprite y and z
			float y = ((sr.sprite.rect.height/tilePPU)*0.5f)/scale;
			float z = (centerZ > 0) ? -centerZ : -0.5f;
			//set location
			loc = new Vector3(tileCenter.x,y,-0.5f);

		} else {
			//get random point
			float chance = Random.Range(0f,1f);
			//Debug.Log (chance);
			Vector3 v = stdMath.PlaceDiamond (chance, tileRight, tileTop,tileBottom, tileLeft);

			//get asset coordinate center
			float y = ((sr.sprite.rect.height/sr.sprite.pixelsPerUnit)/2)+v.y;
			float x = (sr.sprite.rect.width / sr.sprite.pixelsPerUnit) / 4;

			//create z location
			float z = ((0.99f/tileTop.y)*v.y)-0.99f;

			//Set asset location
			loc = new Vector3 (v.x, y, z);
		}

		//set asset location
		asset.transform.localPosition = loc;

		//randomly flip tile asset
		if (Random.Range (0f, 1f) < Random.Range(0f, 1f)) {
			sr.flipX = true;
		}

	}

}
