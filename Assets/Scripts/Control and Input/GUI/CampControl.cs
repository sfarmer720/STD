using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class CampControl : MonoBehaviour
{

    /* 
     * Class designed to control Right screen control wheel.
     * 3 sets of 5 icons.
     * All Icons can only have 1 action assscociated with them.
     * 
     * Control Icons: used to special order units (formation, return, patrol, behavior, select similar/all)
     * Hire Icons: Used to hire new common units (infantry, archer, calvary, scout), last icon used to open special menu
     * Defense / Siege Icons: Special hire menu, allows for the hiring of specific siege or defense units.
     *  
     * TODO:
     * Main Button actions
     * 
     */

    //Constant Variables
    public Animator anim;

    //Images
    public Image mainImage;
    public Image[] hireImgs;
    public Image[] specialImgs;
    public Image[] controlImgs;

    //Sprites
    public Sprite[] mainImg;
    public Sprite[] hireSprites;
    public Sprite[] controlSprites;
    public Sprite[] specialSprites_Offense;
    public Sprite[] specialSprites_Defense;

    //Buttons
    public Button mainButton;
    public Button[] controlButtons;
    public Button[] hireButtons;
    public Button[] specialButtons;

    //default actions
    private UnityAction[] defaultControlActions;
    private UnityAction[] defaultHireActions;
    private UnityAction[] defaultSpecialActions;

    //private variables
    private string[] animVars = { "State" };
    public int currentState = 0;

    //Initialize
    public void Init(bool onDefense, UnityAction[] defaultControl, UnityAction[] defaultHire, UnityAction[] defaultSpec)
    {
        if (onDefense)
        {
            //Set up control for defense player
            mainImage.sprite = mainImg[0];
            for(int i = 0; i < specialImgs.Length; ++i)
            {
                specialImgs[i].sprite = specialSprites_Defense[i];
            }

        }
        else
        {
            //Set up control for offense player
            mainImage.sprite = mainImg[1];
            for (int i = 0; i < specialImgs.Length; ++i)
            {
                specialImgs[i].sprite = specialSprites_Offense[i];
            }
        }

        //Set hire / control sprites
        for(int i = 0; i < hireImgs.Length; ++i)
        {
            hireImgs[i].sprite = hireSprites[i];
            controlImgs[i].sprite = controlSprites[i];
        }

        //Set default actions
        defaultControlActions = defaultControl;
        defaultSpecialActions = defaultSpec;
        defaultHireActions = defaultHire;
        defaultHireActions[4] = () =>
        {
            Open(3);
        };
        ResetActions();
        //set main button action
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(() => { Open(2); });
    }


    //Restore actions
    public void ResetActions()
    {
        for(int i = 0; i < 3; ++i)
        {
            ResetActions(i);
        }
    }
    public void ResetActions(int i)
    {
        if (i == 0)
        {
            ActionSetter(controlButtons, defaultControlActions);
        }
        else if (i == 1)
        {
            ActionSetter(hireButtons, defaultHireActions);
        }
        else if (i == 2)
        {
            ActionSetter(specialButtons, defaultHireActions);
        }
    }

    //Set Special actions
    public void SetActions(int i, UnityAction[] actions)
    {
        //set correct button array
        if(i == 0)
        {
            ActionSetter(controlButtons, actions);
        }else if (i == 1)
        {
            ActionSetter(hireButtons, actions);
        }else if (i == 2)
        {
            ActionSetter(specialButtons, actions);
        }
    }
    private void ActionSetter(Button[] btnA, UnityAction[] acts)
    {
        for(int i = 0; i < btnA.Length; ++i)
        {
            btnA[i].onClick.RemoveAllListeners();
            btnA[i].onClick.AddListener(acts[i]);
        }
    }

    //Rotate Menu
    public void RotateMenu(int i)
    {
        //check if in 0 state
        if (currentState != 0)
        {

            //menu is open, inc/dec in direction
            if (currentState + i <= 0)
            {
                SetAnimation(3);
            }
            else if (currentState + i >= 4)
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
            //set to open
            SetAnimation(i);

            //set main button action
            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(() => { Close(); });
        }
    }

    //Close Menu
    public void Close()
    {
        //end all anim
        SetAnimation(0);

        //set main button action
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(() => { Open(2); });
    }

    //Set Animation State
    private void SetAnimation(int i)
    {
        anim.SetInteger(animVars[0], i);
        currentState = i;
    }



   
}

