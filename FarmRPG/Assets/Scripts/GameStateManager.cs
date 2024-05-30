using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance {get; private set;}


    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }
    /*IEnumerator TransitionTime()
    {
        while(!screenFadedOut)
        {
            yield return new WaitForSeconds(1f);
        }
        //Save 
        SaveManager.Save(ExportSaveState());    
    }*/
    public GameSaveState ExportSaveState()
    {
        //Retrieve Farm Data
        List<LandSaveStatement> landData = LandManager.farmData.Item1;
        List<CropSaveState> cropData = LandManager.farmData.Item2;

        //Retrieve Inventory Data
        ItemSlotData[] toolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] itemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);

        ItemSlotData equippedToolSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool);
        ItemSlotData equippedItemSlot = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item);

        //Time
        GameTimeStamp timeStamp = TimeManager.Instance.GetGameTimeStamp();
        return new GameSaveState(landData, cropData, toolSlots, itemSlots, equippedToolSlot, equippedItemSlot, timeStamp);
    }
    /*public void LoadSave()
    {
        //Set the scene to the player's house
        //SceneTransitionManager.Instance.SwitchLocation(SceneTransitionManager.Location.PlayerHome);
        //Retrieve the loaded save
        GameSaveState save = SaveManager.Load();
        
        //Load up the parts 
        ItemSlotData[] toolSlots = ItemSlotData.DeSerializeArray(save.toolSlots);
        ItemSlotData equippedToolSlot = ItemSlotData.DeSerializeData(save.equippedToolSlot);
        ItemSlotData[] itemSlots = ItemSlotData.DeSerializeArray(save.itemSlots);
        ItemSlotData equippedItemSlot = ItemSlotData.DeSerializeData(save.equippedItemSlot);
        InventoryManager.Instance.LoadInventory(toolSlots, equippedToolSlot, itemSlots, equippedItemSlot);
        //Farming data
        LandManager.farmData = new System.Tuple<List<LandSaveStatement>, List<CropSaveState>>(save.landData, save.cropData);
    }*/
}
