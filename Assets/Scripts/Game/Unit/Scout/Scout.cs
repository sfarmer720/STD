using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scout : Unit {
	
	public ScoutUpgrade upgrades;

    //Base unit information: Scout, Ranger, Medic, Rouge, Grenadier, Engineer
    private Vector3[] UNITID =  { new Vector3(0, 1, 0),     new Vector3(0, 1, 1),   new Vector3(0, 1, 2),   new Vector3(0, 1, 3),   new Vector3(0, 1, 4),   new Vector3(0, 1, 5)    };
    private string[] TYPE =     { "Scout",                  "Ranger",               "Medic",                "Rouge",                "Grenadier",            "Engineer"              };
    private bool[] RANGED =     { false,                    false,                  false,                  false,                  false,                  false                   };
    private int[] COST =        { 1,                        10,                     10,                     10,                     50,                     10                      };
    private int[] MAXHP =       { 10,                       10,                     10,                     10,                     10,                     10                      };
    private int[] SIGHT =       { 7,                        10,                     5,                      6,                      5,                      7                       };
    private int[] SPEED =       { 10,                       10,                     6,                      12,                     8,                      8                       };
    private int[] ATTACK =      { 0,                        0,                      0,                      0,                      0,                      0                       };
    private int[] DEFENSE =     { 0,                        2,                      0,                      1,                      10,                     4                       };
    private int[] RANGE =       { 1,                        1,                      1,                      1,                      1,                      1                       };
    private float[] SIEGE =     { 1,                        0.5f,                   0.5f,                   0.5f,                   4,                      2                       };
    private float[] EVADE =     { 0.1f,                     0.15f,                  0.05f,                  0.25f,                  0.05f,                  0.1f                    };
    private float[][] MOVECOST = { 
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 },
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 },
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 },
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 },
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 },
        new float[]{ 1, 1, 2, 2, 3, 3, 4, 4 }
    };
    private int[] NUMCLASSUP = { 5, 5, 5, 5, 5 };

    //upgrade variables
    public bool reduceCost = false;
    public bool ignoreTrap = false; 

	//class constructor
	public override void Init(Generator mainmap, Overlord overlor, Vector2 loc){

		//initialize unit
		base.Init (mainmap, overlor, loc);

		//Set scout base information
		SetUnitInformation ();

		//initialize support classes
		InitSupportClasses ();

	}



	//Set unit information - Set base information based on current class
	private void SetUnitInformation(){

        //Set all Class info
        unitAI = 4;

        //Create Unit Classes
        unitClasses = new UnitInfo[UNITID.Length];
        for(int i = 0; i < unitClasses.Length; ++i)
        {
            //create new base stats
            UnitInfo ui = new UnitInfo();

            //standard information
            ui.classUpgrades = upgrades.InitClassUpgrade(NUMCLASSUP[i], overlord, this);
            ui.offenseDefenseID = (int)UNITID[i].x;
            ui.unitID = (int)UNITID[i].y;
            ui.unitClass = i;
            ui.type = TYPE[i];
            ui.cost = COST[i];
            ui.numUnits = 1;

            //HP
            ui.HP = baseStats.maxHP = MAXHP[i];

            //Sight & Speed & movement
            ui.sight = SIGHT[i];
            ui.speed = SPEED[i];
            ui.moveCosts = MOVECOST[i];

            //Combat
            ui.attack = ATTACK[i];
            ui.defense = DEFENSE[i];
            ui.siegeMod = SIEGE[i];
            ui.evade = EVADE[i];
            ui.isRanged = RANGED[i];
            ui.range = RANGE[i];
            ui.canDetect = true;
            ui.detection = 0.1f;

            //set to class array
            unitClasses[i] = ui;
        }

        //set base class
        baseStats = unitClasses[currentClass];
        upgrades.SetClassUpgrades(unitClasses[currentClass].classUpgrades);

        //Initialize Upgrade
        upgrades.Init(this, overlord, stdMath);

		//Set current Stats
		currentStats = baseStats;
	}


    /* ===================================================================================================================================
    * 
    * 									Scout Overrides
    * 			 			Functions from parent class to be overriden
    *=================================================================================================================================== */

    //Get Upgrades
    public override Upgrade GetUpgrades()
    {
        return upgrades;
    }

    //change class
    public override void ChangeClass(int i, int cost = 0)
    {
        //set cost
        cost = (reduceCost) ? Mathf.CeilToInt(COST[i] * 0.6f) : COST[i];

        //apply changes
        base.ChangeClass(i, cost);
    }

    //Take Damage
    public override void Damage(int dmg, int dmgType)
    {
        //check if ignoring trap damage for all but grenadier
        if (ignoreTrap && dmgType == 2 && currentClass != 4) { }
        else
        {
            base.Damage(dmg, dmgType);
        }
    }
}
