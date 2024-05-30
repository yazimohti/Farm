using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    public override void PickUp()
    {
        UIManager.Instance.TriggerYesNoPrompt("Do you want to sleep?", Sleep);
    }
    public void Sleep()
    {
        GameTimeStamp timeStampOfNextDay = TimeManager.Instance.GetGameTimeStamp();
        timeStampOfNextDay.day +=1;
        timeStampOfNextDay.hour = 6;
        timeStampOfNextDay.minute = 0;

        Debug.Log(timeStampOfNextDay.day + ":" + timeStampOfNextDay.hour + ":" + timeStampOfNextDay.minute);

        TimeManager.Instance.SkipTime(timeStampOfNextDay);
    }
}
