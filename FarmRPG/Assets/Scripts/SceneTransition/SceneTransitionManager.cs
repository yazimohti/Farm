using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    public Location currentLocation;
    Transform playerPoint;

    //Check FadeOut scene
    bool screenFadeOut;
    public enum Location {Ranch, PlayerHome}
    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
        //Make the gameobject persistent across scenes
        DontDestroyOnLoad(gameObject);
        

        SceneManager.sceneLoaded += OnLocationLoad;

        //Find the player's transform
        playerPoint = FindObjectOfType<PlayerController>().transform;
    }

    public void SwitchLocation(Location locationToSwitch)
    {
        //Call fadeOut screen
        UIManager.Instance.FadeOutScreen();
        screenFadeOut = false;
        StartCoroutine(ChangeScene(locationToSwitch));
    }
    IEnumerator ChangeScene(Location locationToSwitch)
    {
        //Wait for finish the fadeout
        while(!screenFadeOut)
        {
            yield return new WaitForSeconds(0.1f);
        }
        //Reset the boolean
        screenFadeOut = false;
        UIManager.Instance.ResetFadeDefaults();
        SceneManager.LoadScene(locationToSwitch.ToString());
        
    }
    public void OnFadeOutComplete()
    {
        //Disable fadeIn screen when animation finished
        screenFadeOut = true;
    }

    public void OnLocationLoad(Scene scene, LoadSceneMode mode)
    {
        //The location the player is coming from when the scene loads
        Location oldLocation = currentLocation;

        //Get the new Location by converting the string of our current scene into a location enum value
        Location newLocation = (Location) Enum.Parse(typeof(Location), scene.name);

        //If the player is not coming from any new place, stop the code
        if(currentLocation == newLocation) return;

        //Find the start point
        Transform startPoint = LocationManager.Instance.GetPlayerStartingPosition(oldLocation);
        //Disable the player's Character Controller component
        CharacterController playerCharacter = playerPoint.GetComponent<CharacterController>();
        playerCharacter.enabled = false;

        //Change the player's position to the start point
        playerPoint.position = startPoint.position;
        playerPoint.rotation = startPoint.rotation;
        //Re-enable Character Controller
        playerCharacter.enabled = true;

        //Save the current location that we just switched to 
        currentLocation = newLocation;
    }
}
