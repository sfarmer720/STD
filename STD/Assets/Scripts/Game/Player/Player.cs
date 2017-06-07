using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Overlord {

	//vital variables
	private Vector2 startingLocation;

	//initialize Player
	public void InitPlayer( STDMath math, Generator gen, Assets asset){

		//basic settings
		this.gameObject.name = "Player";


		//set starting tile
		//SetStart();

		//Add initial unit
		//CreateUnit(0,startingLocation);
	}

	//main update method
	new void FixedUpdate(){

		//run base update first
		base.FixedUpdate();

		//Setup
		if (gameState == 0) {
			if (onDefense) {
				PlayerSetupDefense ();
			} else {
				PlayerSetupOffense ();
			}
		} 

		//Pre Game
		else if (gameState == 1) {
			if (onDefense) {
				PlayerPreGameDefense ();
			} else {
				PlayerPreGameOffense ();
			}
		}

		//Game updates
		else if (gameState == 2) {
			if (onDefense) {
				PlayerMainDefense ();
			} else {
				PlayerMainOffense ();
			}
		}

        //Active Player updates
        if (activePlayer)
        {
            //Update GUI
            GUIUpdate();

        }

	}



	/* ===================================================================================================================================
	 * 
	 * 										Setup Functions
	 * 			 					Functions used to set up defense and Offense
	 *=================================================================================================================================== */

	private void PlayerSetupDefense(){

		//check if keep has been placed already
		if (units.Count < 1) {
			ClickToPlace (0);
		}

	}

	private void PlayerSetupOffense(){

	}

	/* ===================================================================================================================================
	 * 
	 * 									Pre Game Functions
	 * 			 			Functions to set pre game settings
	 *=================================================================================================================================== */

	private void PlayerPreGameDefense(){

	}

	private void PlayerPreGameOffense(){


		guiCon.SetTimerMessage ("Scouting time remaining", preGameEndTime, gameTime);

		//check if scout has been placed already
		//TODO: allow for multiple scouts placed
		if (units.Count < 1) {
			ClickToPlace (0);
		}
	}


	/* ===================================================================================================================================
	 * 
	 * 										Main Game Functions
	 * 			 					Functions used to run the primary game
	 *=================================================================================================================================== */

	private void PlayerMainDefense(){

	}

	private void PlayerMainOffense(){
		if (units.Count < 1) {
			guiCon.ClearMessageText ();
			ClickToPlace (1);
		}
	}


    /* ===================================================================================================================================
	 * 
	 * 										GUI Functions
	 * 			 					Functions used to set and modify GUI
	 *=================================================================================================================================== */

    //Camp Control Updates
    private void GUIUpdate()
    {
        //check if any units are selected
        if (controls.select.IsUnitSelected())
        {
            //set to controls
            guiCon.SetCampControls(1);

            //check if unit is in group
            if (controls.select.UnitInGroup(controls.select.GetUnit(controls.select.currentSelection)))
            {
                //group actions
            }
            else
            {
                //Set to individual unit controls
                
            }
        }
        else
        {
            //no units or groups selected, default camp controls, close unit controls
            guiCon.SetCampControls(2);

        }
    }
}
