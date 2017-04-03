using UnityEngine;
using System.Collections.Generic;

public class Pathfinder{

	/* This class serves to find the shourtest distance between two points
	 * Two function exist in this class.
	 * the first function exists to find the shortest point between 2 input locations.
	 * the second function finds the nearest matching tile 
	 * 
	 * 
	 * If pathfinding finds an outside coordinate while outside is true, pathfinding will end
	 * 
	 */


	//Heuristic Variables
	List<float[,]> heuristics = new List<float[,]>();
	int[,] hKey;


	//Bake initial Heuristics
	public void BakeHeuristics(int[,] map){

		//initialize hKey
		hKey = new int[map.GetLength(0),map.GetLength(1)];

		//cycle map x and y
		for(int y = 0, i = 0; y < map.GetLength(0); ++y, ++i){
			for(int x = 0; x< map.GetLength(1); ++x, ++i){

				//create heuristic map, add to list, and set key
				float[,] heu = HeuristicMap(map,x,y);
				heuristics.Add(heu);
				hKey[y,x] = i;
			}
		}
	}

	//Create Heuristic map
	private float[,] HeuristicMap(int[,] map, int ex, int ey){

		//initialize float map
		float[,] hMap = new float[map.GetLength(0),map.GetLength(1)];

		//cycle map
		for(int y = 0; y < map.GetLength(0); ++y){
			for(int x = 0; x< map.GetLength(1); ++x){

				//set heuristic for each tile
				float f = ((Mathf.Sqrt (Mathf.Pow ((x - ex), 2) + Mathf.Pow ((y - ey), 2))));
				hMap [y, x] = f;
			}
		}

		//return float map
		return hMap;
	}

	//Pathfinder
	public List<Vector2> Pathfind(Vector2 start, Vector2 end, int[,] map, float[] costs, bool diagonalAllowed){

		/*
		 * Float Lists:
		 * 					[i][0] = Item's X Position
		 * 					[i][1] = Item's Y Position
		 * 					[i][2] = Parent X Position
		 * 					[i][3] = Parent Y Position
		 * 					[i][4] = G Value (movement cost)
		 * 					[i][5] = H Value (heuristic cost)
		 * 					[i][6] = F Value (G+H)
		 */

		//initialize lists
		List<float> final = new List<float>();
		List<List<float>> openList = new List<List<float>>();
		List<List<float>> closedList = new List<List<float>>();
		List<Vector2> finalPath = new List<Vector2> ();


		//initialize heuristics map
		float[,] hMap = heuristics[hKey[(int)end.y,(int)end.x]];

		//initialize current list and add to closed list
		List<float> current = GetFloatList(
			start.x, start.y, start.x, start.y, 0,
			hMap[(int)start.x,(int)start.y], 
			hMap[(int)start.x,(int)start.y]);
		closedList.Add (current);

		//add neighbors to open list
		openList = UpdateOpen(current, map, hMap,costs, end.x, end.y, openList,closedList,diagonalAllowed);

		//Infinate Loop
		//Continuously cycle until end tile has been found
		int hard = 0;
		while (true) {

			//set current list from first in open list
			current = openList[0];

			//check if current coordinates match end coordinates
			if ((current [0] == end.x && current [1] == end.y) || hard > 400) {
				
				//set final list and break loop
				final = current;

				//Set final Path
				finalPath = SetFinalPath(FloatListToVecList(closedList),FloatToVec(final), start);
				PrintPath (finalPath);

				//end pathfinding
				break;

			} else {

				//remove current list from open and add to closed
				closedList.Add (current);
				openList.RemoveAt (0);

				//update open list based on new current list
				openList = UpdateOpen(current, map, hMap,costs, end.x, end.y, openList,closedList,diagonalAllowed);
			}

			++hard;
		}


		return finalPath;				
	}


	//Create float list
	private List<float> GetFloatList(float x, float y, float px, float py, float g, float h, float f){
		return new List<float>(new float[] {x, y, px, py, g, h,f});
	}

	//check if in open list
	private int CheckList (List<float> item, List<List<float>> list){

		//ensure list has items
		if(list.Count > 0){

			//cycle list
			for(int i = 0; i < list.Count; ++i){

				//check for matching coordinates
				if(item[0] == list[i][0] && item[1] == list[i][1]){

					//compare f values
					if(item[6] < list[i][6]){

						//update list 
						return i;
					}
				}
			}
		}

		//return false value
		return -1;
	}

	//check if item is in list
	private bool OnList(List<float> item, List<List<float>> list){
		//cycle list
		for(int i = 0; i < list.Count; ++i){

			//check for matching coordinates
			if(item[0] == list[i][0] && item[1] == list[i][1]){
				return true;
			}
		}

		return false;
	}

	private List<List<float>> UpdateList(List<float> l, List<List<float>> l2, int i){
		l2[i] = l;
		return l2;
	}
		
	//Update open list
	private List<List<float>> UpdateOpen(
		List<float> current, int[,] tileMap, float[,] hMap, float[] costs, float ex, float ey,
		List<List<float>> openList, List<List<float>> closedList, bool diagonalAllowed){

		//break boolean
		bool eop = false;

		//cycle neighbors
		for(int y = -1; y < 2; ++y){
			for(int x = -1; x < 2; ++x){

				//ignore current and diagnol
				if((y == 0 && x == 0) || ((x != 0 && y!=0))){
					continue;
				}

				//initialize neighbor coordinates
				int nx = (int)current[0] + x;
				int ny = (int)current[1] + y;

				//Debug.Log ("Checking ("+current[0]+", "+current[1]+ ") neighbor at: (" + nx + ", " + ny +") | Looking for ("+ex+", "+ey+")");

				//check if neighbor is end tile
				eop = (nx == ex && ny == ey);

				//Ensure coordinates are not outside bounds, and not end of path
				if (eop) {

					//create final list
					List<float> end = GetFloatList(
						nx, ny, current[0], current[1],
						costs[tileMap[ny,nx]],
						hMap[ny,nx],
						costs[tileMap[ny,nx]] +	hMap[ny,nx]);

					//add to list and return
					openList.Insert(0,end);
					return openList;

				}else if(!(nx < 0 || ny < 0 || nx >= tileMap.GetLength(1) || ny >= tileMap.GetLength(0))){

					//create list for neighbor
					List<float> neighbor = GetFloatList(
						nx, ny, current[0], current[1],
						costs[tileMap[ny,nx]],
						hMap[ny,nx],
						costs[tileMap[ny,nx]] +	hMap[ny,nx]);
					
					//check if neighbor is already included in open list
					int openUpdate = CheckList(neighbor, openList);
					//int closedUpdate = CheckList (neighbor, closedList);

					//update lists as needed
					openList = (openUpdate < 0) ? openList : UpdateList(neighbor,openList, openUpdate);
					//closedList = (closedUpdate < 0) ? closedList : UpdateList(neighbor,closedList, closedUpdate);

					//if no list update, and not currently on the closed list
					if(openUpdate < 0 && !OnList(neighbor,closedList)){

						//add list to open
						openList.Add(neighbor);

						//sort list
						openList.Sort( new NodeCompare());
					}
				}
			}
		}

		//return new open list
		return openList;
	}
		
	//Convert float list to Vector2 list
	private List<List<Vector2>> FloatListToVecList(List<List<float>> list){
		List<List<Vector2>> ret = new List<List<Vector2>> ();
		for( int i = 0; i < list.Count; ++i){
			ret.Add (FloatToVec(list[i]));
		}
		return ret;
	}

	//convert floats to vectors
	private List<Vector2> FloatToVec(List<float> list){
		List<Vector2> vList = new List<Vector2> ();
		Vector2 v1 = new Vector2 (list [0], list [1]);
		Vector2 v2 = new Vector2 (list [2], list [3]);
		vList.Add (v1);
		vList.Add (v2);
		return vList;
	}

	//create final Path
	private List<Vector2> SetFinalPath(List<List<Vector2>> closed, List<Vector2> final, Vector2 start){

		List<Vector2> ret = new List<Vector2> ();
		ret.Add (final [0]);
		Vector2 parent = final [1];

		//infinate loop
		//cycle backwards through closed list to find parents until start is found
		int hard = 0;
		while(ret[0] != start || hard == 100){

			//Cycle backwards
			for(int i = closed.Count-1; i >=0 ; --i){

				//check if current matches parent
				if(closed[i][0] == parent){

					//add current to list, update parent, and break
					ret.Insert(0,closed[i][0]);
					parent = closed [i] [1];
					break;
				}
			}
			++hard;
		}

		return ret;
	}

	//TEMP//

	private void PrintList(string s, List<List<float>> list){

		Debug.Log (s);

		for (int i = 0; i < list.Count; ++i) {
			Debug.Log ( "("+
				list [i] [0] + ", " +
				list [i] [1] + ") | (" +
				list [i] [2] + ", " +
				list [i] [3] + ") | " +
				list [i] [4] + " | " +
				list [i] [5] + " | " +
				list [i] [6]);
		}
	}

	private void PrintPath(List<Vector2> path){
		Debug.Log ("FINAL PATH: ");
		for(int i = 0; i < path.Count; ++i){
			Debug.Log (path [i]);
		}
	}
		
}
									
public class NodeCompare : IComparer<List<float>>{

	//compare float lists based on f value
	public int Compare(List<float> a, List<float> b){

		//Debug.Log ("Comparing: " + a [6] + " to " + b [6]);

		//Check if f value is equal
		if(a[6] == b[6]){

			//compare based on move costs
			if(a[4] == b[4]){

				//completely equal (somehow)
				return 0;

			}else if( a[4] < b[4] ){

				//b has higher move cost
				return -1;
			}

		}else if(a[6] < b[6]){

			//b has higher f value
			return -1;
		}

		// a has lower f value or lower move cost
		return 1;
	}
}