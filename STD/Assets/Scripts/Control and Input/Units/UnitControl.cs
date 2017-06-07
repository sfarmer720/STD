using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class UnitControl : MonoBehaviour
{

    /* TODO:
     * Main Button actions
     * 
     */

    //Constant Variables
    public Animator anim;
    public Image mainImage;
    public Color[] selectionColors;
    private string[] animVars = { "State" };
    private int currentState = 0;

    //Class Icons
    public GameObject[] classObjs;
    public Button[] classButtons;
    public Image[] classImgs;
    public Image[] classRings;

    //Class Upgrade Icons
    public GameObject[] classUpgradeObjs;
    public Button[] classUpgradeButtons;
    public Image[] classUpgradeImgs;
    public Image[] classUpgradeRings;

    //Unit Upgrade Icons
    public GameObject[] unitUpgradeObjs;
    public Button[] unitUpgradeButtons;
    public Image[] unitUpgradeImgs;
    public Image[] unitUpgradeRings;

    


    //Set Menu
    public void SetMenu2(UnitConVars ucv)
    {
        //Set Main image
        mainImage.sprite = ucv.mainImage;

        //Set Icons
        SetArray(classObjs, classButtons, classImgs, classRings, ucv.classActive, ucv.classSelect, ucv.classImgs, ucv.classActions);
        SetArray(classUpgradeObjs, classUpgradeButtons, classUpgradeImgs, classUpgradeRings, ucv.classUpgradeActive, ucv.classUpgradeSelect, ucv.classUpgradeImgs, ucv.classUpgradeActions);
        SetArray(unitUpgradeObjs, unitUpgradeButtons, unitUpgradeImgs, unitUpgradeRings, ucv.unitUpgradeActive, ucv.unitUpgradeSelect, ucv.unitUpgradeImgs, ucv.unitUpgradeActions);

    }

    //Set Arrays
    public void SetArray(GameObject[] goA, Button[] btnA, Image[] imgA, Image[] ringA, bool[] active, bool[] select, Sprite[] imgs, UnityAction[] actions)
    {
        for(int i = 0; i < goA.Length; ++i)
        {
            //check if should be active
            if (active[i])
            {

                //Show element
                goA[i].SetActive(true);

                //Set Image
                imgA[i].sprite = imgs[i];

                //Set Selected
                ringA[i].color = (select[i]) ? selectionColors[0] : selectionColors[1];

                //Set Events
                btnA[i].onClick.RemoveAllListeners();
                btnA[i].onClick.AddListener(actions[i]);
            }
            else
            {
                //Hide element
                goA[i].SetActive(false);
            }
        }
    }

    //Rotate Menu
    public void RotateMenu(int i)
    {
        //check if in 0 state
        if(currentState != 0)
        {

            //menu is open, inc/dec in direction
            if(currentState+i <= 0)
            {
                SetAnimation(3);
            }else if(currentState + i >= 4)
            {
                SetAnimation(1);
            }
            else
            {
                SetAnimation(currentState + i);
            }
        }
    }

    //Open Menu - Also used to instant set open menu
    public void Open(int i)
    {
        if (i != 0)
        {
            SetAnimation(i);
        }
    }

    //Close Menu
    public void Close()
    {
        SetAnimation(0);
    }
    
    //Set Animation State
    private void SetAnimation(int i)
    {
        anim.SetInteger(animVars[0], i);
        currentState = i;
    }
    
}

public class UnitConVars
{
    //Images
    public Sprite mainImage;
    public Sprite[] classImgs;
    public Sprite[] classUpgradeImgs;
    public Sprite[] unitUpgradeImgs;

    //Active
    public bool[] classActive;
    public bool[] classUpgradeActive;
    public bool[] unitUpgradeActive;

    //Selected
    public bool[] classSelect;
    public bool[] classUpgradeSelect;
    public bool[] unitUpgradeSelect;

    //Events
    public UnityAction[] classActions;
    public UnityAction[] classUpgradeActions;
    public UnityAction[] unitUpgradeActions;

}
