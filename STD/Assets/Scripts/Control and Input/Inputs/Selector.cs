using UnityEngine;
using System.Collections.Generic;

public class Selector : MonoBehaviour
{
    /*
     * Selection class
     * Used to select individual and group objects.
     * Only selects objects set to selectable layers.
     * Returns parsed selection in order of importance.
     * 1- friendly unit
     * 2- enemy unit
     * 3- tiles
     * 
     * Mask layers:
     * 8 - Selectable layer
     * 12 - Visible Tile
     * 13 - Visited Tile
     * 31 - Tile Outline
     * 
     */

    //Cross class Variables
    public Controls controls;
    private Camera cam;

    //Selection Mask Variables
    private int mask;
    private int[] masks = { 8, 12, 13, 31};

    //Public Selection variables
    public SelectionGroup currentGroup;
    public Vector3 initGroupPos;
    public bool groupSelect = false;
    public Color groupSelectColor = new Color(0.8f, 0.8f, 0.95f, 0.25f);
    public Color groupSelectBorderColor = new Color(0.8f, 0.8f, 0.95f);

    public float selectSize = 1f;
    public Transform currentSelection;
    public List<Transform> groupSelection = new List<Transform>();
    public List<SelectionGroup> selectGroups = new List<SelectionGroup>();

    //selection check variables
    private bool tileIsSelected = false;
    private bool unitIsSelected = false;
    private bool enemeyIsSelected = false;

    //Initializing method
    public void Init(Controls con)
    {

        //Initialize selection mask
        for(int i = 0; i < masks.Length; ++i)
        {
            int m = 1 << masks[i];
            mask = mask | m;
        }

        //set camera
        cam = Camera.main;

    }

    //On GUI, used to draw group selects
    private void OnGUI()
    {
        //check if group selecting
        if (groupSelect)
        {
            //draw selection rectangle
            Rect r = GroupSelectUtil.GetGroupRect(initGroupPos, controls.InputPosition());
            GroupSelectUtil.DrawGroupRect(r, groupSelectColor);
            GroupSelectUtil.DrawGroupRectBorder(r, groupSelectBorderColor, 2);
        }
    }


    // Standard Click selction
    public Transform ClickSelect(Vector3 castFrom, bool noSet = false)
    {

        //create spherecast
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(castFrom);

        //cast sphere
        if (Physics.SphereCast(ray, 1, out hit, Mathf.Infinity, mask))
        {
            //check for hits
            if(hit.transform != null)
            {
                //check if setting selection
                if (noSet)
                {
                    return hit.transform;
                }
                else
                {
                    //set current selection
                    currentSelection = SelectedType(hit.transform);
                }
            }
        }

        return (noSet) ? null : currentSelection;
    }

    //Search click selection
    public Transform SearchSelect(Vector3 castFrom, int lookingFor)
    {

        //create spherecast
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(castFrom);

        //cast sphere
        if (Physics.SphereCast(ray, selectSize, out hit, Mathf.Infinity, mask))
        {
            //check for specific hit
            if (hit.transform != null && SearchHit(hit.transform,lookingFor))
            {
                return hit.transform;
            }
        }

        //nothing found
        return null;
    }

    

    //Determine Type of object selected
    private Transform SelectedType(Transform t)
    {
        //check for types
        if (IsUnit(t.gameObject))
        {
            //check if unit is in a group
            if (UnitInGroup(GetUnit(t)))
            {
                //TODO: Unit is in group
                t = UnitActions(t, false);
            }
            else{
                //Unit is not in group
                t = UnitActions(t, false);
            }

        }else if (IsEnemy(t.gameObject)){

            t = UnitActions(t, true);

        }else if (IsTile(t.gameObject)){
            
            t = TileActions(t);
        }

        //return transform
        return t;
    }


    //Tile Select Actions
    private Transform TileActions(Transform t)
    {

        //Check for previous selection
        if (currentSelection != null)
        {
            //check if same tile was selected
            if (t == currentSelection)
            {
                //deselect and null current select
                TileToggle(GetTile(t));
                ClearSelection();
                return null;
            }

            //Check if tile was previously selected
            if (IsTile(currentSelection.gameObject))
            {
                //toggle selection of both tiles
                TileToggle(GetTile(currentSelection));
            }

            //Check if enemy was previously selected
            if (IsEnemy(currentSelection.gameObject))
            {
                //toggle selection to tile
                UnitToggle(GetUnit(currentSelection));
            }

            //Check if Unit was previously selected
            if (IsUnit(currentSelection.gameObject))
            {
                //Move unit to newly selected tile
                GetUnit(currentSelection).UnitToTile(t);
                return currentSelection;
            }
        }

        //Trigger tile script selection
        TileToggle(GetTile(t));
        SetTileSelect();

        //return modified transform
        return t;

    }

    //Unit Select Actions
    private Transform UnitActions(Transform t, bool isEnemy)
    {
        //Check for previous selection
        if (currentSelection != null)
        {
            //check if same unit was selected
            if (t == currentSelection)
            {
                //deselect and null current select
                UnitToggle(GetUnit(t));
                ClearSelection();
                return null;
            }

            //Check if Tile was previously selected
            if (IsTile(currentSelection.gameObject))
            {
                //toggle selection to Unit
                TileToggle(GetTile(currentSelection));
            }

            //Check if enemy was previously selected
            if (IsEnemy(currentSelection.gameObject))
            {
                //Toggle to new unit
                UnitToggle(GetUnit(currentSelection));
            }

            //Check if Unit was previously selected
            if (IsUnit(currentSelection.gameObject))
            {
                //Check if new unit is enemy
                if (isEnemy)
                {
                    //move to attack
                }
                else
                {
                    //toggle to new unit
                    UnitToggle(GetUnit(currentSelection));
                }
            }
        }

        //Trigger unit script selection
        UnitToggle(GetUnit(t));

        if (isEnemy)
        {
            SetEnemySelect();
        }
        else
        {
            SetUnitSelect();
        }

        //return modified transform
        return t;

    }

    //Unit Group Actions

    //SHORT HANDS//
    public Unit GetUnit(Transform t){
        return t.gameObject.transform.parent.gameObject.GetComponent<Unit>();
    }
    public Tile GetTile(Transform t){
        return t.gameObject.GetComponent<Tile>();
    }
    private void TileToggle(Tile t) {
        t.Selection();
    }
    private void UnitToggle(Unit u) {
        u.Selection();
    }
    private void UnitSetGroup(Unit u, SelectionGroup group){
        group.selectionGroup.Add(u.gameObject.transform);
        u.CurrentStats().inGroup = true;
        u.CurrentStats().group = group;
    }
    private void UnitRemoveGroup(Unit u){
        u.CurrentStats().group.selectionGroup.Remove(u.gameObject.transform);
        u.CurrentStats().inGroup = false;
    }

    //BOOLEAN CHECKS//
    private bool SearchHit(Transform t, int lookingFor){
        switch (lookingFor)
        {
            case 0: return IsUnit(t.gameObject);
            case 1: return IsEnemy(t.gameObject);
            case 2: return IsTile(t.gameObject);
        }
        return false;
    }
    public bool IsUnit(GameObject go){
        return (go.layer == masks[0] && !GetUnit(go.transform).BaseStats().isEnemy);
    }
    public bool IsEnemy(GameObject go){
        return (go.layer == masks[0] && GetUnit(go.transform).BaseStats().isEnemy);
    }
    public bool IsTile(GameObject go){
        return (go.layer == masks[1] || go.layer == masks[2] || go.layer == masks[3]);
    }
    public bool UnitInGroup(Unit u) {
        return (u.CurrentStats().inGroup);
    }
    public bool UnitInBounds(Transform t)
    {
        return GroupSelectUtil.GetRectBounds(initGroupPos, controls.InputPosition()).Contains(Camera.main.WorldToViewportPoint(t.position));
    }


    //LISTENER CHECKS//
    public bool IsUnitSelected()
    {
        return unitIsSelected;
    }
    public bool IsTileSelected()
    {
        return tileIsSelected;
    }
    public bool IsEnemySelected()
    {
        return enemeyIsSelected;
    }
    public bool IsSelection()
    {
        return (IsUnitSelected() || IsTileSelected() || IsEnemySelected());
    }
    public void SetUnitSelect()
    {
        unitIsSelected = true;
        tileIsSelected = false;
        enemeyIsSelected = false;
    }
    public void SetTileSelect()
    {
        unitIsSelected = false;
        tileIsSelected = true;
        enemeyIsSelected = false;
    }
    public void SetEnemySelect()
    {
        unitIsSelected = false;
        tileIsSelected = false;
        enemeyIsSelected = true;
    }
    public void ClearSelection()
    {
        unitIsSelected = tileIsSelected = enemeyIsSelected = false;
        currentSelection = null;
    }
    public void SetSelection(Transform t)
    {
        ClearSelection();
        SelectedType(t);
    }
}

public class SelectionGroup
{
    public int groupType = -1;
    public bool activeGroup;
    public Transform activeSelection;
    public Transform targetSelection;
    public List<Transform> selectionGroup;
    
}

public static class GroupSelectUtil
{
    /*
     * Code writen from Hyunkell's Blog
     * http://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
     * 
     */


    //Initialize texture
    static Texture2D tex;
    public static Texture2D GroupTex
    {
        get
        {
            if(tex == null)
            {
                tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
            }

            return tex;
        }
    }

    //Draw interior rect
    public static void DrawGroupRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, GroupTex);
        GUI.color = Color.white;
    }

    //Draw border rect
    public static void DrawGroupRectBorder(Rect rect, Color color, float size)
    {
        //Top
        GroupSelectUtil.DrawGroupRect(new Rect(rect.xMin, rect.yMin, rect.width, size), color);

        //Left
        GroupSelectUtil.DrawGroupRect(new Rect(rect.xMin, rect.yMin, size, rect.height), color);

        //Right
        GroupSelectUtil.DrawGroupRect(new Rect(rect.xMax - size, rect.yMin, size, rect.height), color);

        //Bottom
        GroupSelectUtil.DrawGroupRect(new Rect(rect.xMin, rect.yMax - size, rect.width, size), color);
    }

    //Set rect position
    public static Rect GetGroupRect(Vector3 pos1, Vector3 pos2)
    {
        //relocate origin
        pos1.y = Screen.height - pos1.y;
        pos2.y = Screen.height - pos2.y;

        //Set corners
        Vector3 topLeft = Vector3.Min(pos1, pos2);
        Vector3 bottomRight = Vector3.Max(pos1, pos2);

        //create rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    //get rect bounds
    public static Bounds GetRectBounds(Vector3 pos1, Vector3 pos2)
    {
        //get viewport points
        Vector3 v1 = Camera.main.ScreenToViewportPoint(pos1);
        Vector3 v2 = Camera.main.ScreenToViewportPoint(pos2);

        //find min/max
        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);
        min.z = Camera.main.nearClipPlane;
        max.z = Camera.main.farClipPlane;

        //Set bounds
        Bounds b = new Bounds();
        b.SetMinMax(min, max);
        return b;
    }
}
