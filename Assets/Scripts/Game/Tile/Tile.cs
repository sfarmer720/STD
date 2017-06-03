using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {


	//external classes
	public STDMath stdMath;

	//Tile information
	public int primeType;
	public int tileType;
	public float highestY = 0;
	public bool visited = false;
	public bool visible = false;
	public Vector3 TileLoc;			//3D world location
	public Vector2 MapLoc;			//2D Matrix location
	public float tileWidth;			//Size of the rendered tile
	public bool surrounded;
	public List<GameObject> neighbors = new List<GameObject> ();


	//Terrain Variables
	public Vector3[,] vertMat;
	public Vector3[,] neighborMat;
	public int sqrt;
	public float[,] noise;
	public Vector2 noiseStart;


	//Asset Variables
	public bool isSelected = false;
	private int assetRangeMin;
	private int assetRangeMax;
	public int numAssets;
	public GameObject[] tileAssets;
	public List<Vector3> assetLocations = new List<Vector3>();


	//Colliders
	public Rigidbody tileBody;
	public BoxCollider tileBounds;

	//Mesh Variables
	private MeshRenderer mr;
	private MeshFilter mf;
	private float[,] tileNoise;


    //Unit variables
    public bool occupied = false;
    public bool isBeingHealed = false;
    public List<Unit> healedBy = new List<Unit>();


	//Fixed Update
	void FixedUpdate(){

        //set to false, will reset if triggered again
        occupied = false;

        //set tiel visiblity
		TileVisiblity();

        //check if tile is still being healed
        CheckHeals();

	}


	public void InitTile (STDMath stdmath, Generator gen,
		int tiletype, int prime, int tilewidth, int sq, Vector2 maploc,
		float[,] noisemap, Vector2 noisestart,
		bool useAssets, int numassets, GameObject[] assets,
		Material mat
	){

		//initialize variables
		stdMath = stdmath;
		tileType = tiletype;
		primeType = prime;
		tileWidth = tilewidth;
		sqrt = sq;
		MapLoc = maploc;
		TileLoc = this.gameObject.transform.position;

		noise = noisemap;
		noiseStart = noisestart;

		numAssets = numassets;
		tileAssets = assets;

		//Initialize mesh variables
		mr = this.gameObject.GetComponent<MeshRenderer>();
		mf = this.gameObject.GetComponent<MeshFilter> ();
		mr.material = mat;

		//modeify terrain
		SetTerrain();

		//set assets
		if (useAssets) {
			SetAssets ();
		}

		//Create Box Collider
		CreateCollider();

	}
		
	//Set neighbor tiles
	public void SetNeighbors(Generator map){

        //get lsit of neighbors
        List<Vector2> neighborsV2 = stdMath.GetNeighbors(map.GetMap(), MapLoc);

        //cycle list
        for(int i = 0; i < neighborsV2.Count; ++i)
        {
            //Add neighbor game object
            neighbors.Add(map.GetTile(neighborsV2[i]));
        }

		//check if neighbor types all match
		for (int i = 0; i < neighbors.Count; ++i) {
			if (neighbors [i].GetComponent<Tile> ().tileType != tileType) {
				surrounded = false;
				break;
			} else {
				if (i == neighbors.Count - 1) {
					surrounded = true;
				}
			}
		}
	}

	public void SetTerrain(){

		//pull tile arrays
		Vector3[] verts = mf.mesh.vertices;
		Vector2[] uv = mf.mesh.uv;
		int[] tri = mf.mesh.triangles;

		//modify terrain
		verts = stdMath.Terraform(verts, noiseStart, noise, sqrt);

		//set vertex matrix
		vertMat = stdMath.VectorMatrix(verts);

		//Set new verts and arrays
		mf.mesh.vertices = verts;
		mf.mesh.uv = uv;
		mf.mesh.triangles = tri;
		mf.mesh.RecalculateBounds ();
		mf.mesh.RecalculateNormals ();

		//find highest Y in tile
		for (int i = 0; i < verts.Length; ++i) {

			Vector3 v = transform.TransformPoint (verts [i]);
			highestY = Mathf.Max (highestY, v.y);

		}

	}

	//add tile assets
	private void SetAssets(){
		
		//iterate equal to number of assets
		for (int i = 0; i < numAssets; ++i) {
			
			//create new vector for asset location and add to list
			Vector3 loc = stdMath.AssetLoc (assetLocations, TileLoc, (int)(tileWidth*0.5f));
			assetLocations.Add (loc);

			//set random rotaion
			Quaternion rot = Quaternion.Euler(0f,Random.Range(0f,360f),0f);

			//instantiate new asset
			GameObject a =  Instantiate (
				tileAssets [Random.Range (0, tileAssets.Length-1)],
				loc+this.gameObject.transform.position,
				rot,
				this.gameObject.transform
			) as GameObject;

			//set scale, name, layer, and tag //TODO: create switch for assets to name and tag
			float s = Random.Range(0.75f,1f);
			a.transform.localScale = new Vector3(s,s,s);
			a.layer = 12; //9;

		}
	}


	public void Selection(){
		isSelected = !isSelected;
	}

	public void Selection(Transform t){

	}

	//Selction - Main function
	public void Selection(int clickType){

		//one click functions
	//	if (clickType == 0) {

			//toggle seelction
			SelectionToggle ();
		//}


	}

	//Selection Togggle
	public void SelectionToggle(){

		//check if already selected
		if (isSelected) {
			isSelected = false;

		} else {
			isSelected = true;
		}
	}



	//Set Tile material
	public void SetTileMaterial(Material mat){
		mr.material = mat;
	}


	//Set tiles visiblity
	public void TileVisiblity(){
		
		//check if tile has been visited previously
		if (visited) {

			//check if tile is currently visible
			if (visible) {

				//tile is visible, set to visible layer
				SetTileLayer(12);

			} else {

				//tile is not visible, set to visited layer
				SetTileLayer(13);

			}
		} else {

			//tile hasn't been visited, set to hidden layer
			SetTileLayer(14);
		}
			
	}

	//set tile to visited not visible
	public void SetToVisited(){

		visible = false;
		visited = true;
	}

	//set tile to visible
	public void SetToVisible(){

		visible = true;
		visited = true;
	}
    

	//update units sight
	void OnTriggerStay(Collider other){

		//check if collideded with a unit
		if (other.gameObject.transform.parent.gameObject.layer == 11) {

			//Check if sight trigger
			if (other.gameObject.name == "Sight Sphere") {

               UnitSight us = other.gameObject.transform.parent.gameObject.GetComponent<UnitSight>();
                us.AddToSeen(this.gameObject);
            }
        }
	}

	void OnTriggerExit(Collider other){
		//check if collideded with a unit
		if (other.gameObject.transform.parent.gameObject.layer == 11) {

			//Check if sight trigger
			if (other.gameObject.name == "Sight Sphere") {

                UnitSight us = other.gameObject.transform.parent.gameObject.GetComponent<UnitSight>();
                us.AddToLost(this.gameObject);

            }
        }
	}



	//Create tile Box collider
	private void CreateCollider(){

		//create new game object to be child, set its layer to the ignorable layer
		GameObject go = new GameObject();
		go.transform.parent = this.gameObject.transform;
		go.name = this.gameObject.name + " Trigger";
		go.layer = 2;
        
		//Create box collider
		tileBounds = go.AddComponent<BoxCollider> ();
		tileBounds.center = this.gameObject.transform.position;
		tileBounds.size = new Vector3 (mr.bounds.size.x-0.001f, 100f, mr.bounds.size.z-0.001f);
		tileBounds.isTrigger = true;

		//create rigidbody
		tileBody = this.gameObject.AddComponent<Rigidbody>();
		tileBody.useGravity = false;
		tileBody.freezeRotation = true;
		tileBody.isKinematic = true;
        
	}

		
	//Set tile layers
	public void SetTileLayer(int layer){

		//get all children
		Transform[] children;
		children = this.gameObject.GetComponentsInChildren<Transform> ();

		//cycle all children, and set layers
		foreach (Transform child in children) {

			if (child.gameObject.layer != 2) {
				child.gameObject.layer = layer;
			}
		}

		//set main layer
		this.gameObject.layer = layer;
	}

    /* ===================================================================================================================================
   * 
   * 									Unit based function
   * 			 			Functions effected by units interaction with tile
   *=================================================================================================================================== */

    //Add a healer to tile
    public void AddHealer(Unit u)
    {
        //set tile to being healed
        if (!isBeingHealed) { isBeingHealed = true; }

        //add healer to healer list
        healedBy.Add(u);
    }

    //remove a healer
    public void RemoveHealer(Unit u)
    {
        //check if healer is affectign tile
        if (healedBy.Contains(u))
        {
            //remove healer
            healedBy.RemoveAt(healedBy.IndexOf(u));
        }

        //Stop healing tile if no more healers
        isBeingHealed = (healedBy.Count >= 1);
    }


    //Check if still healing
    public void CheckHeals()
    {
        //check if tile is being healed
        if (isBeingHealed)
        {
            //create array of healers to remove
            List<int> toRemove = new List<int>();

            //cycle healers
            for(int i = 0; i < healedBy.Count; ++i)
            {
                //check if unit still healing tile
                if(!(healedBy[i].Healing().isHealing && healedBy[i].Healing().tilesToHeal.Contains(this)))
                {
                    //unit is either no longer healing or no longer has this tile in sight, mark for removal
                    toRemove.Add(i);
                }
            }

            //cycle removal list
            for(int i = 0; i < toRemove.Count; ++i)
            {
                //remove healer
                healedBy.RemoveAt(i);
            }

            //Stop healing tile if no more healers
            isBeingHealed = (healedBy.Count >= 1);

        }
    }
}
