using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antiplayer : Overlord {

	//Debug Controls
	public bool debugOn = false;

	//external classes
	private AIDefense AIDef = new AIDefense();

	//Shorthands
	public enum AIDifficulty {	Caboose, Grif, Tucker, Church, Washington, Tex, Carolina, Epsilon, Alpha	};

    //AI personality Variables
    public float STR; // A measure of aggresivness. 0: No Agro. 1: Full Agro
    public float DEX; // A measure of flexibilty in strategy. 0: Single strat only. 1: Random/No Strat
    public float WIS; // Depth of AI memory. 0: no memory. 1: Infinite memeory
    public float INT; // Number of Actions/sec
    public float CON; // Potential for error

    //Critical Variables
    public int offenseRatio;
	public int defenseRatio;
	public int[] tileMod;

	//ACtion variables
	public int numActions;
	public int actionsTaken;


	//Private values
	private int[] offenseMod = { 0, 0, 5, 3, 7, 4, 10, 7, 3 };
	private int[] defenseMod = { 10, 10, 3, 5, 3, 3, 0, 0, 3 };


	public void InitializeAI(){

        //initialize random personality
        STR = Random.Range(0.0f, 1.0f);
        DEX = Random.Range(0.0f, 1.0f);
        WIS = Random.Range(0.0f, 1.0f);
        INT = Random.Range(0.0f, 1.0f);
        CON = Random.Range(0.0f, 1.0f);





        //Set Offense and Defense ratios
        defenseRatio = (onDefense)? Random.Range(5,10) : -1;
		offenseRatio = (onDefense) ? -1 : Random.Range (5, 10);

		if (defenseRatio > 0) {
			offenseRatio = 10 - defenseRatio;
		} else {
			defenseRatio = 10 - offenseRatio;
		}

		//Set tile mod
		tileMod = new int[defenseMod.Length];
		for (int i = 0; i < tileMod.Length; ++i) {

			tileMod[i] = (onDefense) ? defenseMod [i] + defenseRatio : offenseMod [i] + offenseRatio;	 				
		}

		//initilaize additional scripts
		AIDef.InitDefense(stdMath, map, this);

	}


	//Main update method
	new void FixedUpdate(){

		//utilize class update
		base.FixedUpdate();

		//Setup
		if (gameState == 0) {
			if (onDefense) {
				AIDef.EstablishKeep ();
			} else {
				
			}
		} 

		//Pre Game
		else if (gameState == 1) {
			if (onDefense) {
				
			} else {
				
			}
		}

		//Game updates
		else if (gameState == 2) {
			if (onDefense) {
				
			} else {
				
			}
		}

	}


	/* ===================================================================================================================================
	 * 
	 * 										Setup Functions
	 * 			 					Functions used to set up defense and Offense
	 *=================================================================================================================================== */


}
