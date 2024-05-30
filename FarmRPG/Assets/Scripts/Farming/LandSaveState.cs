using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LandSaveState 
{
    public Land.LandStatus landStatus;
    public GameTimeStamp lastWatered;

    public LandSaveState(Land.LandStatus landStatus, GameTimeStamp lastWatered)
    {
        this.landStatus = landStatus;
        this.lastWatered = lastWatered;
    }
}
