using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class FormationDriver : MonoBehaviour
{

    //GUI elements
    public Button SetButton;
    public List<GameObject> toggleObjs;
    public List<Button> prefabSelect;


    private UnitFormation unitForm;
    private int maxToggles = 0;
    private int togglePosition = 0;
    private int currentToggle = 0;
    private int currentFormation = 0;
    private string togName = "Toggle (";
    private List<Toggle[,]> toggles = new List<Toggle[,]>();
    private List<bool[,]> formations = new List<bool[,]>();

    //Initialize
    private void Awake()
    {
        //initialize placeholding formation
        unitForm = new UnitFormation();
        
        //initialize prefab Toggles
        for (int i = 0; i < 6; ++i)
        {
            //create new array list
            Toggle[,] tog = new Toggle[5, 5];

            //cycle lists
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    //get toggle
                    int num = (y * 5) + x + 1;
                    string s = togName + num + ")";
                    Debug.Log(s);
                    Toggle t = toggleObjs[i].transform.Find(s).gameObject.GetComponent<Toggle>();
                    tog[y, x] = t;
                }
            }

            //set toggle list
            toggles.Add(tog);
        }

        //hide panel
        this.gameObject.SetActive(false);
    }



    //Activate panel
    public void TogglePanel(UnitFormation uf = null)
    {
        //check if panel is active & formation info valid
        if(!this.gameObject.activeSelf && uf != null)
        {
            //enable panel
            this.gameObject.SetActive(true);

            //set formations
            unitForm = uf;
            formations = uf.GetAllFormations();
            SetAllToggles();

            //set function info
            maxToggles = uf.maxUnits;
        }
        else
        {
            //disable panel
            this.gameObject.SetActive(false);
        }
    }

    //Set Toggles
    private void SetAllToggles()
    {
        for(int i = 0; i < 6; ++i)
        {
            SetToggles(i);
        }
    }
    private void SetToggles(int i)
    {
        //cycle toggle list
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //activate panels
                toggles[i][y, x].isOn = formations[i][y, x];
            }
        }
    }

    //Set formation
    private void SetFormation(int i)
    {
        //cycle toggle list
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //set formation
                formations[5][y, x] = toggles[i][y, x];
            }
        }

        //update unit formation
        currentFormation = i;
        unitForm.SetFormation(formations[5], 5);
    }

    //Check for matching formation
    private bool FormationMatches()
    {
        //cycle toggle list
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //Check if active
                if(toggles[5][y, x].isOn != formations[currentFormation][y, x])
                {
                    //formation don't match
                    return false;
                }
            }
        }

        //formation matches
        return true;
    }


    //Custom Formation
    public void SetCustom(int i)
    {
        //check if set to custom already
        if(currentFormation == 5)
        {

        }
        else
        {
            //not set to custom, copy previous to custom
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    formations[5][y,x] = toggles[5][y, x].isOn = toggles[currentFormation][y, x].isOn;                  
                    
                }
            }
        }
    }

    //Set toggled position
    private void SetToggledPosition(int fx, int fy)
    {
        //Cyccle formation list
        for (int y = 0; y < 5; ++y)
        {
            for (int x = 0; x < 5; ++x)
            {
                //check if active
                if (CurrentToggle())
                {

                }
            }
        }
    }

    //Get toggle position
    public void ResetTogglePostion()
    {
        currentToggle = 0;
        togglePosition = 0;
    }
    public bool CurrentToggle()
    {
        //return bool
        bool ret = false;

        //check if current toggle
        if(currentToggle == togglePosition)
        {
            //increment toggle
            ++togglePosition;
            currentToggle = 0;
            ret = true;
        }
        else
        {
            //increment current toggle
            ++currentToggle;
        }

        //reset togle position
        if(togglePosition >= maxToggles)
        {
            togglePosition = 0;
        }

        //return bool
        return ret;
    } 
}
