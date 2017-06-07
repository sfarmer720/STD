using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STDMathX : MonoBehaviour {

	//Seed Variables
	public Seeder seeder;
	public int seed;

	//Sprite  Variables
	public int pixelsPerUnit;

	//Initialization
	void Start(){
		
		//define random seed
		seed = seeder.hashedSeed;
		Random.InitState (seed);

		Debug.Log (seed);
	}

	/* ===================================================================================================================================
	 * 
	 * 									Common Math
	 * 			 				Commonly used math functions
	 *=================================================================================================================================== */

	//Checks whether 2 numbers are within a specified deviance
	//Float - Main
	public bool WithinDeviance(float f1, float f2, float deviance){

		//check if  variable 1 is less than variable 2 + deviance and greater than variable 2 - deviance.
		if (f1 <= f2 + deviance && f1 >= f2 - deviance) {
			return true;
		} else {
			return false;
		}
	}
	//int-variant
	public bool WithinDeviance(int i1, int i2, int deviance){
		return WithinDeviance ((float)i1, (float)i2, (float)deviance);
	}


	//Check if number falls within range
	//Float - Main
	public bool InRange(float f, float min, float max){
		return(f > min && f < max);
	}

	//Check if point is within diamond
	public bool IsInDiamond(Vector3 p,Vector3 top, Vector3 right, Vector3 left, Vector3 bottom){
		return (IsInTriangle (p, top, right, left) || IsInTriangle (p, bottom, right, left) );
		}
	public bool IsInDiamond(Vector2 p,Vector2 top, Vector2 right, Vector2 left, Vector2 bottom){
		return IsInDiamond (new Vector3 (p.x, p.y, 0), new Vector3 (top.x, top.y, 0), new Vector3 (right.x, right.y, 0), new Vector3 (left.x, left.y, 0), new Vector3 (bottom.x, bottom.y));
	}


	//Check if point falls within a specified triangle
	//Float/Vector - Main
	public bool IsInTriangle(Vector3 point, Vector3 top, Vector3 right, Vector3 left){

		//find real area of triangle
		float tri = FindArea(top.x,top.y,right.x,right.y,left.x,left.y);

		//find the areas of the triangle using test point
		float a1 = FindArea(point.x,point.y,right.x,right.y,left.x,left.y);
		float a2 = FindArea(top.x,top.y,point.x,point.y,left.x,left.y);
		float a3 = FindArea(top.x,top.y,right.x,right.y,point.x,point.y);

		//return whether the areas using test point are equal to the real triangle area
		return(tri==a1+a2+a3);
	}

	//Check if point falls inside a rectangle
	public bool IsInRectangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4){

		//find area of rectangle
		float rec = FindArea (p1.x, p1.y, p2.x, p2.y, p3.x, p3.y) + FindArea (p1.x, p1.y, p4.x, p4.y, p3.x, p3.y);

		//find area of point in rectangle
		float a1 = FindArea (p1.x, p1.y, p.x, p.y, p4.x, p4.y); //Area of APD
		float a2 = FindArea (p4.x, p4.y, p.x, p.y, p3.x, p3.y); //Area of DPC
		float a3 = FindArea (p3.x, p3.y, p.x, p.y, p2.x, p2.y); //Area of CPB
		float a4 = FindArea (p.x, p.y, p2.x, p2.y, p1.x, p1.y); //Area of PBA

		//check if within aceptable deviance
		if (WithinDeviance (rec, (a1 + a2 + a3 + a4), 3)) {
			return true;
		}

		//return sum of areas
		return (rec == (a1 + a2 + a3 + a4));
	}
	public bool IsInRectangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4){
		return(IsInRectangle (new Vector3 (p.x, p.y, 0), new Vector3 (p1.x, p1.y, 0), new Vector3 (p2.x, p2.y, 0), new Vector3 (p3.x, p3.y, 0), new Vector3 (p4.x, p4.y, 0)));
	}

	//Find the total area based on given points
	public float FindArea(float x1, float y1, float x2, float y2, float x3, float y3){
		//calculate the area from the points
		return(Mathf.Abs((x1*(y2-y3)+x2*(y3-y1)+x3*(y1-y2))/2));
	}


	//randomly place point inside a diamond
	public Vector3 PlaceDiamond(float chance, Vector3 top, Vector3 right, Vector3 left, Vector3 bottom){

		//randomly generate number and check if less than chance
		if (Random.Range (0f, 1f) < chance) {

			//return point in front triangle
			return PlaceTriangle(bottom,right,left);

		} else {

			//return point in back of triangle
			return PlaceTriangle(top,right,left);
		}
	}

	//Randomly place points inside a triangle
	public Vector3 PlaceTriangle(Vector3 top, Vector3 right, Vector3 left){

		//Infinate Loop
		//Loop until valid point created
		while(true){

			//generate 2 random floats
			float rx = Random.Range(0.01f,0.99f);
			float ry = Random.Range(0.01f,0.99f);

			//Triangulate point
			float x = ((1-Mathf.Sqrt(rx))*left.x+(Mathf.Sqrt(rx)*(1-ry))*top.x+(Mathf.Sqrt(rx)*ry)*right.x);
			float y = ((1-Mathf.Sqrt(rx))*left.y+(Mathf.Sqrt(rx)*(1-ry))*top.y+(Mathf.Sqrt(rx)*ry)*right.y);

			//Check if point is within triangle
			if(IsInTriangle(new Vector3(x,y,0),top,right,left)){

				//return new point
				return new Vector3(x,y,0);
			}
		}
	}

	//Gaussian filter
	public float[,] GaussianFilter(float[,] mat){

		//initialize 2D float list to place hold final float[,]
		List<List<float>> floats = new List<List<float>>();

		//cycle the x axis of the initial matrix
		for(int i = 0; i<mat.GetLength(0);i++){

			//Add a new float list
			floats.Add(new List<float>());

			//cycle the y axis of the initial matrix
			for(int j = 0; j<mat.GetLength(0);j++){

				//create new float
				float f = 0f;

				//cycle surrounding tiles
				for(int x = -1; x<2; x++){
					for(int y = -1; y<2;y++){

						//define neighbor coordinates
						int nx = i+x;
						int ny = j+y;

						//check if coordinate is outside of the original matrix
						if(!(nx<0||ny<0||nx>=mat.GetLength(0)||ny>=mat.GetLength(0))){

							//increment new float by the value of each neighboring cell
							f+=mat[nx,ny];
						}
					}
				}

				//check if new float value / 9 is greater than 0, if not set value to 0
				f= (f/9 > 0) ? f/9 : 0;

				//add f to floats list
				floats[i].Add(f);
			}
		}

		//initialize final matrix
		float[,] ret = new float[mat.GetLength(0),mat.GetLength(0)];

		//convert floats list into returnable matrix
		for(int x=0;x<floats.Count;x++){
			for(int y = 0; y<floats[x].Count;y++){
				ret [x, y] = floats [x] [y];
			}
		}

		//return matrix
		return ret;
	}

	/* ===================================================================================================================================
	 * 
	 * 									Generation Math
	 *  					Math functions for prodcedural generation
	 *=================================================================================================================================== */

	//Generate randomly filled matrix
	//generate matrix with no restriction
	public int[,] GenerateMatrix(int size, float spawnChance){

		//create new matrix
		int[,] mat = new int[size,size];

			//cycle through matrix axises
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {

				// create random float value between 0 & 1
				float r = Random.Range (0f, 1f);

				//check if random value is less than spawn chances
				if (r < spawnChance) {

					//if value is less than spawn chance, cell spawns (higher spawn chance better chance to spawn)
					mat [x, y] = 1;

				} else {

					//if greater than spawn chance, cell is empty.
					mat [x, y] = 0;
				}

			}
		}
		
		//return finished matrix
		return mat;
	}

	//Generate modified square matrix. Modified random placement from influence map
	public int[,] ModifiedMatrix(int size, float spawn, int refine, int birth, int death, float[,] influence){

		//initialize return matrix
		int[,] mat = new int[size,size];
		int[,] temp = new int[size, size];

		//Infinate loop
		//Create temporary matrices with a max number of tiles
		while (true) {
		
			//create temporary matrix 
			temp = GenerateMatrix (size, spawn);

			//check if the number of tiles exceeds the maximum allowed
			if(TilesInMatrix(temp,1) <= (size*size*spawn)){

				//Less tiles in matrix than limit, break loop.
				break;
			}
		}

		//apply automata
		//temp = CellularAutomata (temp, 0, birth, death, false, true);

		//cycle through matrix
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {

				//check if living cell in temporary map
				if (temp [x, y] == 1) {

					//Cell is living. Check if influence is less than death limit.
					if (!(influence [x, y] > death)) {

						//too little influience. Set cell to living in return matrix
						mat [x, y] = 1;
					}
				}
			}
		}

		//return modified matrix
		return mat;
	}


	//Cellular Automata
	//used to refine random matrices into cave structured biomes
	public int[,] CellularAutomata(int[,] mat, int numRefinements, int birth, int death, bool countCenter, bool countOutside){

		//initialize holder matrix
		int[,] holdMat = new int[mat.GetLength(0),mat.GetLength(1)];

		//cycle refinement function specified number of times
		for(int refine = 0; refine < numRefinements; refine++){


			//cycle x and y axises
			for (int x = 0; x < mat.GetLength (0); x++) {
				for (int y = 0; y < mat.GetLength(1); y++) {

					//get number of living neighbors
					int live = CountLiving(mat,x,y,countCenter,countOutside);

					//Life & Death rules. If cell is equal to 1 it is alive, else it is considered dead
					if (mat [x, y] == 1) {

						//If cell is not overly surrounded by living cells, cell stays alive.
						//if cell is overly surrounded by live cells, cell dies from starvation.
						if (live < death) {
							holdMat [x, y] = 1;
						} else {
							holdMat [x, y] = 0;
						}


					} else {

						//If cell is dead, but surrounded by enough living cells, cell comes to life
						//If cell is dead, but not surrounded by enough live cells, cell stays dead
						if (live > birth) {
							holdMat [x, y] = 1;
						} else {
							holdMat [x, y] = 0;
						}

					}

				}
			}

		}

		//return new matrix
		return holdMat;
	}


	//Biome influence maps
	public float[,] BiomeInfluenceMapper(int[,] mat, int tileType){

		//initialize map size
		float[,] fmat = new float[mat.GetLength (0), mat.GetLength (1)];

		//cycle x and y axises
		for (int x = 0; x < mat.GetLength (0); x++) {
			for (int y = 0; y < mat.GetLength (1); y++) {

				//count number of living neighbors
				int live = CountLiving (mat, x, y, false, true);

				//set influence
				fmat [x, y] = ((float)live / 10);
			}
		}

		//return influence map
		return fmat;
	}

	//Convert Matrix to TileType
	public int[,] ConvertMatrix (int[,] mat, int tileType){

		//cycle x and y axises
		for (int x = 0; x < mat.GetLength (0); x++) {
			for (int y = 0; y < mat.GetLength (1); y++) {

				//replace 1 value with tile type value, and replace 0 value with -1 value
				if (mat [x, y] == 1) {
					mat [x, y] = tileType;
				} else {
					mat [x, y] = -1;
				}

			}
		}
		return mat;
	}


	//Count number of living neighbors
	//Count living in int matrix
	public int CountLiving(int[,] mat, int x, int y, bool countCenter, bool countOutside){
		int live = 0;

		//cycle surrouding tiles
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {

				//neighbor coordinates
				int nx = x + i;
				int ny = y + j;

				//if origin cell
				if (i == 0 && j == 0) {

					//if counting center cell increment
					if (countCenter) {
						live++;
					}
					continue;
				}

				//Check for outside coordinate
				if (nx < 0 || nx >= mat.GetLength(0) || ny < 0 || ny >= mat.GetLength(1)) {

					//if coordinate outside and counting outside coordinates as live, increment.
					if (countOutside) {
						live++;
					}
					continue;
				}

				//check if cell matches origin cell
				if (mat [x, y] == mat [nx, ny]) {

					//if matching increment
					live++;
					continue;
				}
				
			}
		}

		//return number of liveing neighbors cells
		return live;
	}

	//Count living in float matrix
	public float CountLiving(float[,] mat, int x, int y, bool countCenter, bool countOutside){
		float live = 0;

		//cycle surrouding tiles
		for (int i = -1; i < 2; i++) {
			for (int j = -1; j < 2; j++) {

				//neighbor coordinates
				int nx = x + i;
				int ny = y + j;

				//if origin cell
				if (i == 0 && j == 0) {

					//if counting center cell increment
					if (countCenter) {
						live++;
					}
					continue;
				}

				//Check for outside coordinate
				if (nx < 0 || nx >= mat.GetLength (0) || ny < 0 || ny >= mat.GetLength (0)) {

					//if coordinate outside and counting outside coordinates as live, increment.
					if (countOutside) {
						live++;
					}
					continue;
				}

				//check if cell matches origin cell
				if (mat [x, y] == mat [nx, ny]) {

					//if matching increment
					live++;
					continue;
				}
			}
		}
		//return number of liveing neighbors cells
		return live;
	}

	//count the number of similar tiles in a matrix
	public int TilesInMatrix(int[,] mat, int i){

		//initiate return int
		int ret = 0;

		//cycle matrix
		for(int x = 0; x< mat.GetLength(0);x++){
			for (int y = 0; y < mat.GetLength (1); y++) {

				//if cell is equal increment return int
				ret += (mat [x, y] == i) ? 1 : 0;
			}
		}

		//return total number
		return ret;
	}



	//Coordinate converstions
	//Point coordinate conversions
	public Vector2 IsoLocation(Vector2 v){
		return new Vector2((float)IsoLocationX(v.x,v.y), (float)IsoLocationY(v.x,v.y));
	}
	public Vector2 IsoLocation( int x, int y ){
		return new Vector2((float)IsoLocationX(x,y), (float)IsoLocationY(x,y));
	}
	public Vector2 IsoLocation( float x, float y ){
		return new Vector2((float)IsoLocationX(x,y), (float)IsoLocationY(x,y));
	}
	public Vector2 CartLocation( int x, int y ){
		return new Vector2((float)CartLocationX(x,y), (float)CartLocationY(x,y));
	}
	public Vector2 CartLocation( float x, float y ){
		return new Vector2((float)CartLocationX(x,y), (float)CartLocationY(x,y));
	}
	public Vector2 CartLocation( Vector2 v ){
		return new Vector2((float)CartLocationX(v.x,v.y), (float)CartLocationY(v.x,v.y));
	}

	//X coordinate conversions
	public int IsoLocationX( int x, int y ){
		return(x-y);
	}
	public float IsoLocationX( float x, float y ){
		return(x-y);
	}
	public int CartLocationX( int x, int y ){
		return((2*y+x)/2);
	}
	public float CartLocationX( float x, float y ){
		return((2*y+x)/2);
	}

	//Y coordinate conversions
	public int IsoLocationY( int x, int y ){
		return((x+y)/2);
	}
	public float IsoLocationY( float x, float y ){
		return((x+y)/2);
	}
	public int CartLocationY( int x, int y ){
		return((2*y-x)/2);
	}
	public float CartLocationY( float x, float y ){
		return((2*y-x)/2);
	}
}
