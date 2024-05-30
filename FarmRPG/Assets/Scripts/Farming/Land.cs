using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;

public class Land : MonoBehaviour, ITimeTracker
{
    public int id;
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
    public float timeToDry = 24f;

    [Header("Crops")]
    //The crop prefab to instantiate
    public GameObject cropPrefab;

    //The crop currently planted on the land
    CropBehaviour cropPlanted = null;
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

    public void LoadLandData(LandStatus statusToSwitch, GameTimeStamp lastWatered)
    {
        landStatus = statusToSwitch;
        timeWatered = lastWatered;

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

        LandManager.Instance.OnLandStateChange(id, landStatus, timeWatered);
    }

    public void Select(bool toggle)
    {
        select.SetActive(toggle);
    }
    public void Interact()
    {
        //Check the player's tool slot 
        ItemData toolSlot = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);
        if(!InventoryManager.Instance.SlotEquipped(InventorySlot.InventoryType.Tool))
        {
            return;
        }

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
                case EquipmentData.ToolType.Shovel:
                    //Remove the crop from the land
                    if(cropPlanted != null)
                    {
                        cropPlanted.RemoveCrop();
                    }
                    break;
            }

            //We don't need to check for seeds if we have already confirmed the tool to be an equipment
            return;
        }

        //Try casting the itemdata in the toolslot as SeedData
        SeedData seedTool = toolSlot as SeedData;

        //Conditions for the player to be able to plan a seed
        //1: He is holdin a tool of type SeedData
        //2: The Land State must be either watered or farmland
        //3: There isn't already a crop that has been planted 
        if(seedTool != null && landStatus != LandStatus.Soil && cropPlanted == null)
        {
            SpawnCrop();
            //Plant it with the seed's information
            cropPlanted.Plant(id, seedTool);
            //Consumer the item 
            InventoryManager.Instance.ConsumeItem(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool)); 
        }
    }
    public CropBehaviour SpawnCrop()
    {
        //Instantiate the crop object parented to the land
            GameObject cropObject = Instantiate(cropPrefab,transform);
            //Move the crop object to the top of the land GameObject
            cropObject.transform.position = new Vector3(transform.position.x,.51f,transform.position.z);
            //Access the CropBehaviour of the crop we're going to plant
            cropPlanted = cropObject.GetComponent<CropBehaviour>(); 
            return cropPlanted;
    }

    public void UpdateClock(GameTimeStamp timeStamp)
    {
        //Checked if 24 hours has passed since last watered 
        if(landStatus == LandStatus.Watered)
        {   
            //Hours since the land was watered 
            int hoursElapsed = GameTimeStamp.CompareTimeStamps(timeStamp,timeWatered);
            
            //Grow the planted crop, if any
            if(cropPlanted != null)
            {
                cropPlanted.Grow();
            }

            if(hoursElapsed > timeToDry)
            {
                //Dry up 
                SwitchLandStatus(LandStatus.Farmland);
            }
        }

        //Handle the wilting of the plant when the land is not watered 
        if(landStatus != LandStatus.Watered && cropPlanted != null)
        {
            //If the crop has already germinated, start the withering
            if(cropPlanted.cropState != CropBehaviour.CropState.Seed)
            {
                cropPlanted.Wither();
            }
        }
    }

    private void OnDestroy()
    {
        TimeManager.Instance.UnRegisterTracker(this);
    }
}
