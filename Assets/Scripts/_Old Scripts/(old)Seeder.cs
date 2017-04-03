using UnityEngine;
using System.Collections;

public class SeederX : MonoBehaviour {

	//Public seed Hash
	public int hashedSeed;

	//user defined seed
	public string seed;

	//random seed
	[Range(5,20)]
	public int seedLength = 10;
	public bool randomSeed;


	// Use this for initialization
	void Awake () {

		//check if not using random seed, seed input exists, is not blank, and is not to long
		if (!(randomSeed) && seed != null && seed != "" && seed.Length <= seedLength) {

			//set seed hash
			hashedSeed =  seed.GetHashCode();

		} else {

			//reset seed
			seed = null;

			//define characters for seed
			string seedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char c = seedChars[Random.Range(0,seedChars.Length)];


			//add first character
			seed = seed + c; 

			//randomly add characters until seed length is reached
			while(seed.Length < seedLength){
				c = seedChars[Random.Range(0,seedChars.Length)];
				seed = seed + c;
			}

			//set final seed hash
			hashedSeed =  seed.GetHashCode();
			Debug.Log (hashedSeed);
		}

	}
}
