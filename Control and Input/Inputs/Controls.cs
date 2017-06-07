using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{
    /*
     * Base control class
     * Parent class for accepting user input
     * Derived Classes: ControllerControls & CompControls
     * 
     * Click Types:
     * -1: no Click
     * 0: left Click
     * 1: double click
     * 2: right Click
     * 
     */


    //Cross Class Variables
    protected STDMath stdMath;
    protected Overlord overlord;
    protected Generator map;
    protected Camera cam;
    public Selector select;

    //Protected 'Mouse' Variables
    public float cursorTolerance = 3.5f;
    protected int clickType = -1;
    protected Vector3 inputPosition;
    protected Vector3 previousPosition;
    protected float clickTime = 0f;
    protected float doubleDelay = 0.75f;
    protected bool leftClick = false;
    protected bool leftClick_PreFram = false;
    protected bool leftClick_Double = false;
    protected bool leftClick_Hold = false;
    protected bool leftClick_Release = false;
    protected bool rightClick = false;
    
    //Universal Initialization
    public virtual void Init(STDMath math, Overlord over, Generator gen)
    {
        //Reference cross class
        stdMath = math;
        overlord = over;
        map = gen;
        cam = Camera.main;

        //initalize classes
        select = this.gameObject.AddComponent<Selector>();
        select.Init(this);
    }
    
    //Movement Functions
    public bool CursorMoved()
    {
        return stdMath.mouseTol(previousPosition, inputPosition, cursorTolerance);
    }

    //Left clicks - if returned true, no other clicks checked
    protected bool LeftClicks()
    {
        //Check if left click on this frame
        if (leftClick)
        {
            //Check if previous click
            if(!leftClick_PreFram || leftClick_Double || Time.time - clickTime > doubleDelay)
            {
                //No previous click, previous click was a double, or max double click time exceeded
                //Set previous click
                leftClick_PreFram = true;
                leftClick_Double = false;
                clickTime = Time.time;

                //Perform single selection
                SingleSelect(inputPosition);
            }

            else
            {
                //double click performed
                leftClick_PreFram = false;
                leftClick_Double = true;
            }

            //left click performed, return true
            return true;

        }

        //Check if left click still being held
        else if (leftClick_Hold)
        {
            //Check if cursor position has moved
            if (CursorMoved())
            {
                //begin group select
                GroupSelect();
            }

            //hold click enabled, return true
            return true;
        }

        //Check if Left click released after being held
        else if (leftClick_Release)
        {
            //end group select
            GroupSelect(true);

            //click release frame, return true
            return true;
        }

        //No left clicks, return false
        return false;
    }

   //Selection Functions
    public Transform SingleSelect(Vector3 castFrom, int lookingFor = -1)
    {
        if (lookingFor >= 0)
        {
            //return only certain type of object
            return select.SearchSelect(castFrom, lookingFor);

        } else {

            //Return object below select position
            return select.ClickSelect(castFrom);

        }
    }
    public void GroupSelect(bool endSelect = false)
    {
        if (endSelect)
        {
            select.groupSelect = false;

        } else {
            select.groupSelect = true;
            select.initGroupPos = inputPosition;
        }
    }


    //GETTERS & SETTERS//
    public Vector3 InputPosition(){
        return inputPosition;
    }
    public Transform CurrentSelection(){
        return select.currentSelection;
    }
    public SelectionGroup CurrentSelectedGroup(){
        return select.currentGroup;
    }

}
