using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class Land : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        //Get the renderer component
        renderer = GetComponent<Renderer>();
        //Set the default landStatus
        SwitchLandStatus(LandStatus.Soil);
        //Deselect the land ny default
        Select(false);
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
}
