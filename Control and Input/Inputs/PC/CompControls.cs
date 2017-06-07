using UnityEngine;
using System.Collections;

public class CompControls : Controls
{

    //PC control Classes
   // private MouseInput mouse;
  //  private KeyboardInput keys;

    //Comp Buttons
    private CompInputs btn = new CompInputs();

    //Class constructor
    public override void Init(STDMath math, Overlord over, Generator gen)
    {
        //set base controls
        base.Init(math, over, gen);

        //initialize input classes
       // mouse = this.gameObject.AddComponent<MouseInput>();
       // keys = this.gameObject.AddComponent<KeyboardInput>();
       // mouse.con = keys.con = this;
    }

    //Update
    private void Update()
    {
        //set current cursor position
        previousPosition = inputPosition;
        inputPosition = Input.mousePosition;

        //set Click bools
        LeftClickBools();

        //check for inputs
        if (!LeftClicks())
        {

        }
    }


    //Left click bools
    private void LeftClickBools()
    {
        //Set Left click this frame
        leftClick = Input.GetMouseButtonDown(btn.mouse_Left);

        //Set Left Hold
        leftClick_Hold = Input.GetMouseButtonDown(btn.mouse_Left);

        //Set Left Release
        leftClick_Release = Input.GetMouseButtonUp(btn.mouse_Left);
    }
}

public class CompInputs
{
    public int mouse_Left = 0;
    public int mouse_Right = 1;
}
