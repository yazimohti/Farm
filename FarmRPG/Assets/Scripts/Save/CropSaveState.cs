using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CropSaveState
{
    public int landID;
    public string seedToGrow;
    public CropBehaviour.CropState cropState;
    public int growth;
    public int health;
    public CropSaveState(int landID, string seedToGrow, CropBehaviour.CropState cropState, int growth, int health)
    {
        this.landID = landID;
        this.seedToGrow = seedToGrow;
        this.cropState = cropState;
        this.growth = growth;
        this.health = health;
    }
    public void Grow()
    {
        //Increase the growth point by 1
        growth++;

        SeedData seedInfo =(SeedData) InventoryManager.Instance.itemIndex.GetItemFromString(seedToGrow);
        int maxGrowth = GameTimeStamp.HoursToMinutes(GameTimeStamp.DaysToHours(seedInfo.daysToGrow));
        int maxHealth = GameTimeStamp.HoursToMinutes(48);

        //Restore the health of plant when it is watered
        if(health < maxHealth)
        {
            health++;
        }

        //The seed will sprout into a seedling when the growth is at %50
        if(growth >= maxGrowth/2 && cropState == CropBehaviour.CropState.Seed )
        {
            cropState = CropBehaviour.CropState.Seedling;
        }

        //Fully grown
        if(growth >= maxGrowth && cropState == CropBehaviour.CropState.Seedling)
        {
            cropState = CropBehaviour.CropState.Harvestable;
        }
    }
}
