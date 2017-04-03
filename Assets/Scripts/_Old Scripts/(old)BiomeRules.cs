using UnityEngine;
using System.Collections.Generic;

public class BiomeRulesX : MonoBehaviour {

	public STDMathX stdMath;
	public PathfinderX pathfinder;

	[Range(0,10)] public int biomeMinSize;	//Determines the minimum size of created biomes
	[Range(5,15)] public int biomeMaxSize;	//determines the maximum size of created biomes

	//Plains Rule Variables
	[Range(0,9)]
	public int minPlainsBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Desert Rule Variables
	[Range(0,9)]
	public int minDesertBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Swamp Rule Variables
	[Range(0,9)]
	public int minSwampBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Forest Rule Variables
	[Range(0,9)]
	public int minForestBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Hill Rule Variables
	[Range(0,9)]
	public int minHillsBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//River Rule Variables
	[Range(0,9)]
	public int minRiverBiomeSize;			//Determines the area size a biome needs to be to apply rules. Less than this size will remove.
	[Range(0,10)]
	public int spawnRiverChance;			//Chance for a river cluster to spawn river
	[Range(0,10)]
	public int numRivers;					//number of potential rivers

	//Mountain Rule Variables
	[Range(0,9)]
	public int minMountainBiomeSize;		//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

	//Sea Rule Variables
	[Range(0,9)]
	public int minSeaBiomeSize;				//Determines the area size a biome needs to be to apply rules. Less than this size will remove.

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
				int[] moveCosts = {1,1,1,1,2,0,-1,-1};
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
			else if (r >= 0.65f && r < 0.85f) 	{biome = 5;} //River	- 20%
			else if (r >= 0.85f && r < 0.95f) 	{biome = 6;} //Mountain	- 10%
			else 								{biome = 7;} //Sea	 	- 05%

			//return sub biome
			return biome;

	//Desert
		case 1:
			//generate random number
			r = Random.Range (0f, 1f);

			//check random number against potentail subtiles
			//not using switch here to allow for percent settings
			if (r < 0.01f) 						{biome = 0;} //Plains	- 01%
			else if (r >= 0.01f && r < 0.11f)	{biome = 2;} //Swamp	- 10%
			else if (r >= 0.11f && r < 0.16f) 	{biome = 3;} //Forest	- 05%
			else if (r >= 0.16f && r < 0.41f) 	{biome = 4;} //Hill		- 25%
			else if (r >= 0.41f && r < 0.66f) 	{biome = 5;} //River	- 25%
			else if (r >= 0.66f && r < 0.91f) 	{biome = 6;} //Mountain	- 25%
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
			else if (r >= 0.20f && r < 0.35f) 	{biome = 3;} //Forest	- 15%
			else if (r >= 0.35f && r < 0.55f) 	{biome = 4;} //Hill		- 20%
			else if (r >= 0.55f && r < 0.89f) 	{biome = 5;} //River	- 34%
			else if (r >= 0.89f && r < 0.99f) 	{biome = 6;} //Mountain	- 10%
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
			else if (r >= 0.30f && r < 0.31f)	{biome = 1;} //Desert	- 01%
			else if (r >= 0.31f && r < 0.40f) 	{biome = 2;} //Swamp	- 09%
			else if (r >= 0.40f && r < 0.60f) 	{biome = 4;} //Hill		- 20%
			else if (r >= 0.60f && r < 0.80f) 	{biome = 5;} //River	- 20%
			else if (r >= 0.80f && r < 0.95f) 	{biome = 6;} //Mountain	- 15%
			else 								{biome = 7;} //Sea	 	- 05%

			//return sub biome
			return biome;
		}
			
		//If no prime biom selected return random biome
		return Random.Range(0,7);
	}




	//Convert Automata to biomes
	public int[,] AutomataToBiome(int[,] mat, int numRefine, int birth, int death){

		//declare/initialize arrays
		int[,] bio;
		int[,] hold = mat;
		bool[,] control = new bool[mat.GetLength (0), mat.GetLength (0)];

		//initialize random x and y coordinates
		int rx = 0;
		int ry = 0;

		//cycle initial matrix x and y axises
		for (int i = 0; i < mat.GetLength (0); i++) {
			for (int j = 0; j < mat.GetLength (0); j++) {

				//generate random coordinate 
				rx=Random.Range(0,mat.GetLength(0));
				ry=Random.Range(0,mat.GetLength(1));

				//check if point was previously used. if previously used continue to next, else set to true.
				if(control[rx,ry]){
					continue;
				}else{
					control[rx,ry] = true;
				}

				//set tile type
				int type = mat[rx,ry];


				//generate random biome width/ height
				int w = Random.Range(biomeMinSize,biomeMaxSize+1);
				int h = Random.Range(biomeMinSize,biomeMaxSize+1);
				bio = new int[w, h];

				//fill biome 
				for(int x = 0; x< bio.GetLength(0);x++){
					for(int y = 0; y<bio.GetLength(1);y++){

						//get x and y radius
						float xr = bio.GetLength(0)/2;
						float yr = bio.GetLength(1)/2;

						//apply smallest as used radiues
						float r = Mathf.Min(xr,yr);

						//generate euclidic distance
						float e = Mathf.Sqrt(Mathf.Pow((x-xr),2)+Mathf.Pow((y-yr),2));

						//if the euclidic distance of the biomes cell is within the smallest radius fill cell.
						if(e<r){
							bio[x,y]=type;
						}
					}
				}

				//refine the new biome
				bio = stdMath.CellularAutomata(bio,numRefine,birth,death,false,false);

				//replace tiles in matrix with new biome
				for(int x = 0; x< bio.GetLength(0);x++){
					for(int y = 0; y<bio.GetLength(1);y++){

						//biome real coordiates
						int xr = rx+x;
						int yr = ry+y;

						//ensure that tile can be placed on mat
						if(!(xr>=mat.GetLength(0) || yr >= mat.GetLength(1))){
							hold[xr,yr] = bio[x,y];
						}
					}
				}
			}
		}

		//return modified holding map
		return hold;
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
				int live = stdMath.CountLiving(mat,x,y,false,true);

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
	private int[,] PathfindBiome(int[,] mat, int tileType, int[]moveCosts, int clusterSize, int spawnPath, int numPaths, bool diagonal, bool outside){

		//List to hold clusters 
		List<Vector2> clusters = new List<Vector2> ();

		//cycle through matrix
		for (int x = 0; x < mat.GetLength(0); x++) {
			for (int y = 0; y < mat.GetLength(0); y++) {

				//get the cluster size of cell
				int cluster = stdMath.CountLiving (mat, x, y, false, true);

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
				List<Vector2> path = pathfinder.FindPath (mat, v1, v2, moveCosts, diagonal, outside);

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
