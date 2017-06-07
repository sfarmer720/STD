using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class UnitMenu : MonoBehaviour
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
    public Image[] classImgs;
    public Image[] classUpgradeImgs;
    public Image[] unitUpgradeImgs;

    //Sprites
    public Sprite[] mainImg;
    public Sprite[] classSprites;
    public Sprite[] unitUpgradeSprites;
    public Sprite[] classUpgradeSprites;

    //Buttons
    public Button mainButton;
    public Button[] unitUpgradeButtons;
    public Button[] classButtons;
    public Button[] classUpgradeButtons;

    //default actions
    private Sprite defaultMainImg;
    private Sprite[] defaultClassSprites;
    private Sprite[] defaultUnitUpgradeSprites;
    private Sprite[] defaultClassUpgradeSprites;
    private UnityAction[] defaultUnitUpgradeActions;
    private UnityAction[] defaultClassActions;
    private UnityAction[] defaultClassUpgradeActions;

    //private variables
    private string[] animVars = { "State" };
    public int currentState = 0;

    //Initialize
    public void Init(bool onDefense, UnityAction[] defaultUnitUp, UnityAction[] defaultClass, UnityAction[] defaultClassUp)
    {

        //Set up control for offense player
        mainImage.sprite = defaultMainImg = (onDefense) ? mainImg[1] : mainImg[0];
            for (int i = 0; i < classUpgradeImgs.Length; ++i)
            {
            classUpgradeImgs[i].sprite = classUpgradeSprites[i];
            }
        

        //Set hire / control sprites
        for (int i = 0; i < classImgs.Length; ++i)
        {
            classImgs[i].sprite = classSprites[i];
            unitUpgradeImgs[i].sprite = unitUpgradeSprites[i];
        }

        //Set default actions
        defaultClassSprites = classSprites;
        defaultClassUpgradeSprites = classUpgradeSprites;
        defaultUnitUpgradeSprites = unitUpgradeSprites;
        defaultUnitUpgradeActions = defaultUnitUp;
        defaultClassUpgradeActions = defaultClassUp;
        defaultClassActions = defaultClass;
        defaultClassActions[4] = () =>
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
        for (int i = 0; i < 3; ++i)
        {
            ResetActions(i);
        }
    }
    public void ResetActions(int i)
    {
        if (i == 0)
        {
            ActionSetter(unitUpgradeButtons, defaultUnitUpgradeActions);
        }
        else if (i == 1)
        {
            ActionSetter(classButtons, defaultClassActions);
        }
        else if (i == 2)
        {
            ActionSetter(classUpgradeButtons, defaultClassUpgradeActions);
        }
    }

    //Set Special actions
    public void SetActions(int i, UnityAction[] actions)
    {
        //set correct button array
        if (i == 0)
        {
            ActionSetter(unitUpgradeButtons, actions);
        }
        else if (i == 1)
        {
            ActionSetter(classButtons, actions);
        }
        else if (i == 2)
        {
            ActionSetter(classUpgradeButtons, actions);
        }
    }
    private void ActionSetter(Button[] btnA, UnityAction[] acts)
    {
        for (int i = 0; i < btnA.Length; ++i)
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

    //Set buttons
    public void SetButtons(Button[] btns, UnityAction[] actions, Image[] imgs ,Sprite[] sprites)
    {
        //cycle buttons
        for(int i = 0; i < btns.Length; ++i)
        {
            //set sprite
            imgs[i].sprite = sprites[i];

            //set button
            btns[i].onClick.RemoveAllListeners();
            btns[i].onClick.AddListener(actions[i]);
        }
    }
    public void SetMainButton(Sprite sprite) {
        mainImage.sprite = sprite;
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

    //reset menu to default
    public void ResetMenu()
    {
        
    }

    //Set Animation State
    private void SetAnimation(int i)
    {
        anim.SetInteger(animVars[0], i);
        currentState = i;
    }



}
