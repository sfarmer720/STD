using UnityEngine;
using System.Collections.Generic;

public class BiomeRules : MonoBehaviour {

	public STDMath stdMath;
	//public Pathfinder pathfinder;

	[Range(0,10)] public int biomeMinSize=3;	//Determines the minimum size of created biomes
	[Range(5,15)] public int biomeMaxSize=15;	//determines the maximum size of created biomes

	//Plains Rule Variables
	[Range(0,9)]
	public int minPlainsBiomeSize = 4;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Desert Rule Variables
	[Range(0,9)]
	public int minDesertBiomeSize=4;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Swamp Rule Variables
	[Range(0,9)]
	public int minSwampBiomeSize=4;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Forest Rule Variables
	[Range(0,9)]
	public int minForestBiomeSize=4;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Hill Rule Variables
	[Range(0,9)]
	public int minHillsBiomeSize=0;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//River Rule Variables
	[Range(0,9)]
	public int minRiverBiomeSize=5;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.
	[Range(0,10)]
	public int spawnRiverChance=0;			//Chance for a river cluster to spawn river
	[Range(0,10)]
	public int numRivers=0;					//number of potential rivers

	//Mountain Rule Variables
	[Range(0,9)]
	public int minMountainBiomeSize=0;		//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Sea Rule Variables
	[Range(0,9)]
	public int minSeaBiomeSize=5;				//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Array of Float[,] for influences
	public List<float[,]> biomeInfluences = new List<float[,]>();

	//TODO: Function to get related biomes

	//Primary switch class. Only call this rul function.
	public int[,] ApplyRules(int[,] mat, int biomeType, bool isPrime){

		//apply different rules sets based on type of biome
		switch (biomeType) {

		//Plains Biome
		case 0:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
				//Remove small clusters
				mat = RemoveSmallCluster(mat,minPlainsBiomeSize);
			}
			break;

			//Desert Biome
		case 1:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
				//Remove small clusters
				mat = RemoveSmallCluster(mat,minDesertBiomeSize);
			}
			break;

			//Swamp Biome
		case 2:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
				//Remove small clusters
				mat = RemoveSmallCluster(mat,minSwampBiomeSize);
			}
			break;

			//Forest Biome
		case 3:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
				//Remove small clusters
				mat = RemoveSmallCluster(mat,minForestBiomeSize);
			}
			break;

			//Hills Biome
		case 4:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
			}
			break;

			//River Biome
		case 5:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
				//Remove small clusters
				mat = RemoveSmallCluster(mat,minRiverBiomeSize);

				//Create paths
				float[] moveCosts = {1,1,1,1,2,0,-1,-1};
				mat = PathfindBiome (mat, biomeType, moveCosts, minRiverBiomeSize, spawnRiverChance, numRivers, false, true);
			}
			break;

			//Mountains Biome
		case 6:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules
			}
			break;

			//Sea Biome
		case 7:
			//check if prime biome
			if (isPrime) {

			} else {

				//if not prime biome apply sub biome rules

			}
			break;
		}

		//map biomes influence
		//BiomeInfluenceMapper(mat,biomeType);

		//return modified matrix
		return mat;
	}

	//Biome Preference selector
	public int GetNewBiome(int prime){

		//initialize biome int and random float
		int biome = -1;
		float r;

		//switch between tile types based on prime tile
		switch (prime) {
		//Plains
		case 0:
			//generate random number
			r = Random.Range (0f, 1f);

			//check random number against potentail subtiles
			//not using switch here to allow for percent settings
			if (r < 0.05f) 						{biome = 1;} //Desert	- 05%
			else if (r >= 0.05f && r < 0.15f)	{biome = 2;} //Swamp	- 10%
			else if (r >= 0.15f && r < 0.40f) 	{biome = 3;} //Forest	- 25%
			else if (r >= 0.40f && r < 0.65f) 	{biome = 4;} //Hill		- 25%
			else if (r >= 0.65f && r < 0.75f) 	{biome = 5;} //River	- 10%
			else if (r >= 0.75f && r < 0.95f) 	{biome = 6;} //Mountain	- 20%
			else 								{biome = 7;} //Sea	 	- 05%

			//return sub biome
			return biome;

			//Desert
		case 1:
			//generate random number
			r = Random.Range (0f, 1f);

			//check random number against potentail subtiles
			//not using switch here to allow for percent settings
			if (r < 0.1f) 						{biome = 0;} //Plains	- 10%
			else if (r >= 0.10f && r < 0.25f)	{biome = 2;} //Swamp	- 15%
			else if (r >= 0.25f && r < 0.30f) 	{biome = 3;} //Forest	- 05%
			else if (r >= 0.30f && r < 0.55f) 	{biome = 4;} //Hill		- 25%
			else if (r >= 0.55f && r < 0.70f) 	{biome = 5;} //River	- 15%
			else if (r >= 0.70f && r < 0.95f) 	{biome = 6;} //Mountain	- 25%
			else 								{biome = 7;} //Sea	 	- 09%

			//return sub biome
			return biome;

			//Swamp
		case 2:
			//generate random number
			r = Random.Range (0f, 1f);

			//check random number against potentail subtiles
			//not using switch here to allow for percent settings
			if (r < 0.15f) 						{biome = 0;} //Plain	- 15%
			else if (r >= 0.15f && r < 0.20f)	{biome = 1;} //Desert	- 05%
			else if (r >= 0.20f && r < 0.45f) 	{biome = 3;} //Forest	- 25%
			else if (r >= 0.45f && r < 0.60f) 	{biome = 4;} //Hill		- 15%
			else if (r >= 0.60f && r < 0.85f) 	{biome = 5;} //River	- 25%
			else if (r >= 0.85f && r < 0.99f) 	{biome = 6;} //Mountain	- 14%
			else 								{biome = 7;} //Sea	 	- 01%

			//return sub biome
			return biome;

			//Forest
		case 3:
			//generate random number
			r = Random.Range (0f, 1f);

			//check random number against potentail subtiles
			//not using switch here to allow for percent settings
			if (r < 0.30f) 						{biome = 0;} //Plains	- 30%
			else if (r >= 0.30f && r < 0.35f)	{biome = 1;} //Desert	- 05%
			else if (r >= 0.35f && r < 0.45f) 	{biome = 2;} //Swamp	- 10%
			else if (r >= 0.45f && r < 0.60f) 	{biome = 4;} //Hill		- 15%
			else if (r >= 0.60f && r < 0.70f) 	{biome = 5;} //River	- 10%
			else if (r >= 0.70f && r < 0.90f) 	{biome = 6;} //Mountain	- 20%
			else 								{biome = 7;} //Sea	 	- 10%

			//return sub biome
			return biome;
		}

		//If no prime biom selected return random biome
		return Random.Range(0,7);
	}







	/*=====================================================================================//
	 * ===================================== ACTUAL BIOME RULES============================//
	 * ====================================================================================*/

	//Remove small clusters rule
	//this rule removes a cell if it is not surrounded by enough living cells
	private int[,] RemoveSmallCluster(int[,] mat, int minClusterSize){

		// cycloe matrix
		for(int x = 0; x< mat.GetLength(0); x++){
			for(int y = 0; y < mat.GetLength(0);y++){

				//get number of neighbors
				int live = stdMath.countLiving(mat,x,y,false,true);

				//check if living neighbors is larger than required cluster size
				if(live < minClusterSize){

					//if not enbough neighbors, remove cell
					mat[x,y] = -1;
				}
			}
		}

		//return modified matrix 
		return mat;
	}

	//Pathfind biomes
	//This rull creates trails between similar biomes
	private int[,] PathfindBiome(int[,] mat, int tileType, float[]moveCosts, int clusterSize, int spawnPath, int numPaths, bool diagonal, bool outside){

		//List to hold clusters 
		List<Vector2> clusters = new List<Vector2> ();

		//cycle through matrix
		for (int x = 0; x < mat.GetLength(0); x++) {
			for (int y = 0; y < mat.GetLength(0); y++) {

				//get the cluster size of cell
				int cluster = stdMath.countLiving (mat, x, y, false, true);

				//get random number
				int r = Random.Range (0, 10);

				//if cluster size is big enough and r < spawn chance
				if (cluster >= clusterSize && r < spawnPath) {

					//add to list of potential clusters
					clusters.Add (new Vector2 (x, y));

				}

			}
		}

		//check if there are more than 2 clusters
		if (clusters.Count > 1) {

			//cycle pathfinder set number of times
			for (int i = 0; i < numPaths; i++) {

				Vector2 v1, v2;

				//Infinate Loop
				//get 2 different vectors from cluster list
				while (true) {

					//generate 2 random numbers
					int r1 = Random.Range (0, clusters.Count - 1);
					int r2 = Random.Range (0, clusters.Count - 1);

					//check if number are the same
					if (r1 != r2) {

						//set vectors
						v1 = clusters [r1];
						v2 = clusters [r2];
						break;
					}

				}

				//get path
				List<Vector2> path = stdMath.GetTilePath (v1, v2, mat, moveCosts, diagonal);

				//cycle path and change tiles to tile type
				for (int p = 0; p < path.Count; p++) {

					//change matrix cell based on vector
					mat [(int)(path [p].x), (int)(path [p].y)] = tileType;
				}
			}
		}

		//return modified matrix
		return mat;
	}

}
