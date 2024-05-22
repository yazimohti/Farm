using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Land : MonoBehaviour, ITimeTracker
{
    public enum LandStatus
    {
        Soil,Farmland,Watered,Grass
    }
    public LandStatus landStatus;
    public Material soilMat,farmlandMat,wateredMat,grassMat;
    new Renderer renderer;
    //The selection gameobject to enable when the player is selecting the land
    public GameObject select;

    //Cache the time the land was watered 
    GameTimeStamp timeWatered;
    // Start is called before the first frame update
    void Start()
    {
        //Get the renderer component
        renderer = GetComponent<Renderer>();
        //Set the default landStatus
        SwitchLandStatus(LandStatus.Soil);
        //Deselect the land ny default
        Select(false);
        //Add this to TimeManager's Listener list 
        TimeManager.Instance.RegisterTracker(this);
    }
    public void SwitchLandStatus(LandStatus statusToSwitch)
    {
        landStatus = statusToSwitch;
        Material materialToSwitch = soilMat;
        //Decide what material to switch
        switch(statusToSwitch)
        {
            case LandStatus.Soil:
                //switch to the soil material
                materialToSwitch = soilMat;
                break;
            case LandStatus.Farmland:
                //switch to the farmland material
                materialToSwitch = farmlandMat;
                break;
            case LandStatus.Watered:
                //switch to the watered material
                materialToSwitch = wateredMat;

                //Cache the time it was watered
                timeWatered = TimeManager.Instance.GetGameTimeStamp();
                break;
        }
        //Get the renderer to apply the changes
        renderer.material = materialToSwitch;

    }

    public void Select(bool toggle)
    {
        select.SetActive(toggle);
    }
    public void Interact()
    {
        //Check the player's tool slot 
        ItemData toolSlot = InventoryManager.Instance.equippedTool;

        //Try casting the itemdata in the toolslot as EquipmentData
        EquipmentData equipmentTool = toolSlot as EquipmentData;

        //Check if it is of type EquipmentData
        if(equipmentTool != null)
        {
            //Get the equipment tool type
            EquipmentData.ToolType toolType = equipmentTool.toolType;
            
            switch(toolType)
            {
                case EquipmentData.ToolType.Hoe:
                    SwitchLandStatus(LandStatus.Farmland);
                    break;
                case EquipmentData.ToolType.WateringCan:
                    SwitchLandStatus(LandStatus.Watered);
                    break;
            }
        }
    }

    public void UpdateClock(GameTimeStamp timeStamp)
    {
        //Checked if 24 hours has passed since last watered 
        if(landStatus == LandStatus.Watered)
        {   
            //Hours since the land was watered 
            int hoursElapsed = GameTimeStamp.CompareTimeStamps(timeStamp,timeWatered);
            Debug.Log(hoursElapsed + "Since this was watered");

            if(hoursElapsed > 24)
            {
                //Dry up 
                SwitchLandStatus(LandStatus.Farmland);
            }
        }
    }
}
