using UnityEngine;
using System.Collections.Generic;

public class UnitHealing : MonoBehaviour
{
    //Cross Class
    private STDMath stdMath;
    private Overlord overlord;
    private Generator map;
    private Unit unit;

    //Bools
    public bool startedRecover;
    public bool isHealing;
    public bool isRecovering;

    //variables
    public int costToRecover;
    public List<Tile> tilesToHeal = new List<Tile>();

    //healing update
    public void HealingUpdate()
    {

        //set cost to recover
        costToRecover = Mathf.FloorToInt(unit.CurrentStats().maxHP * 0.1f);

        //check if unit is recovering
        if (CanRecover())
        {
            Recover();
        }
    }

   

    //Recover Unit HP
    private void Recover()
    {
        //check if unit still needs to recover
        if (NeedsRecovery())
        {
            //unit still needs to recover, get next recovery amount
            int recoverBy = Mathf.RoundToInt(unit.CurrentStats().maxHP / unit.Movement().Speed());

            //check against maxHP
            if(unit.CurrentStats().HP + recoverBy >= unit.CurrentStats().maxHP)
            {
                //would exceed, set to max and stop recovery
                unit.CurrentStats().HP = unit.CurrentStats().maxHP;
                StopRecovery();
            }
            else
            {
                //add recovery amount to HP
                unit.CurrentStats().HP += recoverBy;
            }
        }
    }

    //Start Recovery
    public void StartRecovery()
    {
        //check if unit needs recovery
        if (NeedsRecovery())
        {
            //check if unit has already started recovery
            if (!startedRecover)
            {
                //check if tile is being healed
                if (unit.currentTile.isBeingHealed)
                {
                    //check if availible gold
                    if (overlord.gold >= costToRecover)
                    {
                        //Gold availible, subtract then check is unit moving
                        overlord.SubtractGold(costToRecover);

                        if (unit.Movement().IsMoving())
                        {
                            //Unit is moving, stop all movements
                            unit.Movement().StopMovement();
                        }

                        //start recovering
                        startedRecover = true;
                        isRecovering = true;
                    }

                }
            }
        }
    }

    //Check if unit needs recovery
    public bool NeedsRecovery()
    {
        return (unit.CurrentStats().HP < unit.CurrentStats().maxHP);
    }

    //check if unit can still recover
    private bool CanRecover()
    {
        //check if currently recovering
        if (isRecovering)
        {
            //check if tyile is still being healed
            if (unit.currentTile.isBeingHealed)
            {
                //unit can still recover
                return true;
            }
            else
            {
                //tile nologer being healed, stop recovery
                StopRecovery();
            }
        }

        //unit is not recovering
        return false;
    }

    //End Recovery
    public void StopRecovery()
    {
        startedRecover = false;
        isRecovering = false;
    }
    
    //Begin Healing tiles
    public void StartHealing()
    {
        //check if unit is already healing
        if (!isHealing)
        {

            //check if unit is moving
            if (unit.Movement().IsMoving())
            {
                //Unit is moving, stop all movements
                unit.Movement().StopMovement();
            }

            //get tiles to heal
            tilesToHeal.Clear();
            tilesToHeal.Add(unit.currentTile);
            for(int i = 0; i < unit.currentTile.neighbors.Count; ++i)
            {
                tilesToHeal.Add(unit.currentTile.neighbors[i].GetComponent<Tile>());
            }

            //start healing
            SetHealing(true);
            isHealing = true;
        }
    }

    //End Heailing
    public void EndHealing()
    {
        isHealing = false;
        SetHealing(false);
    }

    //Set tiles healing status
    private void SetHealing(bool b)
    {
        for(int i = 0; i < tilesToHeal.Count; ++i)
        {
            if (b)
            {
                tilesToHeal[i].AddHealer(unit);
            }
            else
            {
                tilesToHeal[i].RemoveHealer(unit);
            }
        }
    }

}
