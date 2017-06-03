using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScoutUpgrade : Upgrade
{

    /* Upgrades left:
     * 
     * Ranger: 2,3
     * 
     * 
     */ 

    //Initializer
    public override void Init(Unit u, Overlord o, STDMath m)
    {
        base.Init(u, o, m);

        //set unit upgrade actions
        for(int i = 0; i < numUnitUpgrades; ++i)
        {
            SetUnitUpgradeAction(i, GetUnitAction(i, true), GetUnitAction(i, false));
        }

    }

    //Get Unit as specified Class
    private Scout GetUnit()
    {
        return unit as Scout;
    }

    /* ===================================================================================================================================
    * 
    * 									Scout Unit Upgrades
    * 			 			Upgrades that apply to all units and all classes of scouts
    *=================================================================================================================================== */
    
        //Action returner
    public UnityAction GetUnitAction(int i, bool active)
    {
        if (active)
        {
            switch (i)
            {
                case 0: return () => { Active_UU_1(); };
                case 1: return () => { Active_UU_2(); };
                case 2: return () => { Active_UU_3(); };
                case 3: return () => { Active_UU_4(); };
                case 4: return () => { Active_UU_5(); };
            }
        }
        else
        {
            switch (i)
            {
                case 0: return () => { Deactive_UU_1(); };
                case 1: return () => { Deactive_UU_2(); };
                case 2: return () => { Deactive_UU_3(); };
                case 3: return () => { Deactive_UU_4(); };
                case 4: return () => { Deactive_UU_5(); };
            }
        }

        //errored
        return null;
    }

    //Upgrade 1: 
    //Spot the Difference - Adds 20% to trap detection
    public void Active_UU_1()
    {
        unit.BaseStats().detection += 0.2f;
        unit.CurrentStats().detection += 0.2f;
    }
    public void Deactive_UU_1()
    {
        unit.BaseStats().detection -= 0.2f;
        unit.CurrentStats().detection -= 0.2f;
    }

    //Upgrade 2: 
    //Great Minds - Nullifies trap damage, excluding grenadiers
    public void Active_UU_2()
    {
        GetUnit().ignoreTrap = true;
    }
    public void Deactive_UU_2()
    {
        GetUnit().ignoreTrap = false;
    }

    //Upgrade 3: 
    //Jack of All Trades - Reduces class change cost by 60%
    public void Active_UU_3()
    {
        GetUnit().reduceCost = true;
    }
    public void Deactive_UU_3()
    {
        GetUnit().reduceCost = false;
    }

    //Upgrade 4:    
    //Hunter of Achievement - Adds 10HP
    public void Active_UU_4()
    {
        for(int i = 0; i < unit.unitClasses.Length; ++i)
        {
            unit.unitClasses[i].maxHP += 10;
            unit.unitClasses[i].HP += 10;
        }
        unit.CurrentStats().maxHP += 10;
        unit.CurrentStats().HP += 10;
    }
    public void Deactive_UU_4()
    {
        for (int i = 0; i < unit.unitClasses.Length; ++i)
        {
            unit.unitClasses[i].maxHP -= 10;
            unit.unitClasses[i].HP -= 10;
        }
        unit.CurrentStats().maxHP -= 10;
        unit.CurrentStats().HP -= 10;
    }

    //Upgrade 5:    
    //Caltrops - Increases Movement cost of enemies in surrounding tiles
    public void Active_UU_5()
    {

    }
    public void Deactive_UU_5()
    {

    }

    /* ===================================================================================================================================
    * 
    * 									Scout Class Upgrades
    * 			 			Upgrades that apply to only the scout class
    *=================================================================================================================================== */
    //Upgrade 1: 
    //20/20 - +1 sight
    public void Active_C0_1()
    {
        ++unit.CurrentStats().sight;
    }
    public void Deactive_C0_1()
    {
        --unit.CurrentStats().sight;
    }

    //Upgrade 2:
    //Run Forest - +1 Speed
    public void Active_C0_2()
    {
        ++unit.CurrentStats().speed;
    }
    public void Deactive_C0_2()
    {
        --unit.CurrentStats().speed;
    }

    //Upgrade 3:
    //On the Lam - +5% evade
    public void Active_C0_3()
    {
        unit.CurrentStats().evade += 0.05f;
    }
    public void Deactive_C0_3()
    {
        unit.CurrentStats().evade -= 0.05f;
    }

    //Upgrade 4:
    //Double Time - +1 Speed
    public void Active_C0_4()
    {
        ++unit.CurrentStats().speed;
    }
    public void Deactive_C0_4()
    {
        --unit.CurrentStats().speed;
    }

    //Upgrade 5:
    //High Ground - +1 sight
    public void Active_C0_5()
    {
        ++unit.CurrentStats().sight;
    }
    public void Deactive_C0_5()
    {
        --unit.CurrentStats().sight;
    }

    /* ===================================================================================================================================
    * 
    * 									Ranger Class Upgrades
    * 			 			Upgrades that apply to only the Ranger class
    *=================================================================================================================================== */
    //Upgrade 1:
    //To Isengard - 2x Movement in Open terrain
    public void Active_C1_1()
    {
        unit.CurrentStats().moveCosts[0] /= 2;
        unit.CurrentStats().moveCosts[1] /= 2;
    }
    public void Deactive_C1_1()
    {
        unit.CurrentStats().moveCosts[0] *= 2;
        unit.CurrentStats().moveCosts[1] *= 2;
    }

    //Upgrade 2:
    //Purple Mountains - no sight limitations from hills or mountains
    public void Active_C1_2()
    {

    }
    public void Deactive_C1_2()
    {

    }

    //Upgrade 3:
    //Guerilla Warfare - 3x trap damage
    public void Active_C1_3()
    {

    }
    public void Deactive_C1_3()
    {

    }

    //Upgrade 4:
    //30 Years Later: +10% evade
    public void Active_C1_4()
    {
        unit.CurrentStats().evade += 0.1f;
    }
    public void Deactive_C1_4()
    {
        unit.CurrentStats().evade -= 0.1f;
    }

    //Upgrade 5:
    //Detect Trap - +30% chance to detect terrain trap
    public void Active_C1_5()
    {
        unit.CurrentStats().detection += 0.3f;
    }
    public void Deactive_C1_5()
    {
        unit.CurrentStats().detection -= 0.3f;
    }

    /* ===================================================================================================================================
    * 
    * 									Medic Class Upgrades
    * 			 			Upgrades that apply to only the medic class
    *=================================================================================================================================== */
    //Upgrade 1:
    //Iodine - 2x Healing
    public void Active_C2_1()
    {

    }
    public void Deactive_C2_1()
    {

    }

    //Upgrade 2:
    //Seacoled - 1/2 cost to heal
    public void Active_C2_2()
    {

    }
    public void Deactive_C2_2()
    {

    }

    //Upgrade 3:
    //Field Amputations - Immediately heal unit, but reduces unit MaxHP by 1/5
    public void Active_C2_3()
    {

    }
    public void Deactive_C2_3()
    {

    }

    //Upgrade 4:
    //Snowbody Knows - Immediately stop fire and poison damage
    public void Active_C2_4()
    {

    }
    public void Deactive_C2_4()
    {

    }

    //Upgrade 5:
    //Kavorkian - Kill Unit and return 3/4 unit cost
    public void Active_C2_5()
    {

    }
    public void Deactive_C2_5()
    {

    }

    /* ===================================================================================================================================
    * 
    * 									Rouge Class Upgrades
    * 			 			Upgrades that apply to only the Rouge class
    *=================================================================================================================================== */
    //Upgrade 1:
    //Deadly Effect - 2x trap/bomb damage
    public void Active_C3_1()
    {

    }
    public void Deactive_C3_1()
    {

    }

    //Upgrade 2:
    //Poison Tipped - Traps now deal poison damage
    public void Active_C3_2()
    {

    }
    public void Deactive_C3_2()
    {

    }

    //Upgrade 3:
    //Decoy - +10% evade
    public void Active_C3_3()
    {

    }
    public void Deactive_C3_3()
    {

    }

    //Upgrade 4:
    //Thief - Enemies killed in sight give 1.5x gold
    public void Active_C3_4()
    {

    }
    public void Deactive_C3_4()
    {

    }

    //Upgrade 5:
    //Detect Trap - +30% detection
    public void Active_C3_5()
    {

    }
    public void Deactive_C3_5()
    {

    }

    /* ===================================================================================================================================
    * 
    * 									Scout Class Upgrades
    * 			 			Upgrades that apply to only the scout class
    *=================================================================================================================================== */
    //Upgrade 1:
    //Poison Bomb - Deals no damage, but poisons units
    public void Active_C4_1()
    {

    }
    public void Deactive_C4_1()
    {

    }

    //Upgrade 2:
    //Smoke Bomb - obscures small area
    public void Active_C4_2()
    {

    }
    public void Deactive_C4_2()
    {

    }

    //Upgrade 3:
    //Shrapnel - all bombs now deal splash damage
    public void Active_C4_3()
    {

    }
    public void Deactive_C4_3()
    {

    }

    //Upgrade 4:
    //Bomb Squad - +70Def, 1/4 Speed, 20X Damage From traps
    public void Active_C4_4()
    {

    }
    public void Deactive_C4_4()
    {

    }

    //Upgrade 5:
    //Kamikaze - Kills Unit but deals 10X damage
    public void Active_C4_5()
    {

    }
    public void Deactive_C4_5()
    {

    }

    /* ===================================================================================================================================
    * 
    * 									Scout Class Upgrades
    * 			 			Upgrades that apply to only the scout class
    *=================================================================================================================================== */
    //Upgrade 1:
    //Mathematical Hannibal - 2x movement speed for siege engines in rough terrain 
    public void Active_C5_1()
    {

    }
    public void Deactive_C5_1()
    {

    }

    //Upgrade 2:
    //Trigonometry - increase siege engines chances to beat enemy evade
    public void Active_C5_2()
    {

    }
    public void Deactive_C5_2()
    {

    }

    //Upgrade 3:
    //Under Warrenty - 2X heal for Siege Engines
    public void Active_C5_3()
    {

    }
    public void Deactive_C5_3()
    {

    }

    //Upgrade 4:
    //Hit them where it hurts - 2x siege damage
    public void Active_C5_4()
    {

    }
    public void Deactive_C5_4()
    {

    }

    //Upgrade 5:
    //Tech Support: 2X heal speedo
    public void Active_C5_5()
    {

    }
    public void Deactive_C5_5()
    {

    }
}
