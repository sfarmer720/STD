using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Upgrade:MonoBehaviour
{
    //cross class
    public STDMath stdMath;
    public Overlord overlord;
    public Unit unit;

    //Upgrade arrays
    public int numUnitUpgrades;
    public int numClassUpgrades;
    public int[] unitUpgradeCosts;
    public int[] classUpgradeCosts;
    public UnitUpgrade[] unitUpgrades;
    public ClassUpgrade[] classUpgrades;


    //Default Images
    public Sprite defaultImage1;
    public Sprite defaultImage2;

    //Unit Images
    public Sprite mainImage;
    public Sprite[] classImages;
    public Sprite[] classUpgradeImages;
    public Sprite[] UnitUpgradeImages;

    
    //Base initializer
    public virtual void Init(Unit u, Overlord o, STDMath m)
    {
        //Set cross class
        stdMath = m;
        overlord = o;
        unit = u;

        //Set array sizes
        unitUpgrades = new UnitUpgrade[numUnitUpgrades];
        classUpgrades = new ClassUpgrade[numClassUpgrades];

        //Create inital upgrades
        InitUnitUpgrade();
       
        
    }

    //Initialize Unit upgrades
    private void InitUnitUpgrade()
    {
        for (int i = 0; i < numUnitUpgrades; ++i)
        {
            //create new unit upgrade object
            UnitUpgrade uu = new UnitUpgrade();
            uu.overlord = overlord;
            uu.unit = unit;
            uu.ID = i;
            uu.upgrade = this;
            uu.cost = unitUpgradeCosts[i];
            uu.upgradeImage = UnitUpgradeImages[i];

            //set overlord action
            uu.overlordAction = () =>
            {
                overlord.SetUnitUpgrade(unit.BaseStats().unitID, i);
            };

            //set in array
            unitUpgrades[i] = uu;
        }
    }

    //Initialize Class upgrades
    public ClassUpgrade[] InitClassUpgrade(int num, Overlord o, Unit u)
    {
        //create new array
        ClassUpgrade[] cua = new ClassUpgrade[num];
        for (int i = 0; i < num; ++i)
        {
            //create new unit upgrade object
            ClassUpgrade cu = new ClassUpgrade();
            cu.overlord = o;
            cu.unit = u;
            cu.ID = i;
            cu.upgrade = this;
            cu.cost = classUpgradeCosts[i];
            cu.upgradeImage = classUpgradeImages[i];
            cua[i] = cu;
        }

        //return array
        return cua;
    }

    //Set unit upgrade actions
    public void SetUnitUpgradeAction(int i, UnityAction active, UnityAction deactive)
    {
        unitUpgrades[i].activeAction = active;
        unitUpgrades[i].deactiveAction = deactive;
    }

    //Apply unit upgrade
    public void DoUnitUpgrade(int i)
    {
        unitUpgrades[i].UpgradeAction();
    }
    
    //Set active class upgrades
    public void SetClassUpgrades(ClassUpgrade[] cua)
    {
        classUpgrades = cua;
    }

    //Check if upgrade can be bought
    public bool CanBuyUpgrade(bool unitUpgr, int upgr)
    {
        //Get correct list
        if (unitUpgr)
        {
            //Attempting Unit upgrade, confirm upgrade exist
            if(upgr >= 0 && unitUpgrades.Length > upgr)
            {
                //Upgrade exists, check if can be bought
                if(overlord.gold >= unitUpgrades[upgr].cost)
                {
                    //can be bought return true
                    return true;
                }
            }
        }
        else
        {
            //Attempting class upgrade, confirm upgrade exist
            if (upgr >= 0 && classUpgrades.Length > upgr)
            {
                //Upgrade exists, check if can be bought
                if (overlord.gold >= classUpgrades[upgr].cost)
                {
                    //can be bought return true
                    return true;
                }
            }
        }


        //cannot buy upgrade
        return false;
    }

    //unset Class upgrades
    public void SetAllUpgrades(bool set)
    {
        for (int u = 0; u < classUpgrades.Length; ++u)
        {
            //check if upgrade active
            if (classUpgrades[u].active)
            {
                //if setting, run active, if unsetting run deactive
                if (set)
                {
                    classUpgrades[u].activeAction();
                }
                else
                {
                    classUpgrades[u].deactiveAction();
                }
            }
        }
        for (int u = 0; u < unitUpgrades.Length; ++u)
        {
            //check if upgrade active
            if (unitUpgrades[u].active)
            {
                //if setting, run active, if unsetting run deactive
                if (set)
                {
                    unitUpgrades[u].activeAction();
                }
                else
                {
                    unitUpgrades[u].deactiveAction();
                }
            }
        }
    }

}

//Class that sets the structure of Class Changes
public class UnitClass
{

    //GUI Variables
    public Sprite upgradeImage;
    public int ID;
    public bool active = false;
    public int cost;

    public void DoAction()
    {

    }
}

//Class that sets the structure of Class Upgrades
public class ClassUpgrade
{
    //Cross Class

    public Overlord overlord;
    public Unit unit;
    public Upgrade upgrade;

    //GUI Variables
    public Sprite upgradeImage;

    //Upgrade Variables
    public int ID;
    public bool active = false;
    public int cost;
    public UnityAction activeAction;
    public UnityAction deactiveAction;

    public void DoAction()
    {
        //inverse active
        active = !active;

        //perform active or inactive action
        if (active)
        {
            //check if upgrade can be bought
            if (upgrade.CanBuyUpgrade(false, ID))
            {
                //subtract gold amount, and apply upgrade
                overlord.SubtractGold(cost); 
                activeAction();
            }
        }
        else
        {
            //return gold and deactivate
            overlord.AddGold(cost);
            deactiveAction();
        }
    }
}

//Class that sets the structure of Unit Upgrades
public class UnitUpgrade
{
    //cross Class variables
    public Overlord overlord;
    public Upgrade upgrade;
    public Unit unit;

    //GUI Variables
    public Sprite upgradeImage;

    //Upgrade Variables
    public int ID;
    public bool active = false;
    public int cost;
    public UnityAction activeAction;
    public UnityAction deactiveAction;
    public UnityAction overlordAction;

    public void DoAction()
    {
        overlordAction();

    }

    public void UpgradeAction()
    {

        //inverse active
        active = !active;

        //perform active or inactive action
        if (active)
        {
            //check if upgrade can be bought
            if (upgrade.CanBuyUpgrade(true, ID))
            {
                //subtract gold amount, and apply upgrade
                overlord.SubtractGold(cost);
                activeAction();
            }
        }
        else
        {
            //return gold and deactivate
            overlord.AddGold(cost);
            deactiveAction();
        }
    }
}


