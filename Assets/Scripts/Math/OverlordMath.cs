using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlordMath {

	//Set Unit Math seed
	public void SetSeed(int seed){
		Random.InitState (seed);
	}

	//set defense domain
	public bool[,] SetDefenseDomain(Vector2 location, Generator map){

		bool[,] ret = new bool[map.mapSize, map.mapSize];
		int start = -(map.mapSize/10);
		int end = (map.mapSize/10)+1;

		//cycle surrounding tiles
		for(int startY = start; startY < end; ++startY){
			for (int startX = start; startX < end; ++startX) {
			
				int nx = (int)location.x + startX;
				int ny = (int)location.y + startY;

				//set domain to true
				ret[ny, nx] = true;
			}
		}

		//return domain
		return ret;
	}


	//get Offense Starting tile
	public List<Vector2> OffenseStartingTile(Generator map, bool[,] domain, int[] ignore){

		List<Vector2> ret = new List<Vector2> ();
		bool[,] mat = new bool[map.mapSize, map.mapSize];

		//cycle map
		for (int y = 0; y < map.mapSize; ++y) {
			for (int x = 0; x < map.mapSize; ++x) {

				//check if map is in domain
				if (!domain [y, x]) {

					bool potentialTile = true;

					//check if ignorable tile
					for (int i = 0; i < ignore.Length; ++i) {

						if (map.GetTile (x, y).GetComponent<Tile> ().tileType == ignore [i]) {
							potentialTile = false;
						}
					}

					//add potential tile
					mat [y, x] = potentialTile;
				}
			}
		}

		//INFINATE LOOP
		//continuous cycle until number of startign tiles chosen
		while (ret.Count <= map.mapSize / 10) {

			Vector2 v = new Vector2 (Random.Range(0,map.mapSize-1), Random.Range(0,map.mapSize-1));

			if (mat [(int)v.y, (int)v.x]) {
				ret.Add (v);
			}
		}

		//return starting tiles
		return ret;
	}





}
