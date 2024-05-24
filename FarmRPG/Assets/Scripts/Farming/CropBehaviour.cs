using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    //Information on what the grow will grow into
    SeedData seedToGrow;
    [Header("Stages of Seed")]
    public GameObject seed;
    private GameObject seedling;
    private GameObject harvestable;
    //The grow points of the crop
    int growth;
    int maxGrowth;
    public enum CropState
    {
        Seed,Seedling,Harvestable
    }
    public CropState cropState;

    //Initialisation for the crop GameObject
    public void Plant(SeedData seedToGrow)
    {
        

        //Save the seed information
        this.seedToGrow = seedToGrow;

        //Instantiate the seedling and harvestable GameObject
        seedling = Instantiate(seedToGrow.seedling,transform);

        //Access the crop item data
        ItemData cropToYield = seedToGrow.cropToYield;

        //Instantiate the harvestable GameObjct
        harvestable = Instantiate(cropToYield.gameModel, transform);

        //Convert Days To Grow into Hours
        int hoursToGrow = GameTimeStamp.DaysToHours(seedToGrow.daysToGrow);

        //Convert Hours to Grow into minutes
        maxGrowth = GameTimeStamp.HoursToMinutes(hoursToGrow);

        //Check if it is regrowable
        if(seedToGrow.regrowable)
        {
            //Get the RegrowableHarvestBehaviour from gameObject
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();

            //Initialise harvestable
            regrowableHarvest.SetParent(this);
        }

        //Set the initial state to seed
        SwitchState(CropState.Seed);
        
    }

    //the crop will grow when watered
    public void Grow()
    {
        //Increase the growth point by 1
        growth++;

        //The seed will sprout into a seedling when the growth is at %50
        if(growth >= maxGrowth/2 && cropState == CropState.Seed )
        {
            SwitchState(CropState.Seedling);
        }

        //Fully grown
        if(growth >= maxGrowth && cropState == CropState.Seedling)
        {
            SwitchState(CropState.Harvestable);
        }
    }
    public void SwitchState(CropState stateToSwitch)
    {
        //Reset everything and set all GameObjects to inactive
        seed.SetActive(false);
        seedling.SetActive(false);
        harvestable.SetActive(false);

        switch(stateToSwitch)
        {
            case CropState.Seed:
                //Enable the seed GameObject
                seed.SetActive(true);
                break;
            
            case CropState.Seedling:
                //Enable the seedling GameObject
                seedling.SetActive(true);
                break;

            case CropState.Harvestable:
                //Enable the harvestable GameObject
                harvestable.SetActive(true);

                //If the seed is not regrowable, detach the harvestable from this crop gameobject and destroy it.
                if(!seedToGrow.regrowable)
                {
                    //Unparent it to the crop
                    harvestable.transform.parent = null;
                    Destroy(gameObject);
                }
                break;
        }
        //Set the current crop state to the state we're switching to
        cropState = stateToSwitch;
    }

    //Called when the player harvests a regrowable crop. Resets the state to seedling 
    public void Regrow()
    {
        //Switch to state back to seedling
        SwitchState(CropState.Seedling);
    }
}
