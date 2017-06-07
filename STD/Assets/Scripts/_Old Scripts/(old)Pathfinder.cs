using UnityEngine;
using System.Collections.Generic;

public class PathfinderX : MonoBehaviour {

	/* This class serves to find the shourtest distance between two points
	 * Two function exist in this class.
	 * the first function exists to find the shortest point between 2 input locations.
	 * the second function finds the nearest matching tile 
	 */

	//Get jsut final fValue from path
	private float finalF;
	public float FValue(int[,] map, Vector2 start, Vector2 end, int[] moveCost, bool diagnols, bool outside){
		FindPath (map, start, end, moveCost, diagnols, outside);
		return finalF;
	}

	//Find the shortest path between two input points
	//return a Vector 2 list of the points 
	public List<Vector2> FindPath(int[,] map, Vector2 start, Vector2 end, int[] moveCost, bool diagnols, bool outside){

		//create new vector to represent current location
		Vector2 current = start;

		//get the current tile type of the starting tile
		int tileType = map[(int)start.x,(int)start.y];

		//create a list of tested and untested vectors, and their parent vectors
		List<Vector2> tested = new List<Vector2>();
		List<Vector2> testedParent = new List<Vector2>();
		List<Vector2> untested = new List<Vector2> ();
		List<Vector2> untestedParent = new List<Vector2>();

		//create lists to hold movement costs/variables
		List<float> gTest = new List<float>();
		List<float> gUntest = new List<float>();
		List<float> hTest = new List<float>();
		List<float> hUntest = new List<float>();
		List<float> fTest = new List<float>();
		List<float> fUntest = new List<float>();

		//add current position to untested list.
		//add to untested so at the start of the loop it will be added to tested
		untested.Add(current);
		untestedParent.Add(current);
		gUntest.Add (0f);
		hUntest.Add ((Mathf.Sqrt(Mathf.Pow((start.x-end.x),2)+Mathf.Pow((start.y-end.y),2))));
		fUntest.Add ((gUntest[0]+hUntest[0]));

		//Infinate Loop
		//Loop through until a path has been completed or is deemed impossible
		bool exitLoop = false;
		while (!exitLoop) {

			//Add top untested point from previous itteration to tested, as this is the best possible choice currently.
			tested.Add(untested[0]);
			testedParent.Add(untestedParent[0]);
			gTest.Add (gUntest [0]);
			hTest.Add (hUntest [0]);
			fTest.Add (fUntest [0]);

			//remove top option from untested list.
			untested.RemoveAt(0);
			untestedParent.RemoveAt(0);
			gUntest.RemoveAt(0);
			hUntest.RemoveAt(0);
			fUntest.RemoveAt(0);

			//set current location to top position in tested list
			current = tested[0];

			//check if the goal has been reached. If so break the loop.
			Exit:
			if (current == end || exitLoop) {
				break;
			}

			//Begin path finding.
			//Cycle the neighbors of the current tile
			for (int x = -1; x < 2; x++) {
				for (int y = -1; y < 2; y++) {

					//Check if current location or if diagnol
					if ((x == 0 && y == 0) || (!diagnols && (x != 0 && y != 0))) {

						//if current location or diagonal location while not allowed.
							continue;
					}

					//set neighbor coordinates
					Vector2 neighbor = new Vector2(x+current.x, y+current.y);

					//Check if current coordiantes are outside of the map coordinates.
					if (neighbor.x < 0 || neighbor.y < 0 || neighbor.x >= map.Length || neighbor.y >= map.Length) {

						//if looking for an outside coordinate add coordinate and end loop
						if (outside) {
							//break loop
							exitLoop = true;
							goto Exit;
						}
					}else{

						//if no other variables apply seach check for the best next tile
						//get the neighboring tile type
						int type = map[(int)neighbor.x,(int)neighbor.y];

						//get the dynamic cost to move to neighbor tile.
						int cost = moveCost[type];

						//check if poisitive movement cost.
						if (cost < 0) {

							//if non poisititive cost. Hard break loop
							exitLoop = true;
							goto Exit;
						}

						//get general move cost
						float g = cost + gTest[0];

						//get the hueristic cost
						float h = (Mathf.Sqrt(Mathf.Pow((neighbor.x-end.x),2)+Mathf.Pow((neighbor.y-end.y),2)));

						//get the f value
						float f = g+h;


						//check for end of path
						if(neighbor ==end){

							//if neighbor coordinate is end coordinate
							//add it to the begining of tested lists and break loop
							tested.Insert(0, neighbor);
							testedParent.Insert(0,current);
							gTest.Insert(0,g);
							hTest.Insert(0,h);
							fTest.Insert(0,f);
							exitLoop = true;
							goto Exit;
						}

						//Check if neighbor is on untested list already
						for(int u = 0; u < untested.Count; u++){

							//check neighbor against coordinates in list
							if(untested[u] == neighbor){

								//if neighbor is in list, check if new neighbor f value is less than previous
								if( f < fUntest[u]){

									//if value is less, update placement in list to refect new f value
									untested.Insert(u,neighbor);
									untestedParent.Insert(u,current);
									gUntest.Insert(u,g);
									hUntest.Insert(u,h);
									fUntest.Insert(u,f);
								}

								//regardless of updated placement, continue to next neighboring cell.
								goto NextCell;
							}
						}

						//check if nieghbor is on tested list already
						for(int t = 0; t < tested.Count; t++){

							//check if neighboor against coordinate in list
							if(tested[t] == neighbor){

								//if neighbor is in list, check if current f value is less
								if(f < fTest[t]){

									//if f value is less update placement in list
									tested.Insert(t,neighbor);
									testedParent.Insert(t,current);
									gTest.Insert(t,g);
									hTest.Insert(t,h);
									fTest.Insert(t,f);
								}

								//regardless of updated placement, continue to next cell
								goto NextCell;
							}
						}

						/* ACTUAL PATHFINDING
						 * determine highest priority move by testing current f value against previous f values
						 * Test will determine if cell is placed at the beginning, in the middle, or end of the current untested lists
						 * the lower the f value the higher in the list it is placed.
						 */

						//confirm that their are other points to test in the list
						if(0 < fUntest.Count){
							
						//check if neighbors f value is less than the first value in the list
						if( f > fUntest[0]){

								//cycle the untested vectors
								for(int u = 0; u < fUntest.Count; u++){

									//check if neighbors f value is less than the current list items f value
									if( f < fUntest[u]){

										//if neighbors f value is less insert neighbor at location.
										untested.Insert(u,neighbor);
										untestedParent.Insert(u,current);
										gUntest.Insert(u,g);
										hUntest.Insert(u,h);
										fUntest.Insert(u,f);
									}

									//check if this is the last possible item in the list to check
									if(u == fUntest.Count-1){

										//no f value is less than neighbors. Add it to the end of the list
										untested.Add(neighbor);
										untestedParent.Add(current);
										gUntest.Add(g);
										hUntest.Add(h);
										fUntest.Add(f);
									}
								}
							}else{


								//if neighbor has a p vlue less than the first item in the list add it at the begining
								untested.Insert(0,neighbor);
								untestedParent.Insert(0,current);
								gUntest.Insert(0,g);
								hUntest.Insert(0,h);
								fUntest.Insert(0,f);
							}
						}else{

							//if there are no item to cyle through add neighbor to the list
							untested.Add(neighbor);
							untestedParent.Add(current);
							gUntest.Add(g);
							hUntest.Add(h);
							fUntest.Add(f);
						}
					}

					//call to proceed to next cell
					NextCell: continue;
				}
			}
		}

		//set final fValue
		finalF = fTest[0];

		//add the final tested / end vector to the return list
		List<Vector2> ret = new List<Vector2>();
		ret.Add(tested[0]);

		//Infinate Loop 
		//Loop continuously until a path from the last tested vector to the first is built
		bool build = true;
		while(build){
			
			//cycle through the tested vectors in reverse
			for( int t = tested.Count-1; t >=0; t--){

				//check if the last vector added to the return list is equal to the current vector being cycled
				if(tested[t] == ret[(ret.Count-1)]){

					//if vectors are equal, add current cycle vector's parent vector to the return list
					ret.Add(testedParent[t]);

					//check if parent vector is the starting vector
					if(testedParent[t] == start){

						//break cycle and loop
						build = false;
					}

					//break cycle
					break;
				}
			}
		}

		//return final vector list
		return ret;

	}

}
