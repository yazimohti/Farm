using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LandSaveStatement 
{
    public Land.LandStatus landStatus;
    public GameTimeStamp lastWatered;

    public LandSaveStatement(Land.LandStatus landStatus, GameTimeStamp lastWatered) 
    {
        this.landStatus = landStatus;
        this.lastWatered = lastWatered;
    }

    public void UpdateClock(GameTimeStamp timeStamp)
    {
        //Checked if 24 hours has passed since last watered 
        if(landStatus == Land.LandStatus.Watered)
        {   
            //Hours since the land was watered 
            int hoursElapsed = GameTimeStamp.CompareTimeStamps(lastWatered,timeStamp);
            
            if(hoursElapsed > 24)
            {
                //Dry up 
                landStatus = Land.LandStatus.Farmland;
            }
        }
        
    }
}
