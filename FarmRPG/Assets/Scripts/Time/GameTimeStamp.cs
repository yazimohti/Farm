using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameTimeStamp 
{
    public int year;
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    public Season season;
    public enum DaysOfTheWeek
    {
        Saturday,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }
    public DaysOfTheWeek daysOfTheWeek;
    public int day;
    public int hour;
    public int minute;

    //Constructor to set up the Class
    public GameTimeStamp(int year, Season season, int day, int hour, int minute)
    {
        this.year = year;
        this.season = season;
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    //Creating a new instance of a GameTimeStamp from another pre-existing one
    public GameTimeStamp(GameTimeStamp timeStamp)
    {
        this.year = timeStamp.year;
        this.season = timeStamp.season;
        this.day = timeStamp.day;
        this.hour = timeStamp.hour;
        this.minute = timeStamp.minute;
    }

    //Increase the time by 1 minute    
    public void UpdateClock()
    {
        minute++;
        if(minute >= 60)
        {
            //Reset minutes
            minute = 0;
            hour++;
        }

        if(hour >= 24)
        {
            //Reset Hours
            hour = 0;
            day++;
        }

        if(day > 30)
        {
            //Reset Days
            day = 1;

            //If at the Final Season, reset and change to spring 
            if(season == Season.Winter)
            {
                season = Season.Spring;
                //Start of new year
                year++;
            }
            else
            {
                season++;
            }

        }
    }

    public DaysOfTheWeek GetDaysOfTheWeek()
    {
        //Convert the total time passed into days
        int dayPassed = YearsToDays(year) + SeasonsToDays(season) + day;

        //Remainder after dividing daysPassed by 7
        int dayIndex = dayPassed % 7;

        //Cast into day of the week
        return (DaysOfTheWeek)dayIndex;
    }

    //Convert hours to minutes
    public static int HoursToMinutes(int hour)
    {
        // 60 minutes = 1 hour
        return hour * 60;
    }

    //Convert days to hours
    public static int DaysToHours(int day)
    {
        // 24 hours = 1 day
        return day * 24;
    }

    //Convert Seasons to days
    public static int SeasonsToDays(Season season)
    {
        int seasonIndex = (int)season;
        return seasonIndex * 30;
    }

    //Convert years to day
    public static int YearsToDays(int year)
    {
        return year * 4 * 30;
    }

    public static int CompareTimeStamps(GameTimeStamp timeStamp1, GameTimeStamp timeStamp2)
    {   
        //Convert timestamps to hours
        int timeStamp1Hours = DaysToHours(YearsToDays(timeStamp1.year)) + DaysToHours(SeasonsToDays(timeStamp1.season)) + DaysToHours(timeStamp1.day) + timeStamp1.hour;
        int timeStamp2Hours = DaysToHours(YearsToDays(timeStamp2.year)) + DaysToHours(SeasonsToDays(timeStamp2.season)) + DaysToHours(timeStamp2.day) + timeStamp2.hour;
        int difference = timeStamp1Hours-timeStamp2Hours;
        return Math.Abs(difference);
    }
}
