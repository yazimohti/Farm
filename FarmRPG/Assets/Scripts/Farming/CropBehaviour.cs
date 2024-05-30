using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropBehaviour : MonoBehaviour
{
    int landID;
    //Information on what the grow will grow into
    SeedData seedToGrow;
    [Header("Stages of Seed")]
    public GameObject seed;
    public GameObject wilted;
    private GameObject seedling;
    private GameObject harvestable;
    
    //The grow points of the crop
    int growth;
    int maxGrowth;

    //Healt system
    int maxHealth = GameTimeStamp.HoursToMinutes(48);
    int health;
    
    public enum CropState
    {
        Seed,Seedling,Harvestable,Wilted
    }
    public CropState cropState;

    //Initialisation for the crop GameObject
    public void Plant(int landID, SeedData seedToGrow)
    {
        LoadCrop(landID, seedToGrow, CropState.Seed, 0, 0);
        LandManager.Instance.RegisterCrop(landID, seedToGrow.name, cropState, growth, health);
    }

    public void LoadCrop(int landID, SeedData seedToGrow, CropState cropState, int growth, int health)
    {
        this.landID = landID;

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

        this.growth = growth;
        this.health = health;

        //Check if it is regrowable
        if(seedToGrow.regrowable)
        {
            //Get the RegrowableHarvestBehaviour from gameObject
            RegrowableHarvestBehaviour regrowableHarvest = harvestable.GetComponent<RegrowableHarvestBehaviour>();

            //Initialise harvestable
            regrowableHarvest.SetParent(this);
        }
        //Set the initial state to seed
        SwitchState(cropState);
    }

    //the crop will grow when watered
    public void Grow()
    {
        //Increase the growth point by 1
        growth++;

        //Restore the health of plant when it is watered
        if(health < maxHealth)
        {
            health++;
        }

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

        //Update Landmanager
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }

    //The crop will progressively wither when the soil is dry
    public void Wither()
    {
        health--;
        //If the health is below 0 and the crop has germinated, kill it
        if(health <= 0 && cropState != CropState.Seed)
        {
            SwitchState(CropState.Wilted);
        }
        //Update Landmanager
        LandManager.Instance.OnCropStateChange(landID, cropState, growth, health);
    }
    
    public void SwitchState(CropState stateToSwitch)
    {
        //Reset everything and set all GameObjects to inactive
        seed.SetActive(false);
        seedling.SetActive(false);
        wilted.SetActive(false);
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

                //Give the seed health
                health = maxHealth;
                break;

            case CropState.Harvestable:
                //Enable the harvestable GameObject
                harvestable.SetActive(true);

                //If the seed is not regrowable, detach the harvestable from this crop gameobject and destroy it.
                if(!seedToGrow.regrowable)
                {
                    //Unparent it to the crop
                    harvestable.transform.parent = null;
                    RemoveCrop();
                }
                break;
            case CropState.Wilted:
                wilted.SetActive(true);
                break;
        }
        //Set the current crop state to the state we're switching to
        cropState = stateToSwitch;
    }
    public void RemoveCrop()
    {
        LandManager.Instance.DeRegisterCrop(landID);
        Destroy(gameObject);
    }

    //Called when the player harvests a regrowable crop. Resets the state to seedling 
    public void Regrow()
    {
        //Reset the growth
        int hoursToReGrow = GameTimeStamp.DaysToHours(seedToGrow.daysToRegrow);
        growth = maxGrowth - GameTimeStamp.HoursToMinutes(hoursToReGrow); 
        
        //Switch to state back to seedling
        SwitchState(CropState.Seedling);
    }
    
}
