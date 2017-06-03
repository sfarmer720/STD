using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class OverlordUpgrade : MonoBehaviour
{
    //external variables
    private STDMath stdMath;
    private Overlord overlord;
    private Generator map;
    public bool activePlayer;

    //Camp Control Actions
    public UnityAction[] hireActions;
    public UnityAction[] controlActions;
    public UnityAction[] specialActions;


    //number of action icons
    private int num = 5;

    //Initialize
    public void Init(bool active, STDMath math, Overlord over, Generator mp)
    {
        //Set ecternal variables
        stdMath = math;
        overlord = over;
        map = mp;
        activePlayer = active;

        //set empty action lists
        hireActions = new UnityAction[5];
        controlActions = new UnityAction[5];
        specialActions = new UnityAction[5];
    }



    /* ===================================================================================================================================
	 * 
	 * 									Actions
	 * 			 			Actions to be added or removed. 
	 *=================================================================================================================================== */

    //default hire actions
    public void DefaultHire()
    {
        //create 4 hire actions, last hire action will open special (assigned in Camp Control)
        for(int i = 0; i < 4; ++i)
        {
            hireActions[i] = () =>
            {
                CreateUnit(false, i + 1);
            };
        }
        
    }


    //Find location to spawn unit
    public void CreateUnit(bool defUnit, int unitID)
    {
        //check location availble
        Vector2 v = stdMath.CanBuildUnit(map, overlord.campLoc);
        if(v.x >=0 && v.y >= 0)
        {
            CreateUnit(defUnit, unitID, v);
        }
    }

    //Spawn unit at new location
    public void CreateUnit(bool defUnit, int unitID, Vector2 location)
    {
        //create unit game object
        GameObject unit = (defUnit) ? Instantiate(overlord.assetHolder.defenseAssets[unitID]) : Instantiate(overlord.assetHolder.offenseAssets[unitID]);

        //Set unit position and parent
        unit.transform.position = stdMath.SpawnLocation(location, map.mapSize);
        unit.transform.parent = this.gameObject.transform;

        //add unit to unit list
        overlord.units.Add(unit);

        //initialize unit
        unit.GetComponent<Unit>().Init(map, overlord, location);
    }
    
}
