using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class RightMenuControl : MonoBehaviour
{
    /*
     * Class designed to control the right click based menu
     * Right click menu is radial, with no center image
     * contains 3 sets of 5 icon buttons.
     * 
     * Icon buttons are customized based on current selection
     * 
     * TODO: SET ANIMATIONS
     */

    //cross class
    private STDMath stdMath;
    private Selector select;

    //GUI
    public Animator anim;
    public Sprite defaultImg;

    //Image group 1
    public Sprite[] imgs_01;
    public Image[] cons_01;
    public Button[] btns_01;

    //Image group 2
    public Sprite[] imgs_02;
    public Image[] cons_02;
    public Button[] btns_02;

    //Image group 3
    public Sprite[] imgs_03;
    public Image[] cons_03;
    public Button[] btns_03;

    //Right Menu Target and reference
    public Unit reference;
    public Transform target;
    public int targetType;          //0 = tile, 1 = unit, 2 = enemy

    //Control variables
    public bool isOpen;

    //Update
    void FixedUpdate()
    {
        //check if menu is open
        if (isOpen)
        {

        }
    }

    //Open menu
    public void OpenMenu(Vector3 clickPos, bool recurse = false)
    {
        //Check if menu has a reference
        if(reference != null)
        {
            //menu has a refernce, move menu to click pos
            this.gameObject.transform.position = clickPos;

            //load menu elements
            GroupSetter(0);

            //if not recursive, check for target
            if (!recurse)
            {
                SetTarget(select.ClickSelect(clickPos, true));
            }
            else
            {
                //is recursive, set target to current tile
                SetTarget(reference.currentTile.gameObject.transform);
            }

            //set menu to visible
            this.gameObject.SetActive(true);
        }
        else
        {
            //no reference, check if clicked reference
            Transform t = select.ClickSelect(clickPos, true);
            if(t != null && select.IsUnit(t.gameObject))
            {
                //Friendly unit found, set as reference
                reference = t.GetComponent<Unit>();

                //recurse with new refence
                OpenMenu(clickPos, true);
            }
            else
            {
                //no reference found, run close for safety
                CloseMenu();
            }
        }
    }

    //Set group of buttons
    private void GroupSetter(int i)
    {
        //set buton group
        if(i == 0)
        {
            SetButtons(btns_01, cons_01, reference.CurrentStats().Group_01);
            SetButtons(btns_03, cons_02, reference.CurrentStats().Group_02);
            SetButtons(btns_03, cons_03, reference.CurrentStats().Group_03);
        }
        else if(i == 1)
        {
            SetButtons(btns_01, cons_01, reference.CurrentStats().Group_01);
        }else if(i == 2)
        {
            SetButtons(btns_03, cons_02, reference.CurrentStats().Group_02);
        }
        else if(i == 3)
        {
            SetButtons(btns_03, cons_03, reference.CurrentStats().Group_03);
        }
    }

    //Set button actions
    private void SetButtons(Button[] btns, Image[] imgs, RightActions[] actions)
    {
        //cycle group
        for(int i = 0; i < btns.Length; ++i)
        {
            //Run check active, to see if action is active
            actions[i].checkActive();

            //set image
            imgs[i].sprite = actions[i].actionImage;

            //set button action
            btns[i].onClick.RemoveAllListeners();

            //add only if active
            if (actions[i].active)
            {
                btns[i].onClick.AddListener(actions[i].action);
            }
        }
    }

    //Close menu
    public void CloseMenu()
    {
        target = null;
        isOpen = false;

        this.gameObject.SetActive(false);
    }

    //Set Target and type
    public void SetTarget(Transform t)
    {
        target = t;
        if (select.IsTile(t.gameObject))
        {
            targetType = 0;
        }else if (select.IsUnit(t.gameObject))
        {
            targetType = 1;
        }else if (select.IsEnemy(t.gameObject))
        {
            targetType = 2;
        }
    }

}