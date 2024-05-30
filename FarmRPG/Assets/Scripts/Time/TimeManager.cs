using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance {get; private set; }
    [SerializeField]
    public GameTimeStamp timeStamp;
    public float timeScale = 1.0f;

    //The transform of the directional light(sun)
    public Transform sunTransform;

    //Observer pattern 
    List<ITimeTracker> listeners = new List<ITimeTracker>();
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
    void Start()
    {   
        //Initialise the time stamp
        timeStamp = new GameTimeStamp(0, GameTimeStamp.Season.Spring, 1, 6, 0);
        StartCoroutine(TimeUpdate());
    }

    //Load the time from a save
    public void LoadTime(GameTimeStamp timeStamp)
    {
        this.timeStamp = new GameTimeStamp(timeStamp);
    }
    IEnumerator TimeUpdate()
    {
        while(true)
        {
            Tick();
            yield return new WaitForSeconds(1/timeScale);
        }
    }
    //A tick of the in-game time
    public void Tick()
    {
        timeStamp.UpdateClock();

        //Inform the listeners of the new time states
        foreach(ITimeTracker listener in listeners)
        {
            listener.UpdateClock(timeStamp);
        }
        UpdateSunMovement();  
    }
    public void SkipTime(GameTimeStamp timeToSkip)
    {
        int timeToSkipInMinutes = GameTimeStamp.TimeStampInMinutes(timeToSkip);
        Debug.Log("Skip time: " + timeToSkipInMinutes);
        int timeNowInMinutes = GameTimeStamp.TimeStampInMinutes(timeStamp);
        Debug.Log("Time Now:" + timeNowInMinutes);
        int differenceInMinutes = timeToSkipInMinutes - timeNowInMinutes;
        Debug.Log("Difference:" + differenceInMinutes);

        //Check if the timestamp to skip to has already been reached
        if(differenceInMinutes <= 0) return;

        for(int i = 0; i < differenceInMinutes; i++)
        {
            Tick();
        }
    }

    //Day and Night cycle
    void UpdateSunMovement()
    {
        
        //Convert the current time to minutes
        int timeInMinutes = GameTimeStamp.HoursToMinutes(timeStamp.hour) + timeStamp.minute;

        //Sun moves 15 degree in an hour
        //.25 degree in a minute
        //At midnight (0:00), the angle of the sun shoul be -90
        float sunAngle = .25f * timeInMinutes - 90;

        //Apply the angle to the directional light (sun)
        sunTransform.eulerAngles = new Vector3(sunAngle,0,0);
    }

    //Get the TimeStamp
    public GameTimeStamp GetGameTimeStamp()
    {   
        //Return a clone instance
        return new GameTimeStamp(timeStamp);
    }

    //Handling listeners

    //Add the object to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    //Remove the object from the list of listeners
    public void UnRegisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
    
}
