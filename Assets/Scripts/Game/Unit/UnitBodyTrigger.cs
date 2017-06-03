using UnityEngine;
using System.Collections;

public class UnitBodyTrigger : MonoBehaviour
{
    //cross class variables
    public Generator map;
    public Unit unit;
    public bool active = false;

    //Initialize body trigger
    public void Init(Unit u, Generator gen)
    {
        map = gen;
        unit = u;
        active = true;
    }


    //on Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        //confirm trigger active
        if (active)
        {

            //chek if unit has entered a new tile
            if (isTile(other))
            {
                //update unit location and movement
                Tile t = GetTile(other);
                unit.Movement().SetNewTileLocation(t.MapLoc, t);
                unit.currentTile = t;
            }
        }
    }

    //On Trigger Stay
    private void OnTriggerStay(Collider other)
    {
        //confirm trigger active
        if (active)
        {

            //check if unit still on tile
            if (isTile(other))
            {
                //set tile to occupied
                GetTile(other).occupied = true;
            }

        }
    }

    //On Trigger exit
    private void OnTriggerExit(Collider other)
    {
        //confirm trigger active
        if (active)
        {

        }
    }
    

    //Check if collider is a tile
    private bool isTile(Collider other)
    {
        return (other.gameObject.layer == 12 || other.gameObject.layer == 13);
    }

    //get tile object
    private Tile GetTile(Collider other)
    {
        return other.gameObject.GetComponent<Tile>();
    }
}
