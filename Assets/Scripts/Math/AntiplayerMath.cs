using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiplayerMath {

	//Set Unit Math seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}


	//Find best location for keep TODO: could be done more efficiently
	public Vector2 BestKeepLocation(Generator map, int[] tileMod, int[] ignore){

		//create influence map
		//int[,] mat = new int[map.mapSize,map.mapSize];
		int best = -1;
		List<Vector2> vList = new List<Vector2> ();

		//cycle map
		for (int y = 0; y < map.mapSize; ++y) {
			for (int x = 0; x < map.mapSize; ++x) {

				//get tile
				Tile t = map.GetTile(x,y).GetComponent<Tile>();

				//confirm tile is not ignorable
				bool ig = false;
				for (int i = 0; i < ignore.Length; ++i) {
					if (t.tileType == ignore [i]) {
						ig = true; break;
					}
				}
				if (ig) {
					continue;
				} else {
				
					//cycle surrounding tiles and add influence
					int influ = tileMod[t.tileType];
					for (int i = 0; i < t.neighbors.Count; ++i) {
						influ += tileMod [t.neighbors [i].GetComponent<Tile> ().tileType];
					}

					//set influence map & check if higher influence than before
					//mat[y,x] = influ;

					if (influ > best) {
						vList.Clear ();
						best = influ;
						vList.Add (new Vector2 (x, y));
					} else if (influ == best) {
						vList.Add (new Vector2 (x, y));
					}

				}
			}
		}

		//check if more than 1 best tile
		if (vList.Count > 1) {

			//more than 1 best tile, choose randomly
			return vList [Random.Range (0, vList.Count - 1)];
		} else if (vList.Count > 0) {

			//only 1 best tile
			return vList [0];
		} else {

			//Something errored in the check.
			return new Vector2(-1,-1);
		}

	}




}
