using UnityEngine;
using System.Collections.Generic;

public class UnitFormation : MonoBehaviour
{
    /*
     * Sets Units placement within main unit object
     * 
     * Formations are saved as 3D booleans. 1D dictates the formation, 2D/3D correspond with x/y coordinates
     * 
     */

    //Cross class variables
    protected STDMath stdMath;

    //Formation Variables
    public UnitAssets assets;
    public int maxUnits = 0;
    protected int currentFormation = 0;
    protected List<bool[,]> formations = new List<bool[,]>(5);
    private Vector3[,] formationVectors;
    private Vector3[] formationPositions;


    //Initialize
    public void Init(STDMath std)
    {
        //Set Cross class
        stdMath = std;
        Debug.Log(this);
        maxUnits = assets.unitInfo.Length;

        //set number of formation postions
        formationPositions = new Vector3[maxUnits];

        //initialize formation vectors
        formationVectors = new Vector3[5, 5];
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //create new vector with 0 center and +/- 2 deviance
                Vector3 v = new Vector3((2 * (x - 2)), 0, (2 * (y - 2)));
                formationVectors[y, x] = v;
            }
        }

        //set default formation
    }

    //Check position
    public bool CheckPosition()
    {
        //cycle all active units
        for(int i =0; i < assets.unitInfo.Length; ++i)
        {
            //check if unit is active
            if (assets.unitInfo[i].unitObj.activeSelf)
            {
                //check if in position
                if(assets.unitInfo[i].unitObj.transform.position != formationPositions[i])
                {
                    //unit is out of position, check full position
                    return true;
                }
            }
        }
        //all units in position return false
        return false;
    }

    //Check formation
    public void CheckFormation(float speed)
    {

        //check if any unit is out of position
        if (CheckPosition())
        {

            // unit is out of position, cycle formation
            for (int y = 0, i = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    //check if vaild spot
                    if (formations[currentFormation][y, x])
                    {
                        //valid location, get unit transform and position
                        Transform unit = assets.unitInfo[i].unitObj.transform;
                        Vector3 unitPos = unit.position;

                        //get group position and intended formation position
                        Vector3 groupPos = this.gameObject.transform.position;
                        Vector3 formationPos = formationVectors[y, x] + groupPos;

                        //set global formation position
                        formationPositions[i] = formationPos;

                        //check if unit position matches formation position
                        if (unitPos != formationPos)
                        {
                            //unit is not yet in place, look at correct location and move towards
                            unit.LookAt(formationPos);
                            assets.unitInfo[i].con.Move(stdMath.MoveVec(unit.forward, speed));
                        }
                        else
                        {
                            //unit is in postion, ensure unit is looking at container forward
                            unit.LookAt(this.gameObject.transform.forward);
                        }

                        //increment for next unit
                        ++i;
                    }

                }
            }
        }
    }


    //Set a formation
    public void SetFormation(bool[,] form, int formNum)
    {
        //cycle form
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //update formation
                formations[formNum][y, x] = form[y, x];
                
            }
        }
    }

    //get formation
    public List<bool[,]> GetAllFormations()
    {
        return formations;
    }
    public bool[,] GetFormation(int i)
    {
        return formations[i];
    }
}
