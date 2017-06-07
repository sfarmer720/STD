using UnityEngine;
using System.Collections;

public class UnitAssets : MonoBehaviour
{

    //Unit Asset Struct
    public struct assetInfo
    {
        public GameObject unitObj;
        public GameObject[] unitClass;
        public Animator[] unitAnim;
        public CharacterController con;
    }

    //Constants
    public SphereCollider sightSphere;
    public string[] animVars;
    public GameObject[] unitObjs;
    public IndividualUnitAssets[] individualUnits;
    public assetInfo[] unitInfo;

    //Change variables
    public int activeClass;
    public bool[] activeUnits;

    //Initialize
    private void Awake()
    {
        //Initialize variables
        activeClass = 0;
        unitInfo = new assetInfo[unitObjs.Length];
        activeUnits = new bool[unitObjs.Length];
        Debug.Log(unitObjs.Length);

        //create struct arrays
        for(int i = 0; i < unitObjs.Length; ++i)
        {
            //create new struct
            assetInfo ai = new assetInfo();
            ai.unitObj = unitObjs[i];
            ai.unitClass = individualUnits[i].unitTypes;
            ai.unitAnim = individualUnits[i].unitAnims;
            ai.con = individualUnits[i].controller;
            unitInfo[i] = ai;

            //Set active
            activeUnits[i] = true;
        }

        //Set only active class active
        SetActiveClass(activeClass);
    }


    //Set active class
    //TODO: allow for class change animations
    public void SetActiveClass(int i)
    {
        activeClass = i;

        //cycle all units
        for(int ui = 0; ui<unitInfo.Length; ++ui)
        {

            //cycle all classes
            for(int c = 0; c < unitInfo[ui].unitClass.Length; ++c)
            {
                if(c == i && activeUnits[ui])
                {
                    unitInfo[ui].unitClass[c].SetActive(true);
                }
                else
                {
                    unitInfo[ui].unitClass[c].SetActive(false);
                }
            }

        }
    }

}
