using UnityEngine;
using System.Collections.Generic;

public class MatrixMath {

	//external class
	public STDMath stdMath;


	//Set seed
	public void Seed(int s){
		Random.InitState (s);
	}

	//Create Primary Matrix
	public int[,] PrimeMat(List<float[,]> fmats, int size, int prime, int sub, float[] adjust, int numBiomes){

		//initialize return matrix
		int[,] mat = new int[size, size];

		//cycle matrix
		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x) {

				//seet rgb values
				float r = fmats[0] [y, x] *(1f+adjust[0]);
				float g = fmats[1] [y, x] *(1f+adjust[1]);
				float b = fmats[2] [y, x] *(1f+adjust[2]);

				//find largeest int
				float t = Mathf.Max(Mathf.Max(r,g),Mathf.Max(r,b));

				//set matrix value
				if (t == r) {
					mat [y, x] = prime;
				} else if (t == g) {
					mat [y, x] = sub;
				}else{
					
					//if no other biomes set to prime tile else set to -1
					mat [y, x] = (numBiomes>2) ? -1 : prime;
				}
			}
		}

		//return mat
		return mat;
	}

	//create supple matrix
	public int[,] CreateMat(float[,] fmat, int size, int prime, int sub, bool primary){

		//initialize return matrix
		int[,] mat = new int[size, size];

		//cycle matrix
		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x) {

				//round value from original matrix
				int i = Mathf.RoundToInt (fmat [y, x]);

				//check if primary or supple matrix
				if (primary) {

					//If Primary sheck if filled tile, if filled convert to sub
					mat [x, y] = (i > 0) ? prime : -1;

				} else {

					//If not Primary check if filled tile, if filled convert to supple
					mat [x, y] = (i > 0) ? sub : -1;
				}
			}
		}

		//return matrix
		return mat;
	}

	//merge supple matrices
	public int[,] MergeMats(List<float[,]> fmats, List<int> subs, int size){

		//intialize return mat and testing mat
		int[,] mat = new int[size, size];
		float[,] fmat = fmats [0];

		//cycle supple mats
		for (int f = 0; f < fmats.Count; ++f) {

			//cycle matrix
			for (int y = 0; y < size; ++y) {
				for (int x = 0; x < size; ++x) {
				
					//Check for highest value
					if (!(fmat [y, x] > fmats [f] [y, x]) && fmats[f][y,x] > 0) {

						//if checked value is high assign
						fmat [y, x] = fmats [f] [y, x];
						mat [y, x] = subs[f];
					}
				}
			}
		}

		//return mat
		return mat;
	}

	//Merge Supple Matrix with prime Matrix
	public int[,] SuppleToPrimeMatrix(int[,] prime, int[,] supple, int size){

		//initialize return matrix
		int[,] mat = new int[size, size];

		//cycle matrix
		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x) {

				//if prime matrix is = -1, set to supple vale
				mat[y,x] = (prime[y,x]>=0)? prime[y,x]:supple[y,x];
			}
		}

		//return matrix
		return mat;
	}
				


	//Fill blanks and remove stand alone tiles
	public int[,] CleanMat(int[,] mat,int size, int prime){

		//initialize return matrix
		int[,] retmat = new int[size, size];

		//cycle matrix
		for (int y = 0; y < size; ++y) {
			for (int x = 0; x < size; ++x) {

				//get current tile value
				int t = mat [y, x];

				//check if tile should be filled
				bool fillTile = (t >= 0);
				List<int> fillVal = new List<int> (8);

				//get dominate surrounding tile type
				int live = 0;
				for (int i = -1; i < 2; ++i) {
					for (int j = -1; j < 2; ++j) {

						//set neighbor coordinates
						int nx = j + x;
						int ny = i + y;

						//confirm neighbor is on map
						if (!(nx < 0 || nx >= mat.GetLength (0) || ny < 0 || ny >= mat.GetLength (1))) {

							//confirm real value
							if (mat [ny, nx] >= 0) {

								//increment fill value
								++live;
								fillVal [mat [ny, nx]] = ++fillVal [mat [ny, nx]];
							}
						}
					}
				}

				//check if tile needs to be filled or any similar surround tiles
				if (!fillTile || live > 0 || !(fillVal [t] > 0)) {

					//cycle fill to find highest value
					t = 0;
					for (int i = 1; i < 8; ++i) {

						t = Mathf.Max (fillVal [t], fillVal [i]);
					}
				} else if (live <= 0) {
					t = prime;
				}

				//set return mat value
				retmat [y, x] = t;
			}
		}

		return retmat;
	}


	//Count Living//
	public int CountLive(int[,] mat, int x, int y, bool countCenter, bool countOutside){
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

	//Gaussian Blur
	public float[,] GaussianFilter(float[,] fmat, int iterations, bool outside){

		//initialize final matrix
		float[,] final = new float[fmat.GetLength (0), fmat.GetLength (1)];

		//cycle per iterations
		for (int re = 0; re <= iterations; ++re) {

			//initialize filter matrix
			float[,] filtered = new float[fmat.GetLength (0), fmat.GetLength (1)];

			//cycle axis of original matrix
			for (int i = 0; i < fmat.GetLength (0); ++i) {
				for (int j = 0; j < fmat.GetLength (1); ++j) {

					//create holding float
					float f = 0f;

					//create divisor
					int d = 0;

					//cycle neighbors
					for (int y = -1; y < 2; ++y) {
						for (int x = -1; x < 2; ++x) {

							//define neighbor coordinates
							int nx = j + x;
							int ny = i + y;

							//check if coordinate is outside original matrix
							if (!outside) {
								if (!(nx < 0 || ny < 0 || nx >= fmat.GetLength (0) || ny >= fmat.GetLength (1))) {

									//increment float by neighbor values
									f += (re == 0) ? fmat [nx, ny] : final [nx, ny];
									++d;
								}
							} else {
								if (!(nx < 0 || ny < 0 || nx >= fmat.GetLength (0) || ny >= fmat.GetLength (1))) {
									//increment float by neighbor values
									f += (re == 0) ? fmat [nx, ny] : final [nx, ny];
									++d;
								} else {
									f += (re == 0) ? fmat [j, i] : final [j, i];
									++d;
								}
							}
						}
					}

					//average float value.
					f = f / d;

					//set float to new matrix
					filtered [j, i] = f;
				}
			}

			//set filtered to final matrix
			final = filtered;
		}

		//return final matrix
		return final;
	}
}
