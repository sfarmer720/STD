using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSight : MonoBehaviour {

    private SphereCollider sightSphere;
    private Unit unit;
    private Overlord overlord;
    private float radius;
    private int tileWidth;

	//Initialize class
    public void InitSight(Unit u, Overlord over, SphereCollider sight, float r, int tilewide)
    {
        unit = u;
        overlord = over;
        radius = r;
        tileWidth = tilewide;
        sightSphere = sight;
    }

    //Sight Updates
    public void UpdateSight()
    {

        //set sight sphere radius
        if (sightSphere != null)
        {
            sightSphere.radius = radius * tileWidth * 0.5f;
        }
    }

    
    //Update Tile visiblity
    public void AddToSeen(GameObject go)
    {
        overlord.AddToSeen(go);
    }
    public void AddToLost(GameObject go)
    {
        overlord.AddToLost(go);
    }
}
