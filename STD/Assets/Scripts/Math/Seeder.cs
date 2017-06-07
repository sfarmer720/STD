using UnityEngine;
using System.Collections;

public class Seeder : MonoBehaviour {

	//DEVELOPER TOOLS
	public bool showSeed;


	//create Hash
	public int hashedSeed;

	//user defined seed
	public string seed;

	//Random seed length
	[Range(5,20)]
	public int seedLength = 10;

	//Use random seed
	public bool randomSeed;


	// Perform before all other functions
	void Awake () {
	
		//check if using random seed, if seed input exist, is not black, and is not too long
		if (!(randomSeed) && seed != null && seed != "" && seed.Length <= seedLength) {

			//create hashed seed from user input
			hashedSeed = seed.GetHashCode ();

		} else {

			//reset seed
			seed = null;

			//define characters for seed
			string seedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

			//choose random character
			char c = seedChars [Random.Range (0, seedChars.Length)];

			//add first character to seed
			seed = seed + c;

			//cycle until seed is equal to seed length
			while (seed.Length < seedLength) {

				//generate new random seed
				c = seedChars [Random.Range (0, seedChars.Length)];

				//add character to seed
				seed = seed + c;
			}

			//create hashed seed
			hashedSeed = seed.GetHashCode ();
		}

		//DEVELOPER TOOLS
		if (showSeed) {
			Debug.Log (hashedSeed);
		}
	}
}
