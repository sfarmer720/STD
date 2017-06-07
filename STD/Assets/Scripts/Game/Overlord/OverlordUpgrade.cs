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

    //Unit upgrade holders
    public List<Upgrade> upgradeHold = new List<Upgrade>();

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

    //Create new unit upgrade object
    private void AddNewUpgrade(Upgrade u)
    {
        //create new upgrade object
        Upgrade upgr = new Upgrade();
        upgr.unitGroupID = u.unitGroupID;
        upgr.unitUpgrades = new UnitUpgrade[u.unitUpgrades.Length];
        
        //cycle unit upgrades
        for(int i = 0; i < u.unitUpgrades.Length; ++i)
        {
            //create new fake upgrade, add to list
            UnitUpgrade uniUp = new UnitUpgrade();
            upgr.unitUpgrades[i] = uniUp;
        }

        //add to list
        upgradeHold.Add(upgr);
    }

    //Get universal unit upgrade
    public Upgrade GetUnitUpgrade(Upgrade u)
    {
        //cycle upgrade list and look for group
        for(int i = 0; i < upgradeHold.Count; ++i)
        {
            //check for matching ids
            if(upgradeHold[i].unitGroupID == u.unitGroupID)
            {
                return upgradeHold[i];
            }
        }

        //no matching upgrade, create new and return empty
        AddNewUpgrade(u);
        return u;
    }

    //Update Unit upgrade
    public void UpdateUnitUpgrade(int groupID, int upgradeID)
    {
        //cycle saved upgrades
        for(int i = 0; i < upgradeHold.Count; ++i)
        {
            //check for matching group
            if(groupID == upgradeHold[i].unitGroupID)
            {
                //invert upgrade active status
                upgradeHold[i].unitUpgrades[upgradeID].active = !upgradeHold[i].unitUpgrades[upgradeID].active;
            }
        }
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
