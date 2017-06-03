using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MoveWith : MonoBehaviour
{

    public List<Collider> colliders;
    public List<GameObject> colliderTarget;


    private void FixedUpdate()
    {
        //Cycle Colliders to match targets
        for(int i = 0; i < colliders.Count; ++i)
        {
            colliders[i].gameObject.transform.position = colliderTarget[i].transform.position;
        }
    }

}
