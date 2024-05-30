using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveState
{
    //Farm data
    public List<LandSaveStatement> landData;
    public List<CropSaveState> cropData;

    //Inventory data
    public ItemSlotSaveData[] toolSlots;
    public ItemSlotSaveData[] itemSlots;

    public ItemSlotSaveData equippedToolSlot;
    public ItemSlotSaveData equippedItemSlot;

    //Time
    public GameTimeStamp timeStamp;

    public GameSaveState(List<LandSaveStatement> landData, List<CropSaveState> cropData, ItemSlotData[] toolSlots, ItemSlotData[] itemSlots, ItemSlotData equippedToolSlot, ItemSlotData equippedItemSlot, GameTimeStamp timeStamp)
    {
        this.landData = landData;
        this.cropData = cropData;
        this.toolSlots = ItemSlotData.SerializeArray(toolSlots);
        this.itemSlots = ItemSlotData.SerializeArray(itemSlots);
        this.equippedToolSlot = ItemSlotData.SerializeData(equippedToolSlot);
        this.equippedItemSlot = ItemSlotData.SerializeData(equippedItemSlot);
        this.timeStamp = timeStamp;
    }
}
