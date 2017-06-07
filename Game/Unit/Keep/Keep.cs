using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class Keep : Unit
{

    public KeepUpgrade upgrades;


    //TEST
    public void Test()
    {
        Debug.Log(this.gameObject + " is testing");
    }

    //class constructor
    public override void Init(Generator mainmap, Overlord overlor, Vector2 loc)
    {

        //initialize unit
        base.Init(mainmap, overlor, loc);

        //Set scout base information
        SetUnitInformation();

        //initialize support classes
        InitSupportClasses();

        Debug.Log("init all keep classes");
    }



    //Set unit information - Used to set bas information for unit type 
    private void SetUnitInformation()
    {

        //standard information
        baseStats.offenseDefenseID = 0;
        baseStats.unitID = 1;
        baseStats.type = "Camp";
        baseStats.cost = 0;
        baseStats.showHire = true;

        //HP
        baseStats.HP = baseStats.maxHP = 10;

        //Sight & Speed & movement
        baseStats.sight = 10;
        baseStats.speed = -1;
        baseStats.moveCosts = new float[] { 1, 1, 2, 2, 3, 3, 4, 4 };

        //Combat
        baseStats.attack = 0;
        baseStats.defense = 2;
        baseStats.siegeMod = 1;
        baseStats.evade = 0.1f;
        baseStats.isRanged = true;
        
        //Set current Stats
        currentStats = baseStats;
    }


}
