using UnityEngine;
using System.Collections;

public class AIForebrain : MonoBehaviour
{

    //Beliefs
    public bool passive = false;    //Boolean dictating assertiveness of the AI. False: No Assertion. True: Active assertion

    //Goals
    //Life Goal - Main Goal AI Wants to achieve


    //Initial Ideas
    //Set play stlye as passive or aggressive
    public void SetAIPlayStyle(bool onDefense, float str)
    {
        if (onDefense)
        {
            passive = (Mathf.Round(str) == 1) ? true : false;
        }
    }
}
