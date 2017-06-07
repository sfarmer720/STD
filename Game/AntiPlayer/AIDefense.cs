using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDefense {

	//External Classes
	public STDMath stdMath;
	public Generator map;
	public Antiplayer antiplayer;


	//Initialize Defense Script
	public void InitDefense(
		STDMath std, Generator gen, Antiplayer anti
	){

		//initialzie external classes
		map = gen;
		antiplayer = anti;
		stdMath = std;
		Random.InitState (stdMath.seed);

	}


	/* ===================================================================================================================================
	 * 
	 * 										Setup Functions
	 * 			 					Functions used to set up defense and Offense
	 *=================================================================================================================================== */

	//Establish Keep
	public void EstablishKeep(){

		//create keep at best location
		Debug.Log(antiplayer);
		antiplayer.CreateUnit(0,stdMath.BestKeepLocation(map,antiplayer.tileMod,antiplayer.getIgnorableTiles()));
	}


}
