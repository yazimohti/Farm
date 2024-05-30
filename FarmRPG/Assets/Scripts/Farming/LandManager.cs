using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LandManager : MonoBehaviour
{
    public static LandManager Instance {get; private set; }

    public static Tuple<List<LandSaveStatement>, List<CropSaveState>> farmData = null;
    List<Land> landPlots = new List<Land>();

    List<LandSaveStatement> landData = new List<LandSaveStatement>();
    List<CropSaveState> cropData = new List<CropSaveState>();

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

    // Start is called before the first frame update
    void OnEnable()
    {
        RegisterLandPlots();
        StartCoroutine(LoadFarmData());
    }
    IEnumerator LoadFarmData()
    {
        yield return new WaitForEndOfFrame();
        if(farmData != null)
        {
            //Load data
            ImportLandData(farmData.Item1);
            ImportCropData(farmData.Item2);
        }
    }
    private void OnDestroy()
    {
        //Save the instance
        farmData = new Tuple<List<LandSaveStatement>, List<CropSaveState>>(landData, cropData);
    }
    #region Registering and Deregistering
    void RegisterLandPlots()
    {
        foreach(Transform landTransform in transform)
        {
            Land land = landTransform.GetComponent<Land>();
            landPlots.Add(land);

            landData.Add(new LandSaveStatement());

            land.id = landPlots.Count - 1;
        }
    }
    public void RegisterCrop(int landID, string seedToGrow, CropBehaviour.CropState cropState, int growth, int health)
    {
        cropData.Add(new CropSaveState(landID, seedToGrow, cropState, growth, health));
    }
    public void DeRegisterCrop(int landID)
    {
        cropData.RemoveAll(x => x.landID == landID);
    }
    #endregion
    #region State Changes
    public void OnLandStateChange(int id,Land.LandStatus landStatus, GameTimeStamp lastWatered)
    {
        landData[id] = new LandSaveStatement(landStatus, lastWatered);
    }

    public void OnCropStateChange(int landID, CropBehaviour.CropState cropState, int growth, int health)
    {
        int cropIndex = cropData.FindIndex(x => x.landID == landID);

        string seedToGrow = cropData[cropIndex].seedToGrow;
        cropData[cropIndex] = new CropSaveState(landID, seedToGrow, cropState, growth, health);
    }
    #endregion
    #region Loading Data
    //Load the FarmData
    public void ImportLandData(List<LandSaveStatement> landDatasetToLoad)
    {
        for(int i = 0; i < landDatasetToLoad.Count; i++)
        {
            //Get the individual land save state 
            LandSaveStatement landDataToLoad = landDatasetToLoad[i];
            //Load it up onto the land instance
            landPlots[i].LoadLandData(landDataToLoad.landStatus, landDataToLoad.lastWatered);
        }
        landData = landDatasetToLoad; 
    }
    public void ImportCropData(List<CropSaveState> cropDataSetToLoad)
    {
        cropData = cropDataSetToLoad;
        foreach(CropSaveState cropSave in cropDataSetToLoad)
        {
            //Access the land
            Land landToPlant = landPlots[cropSave.landID];
            //Spawn the crop
            CropBehaviour cropToPlant = landToPlant.SpawnCrop();
            //Load in the data
            SeedData seedToGrow =(SeedData) InventoryManager.Instance.itemIndex.GetItemFromString(cropSave.seedToGrow);
            cropToPlant.LoadCrop(cropSave.landID, seedToGrow, cropSave.cropState, cropSave.growth, cropSave.health);
        }
        
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        
    }
}
