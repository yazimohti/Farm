using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocationEntryPoint : MonoBehaviour
{
    [SerializeField]
    SceneTransitionManager.Location locationToSwitch;

    private void OnTriggerEnter(Collider other)
    {
        //Check if the collider belongs to player
        if(other.tag == "Player")
        {
            //Switch scene to the desired scene
            SceneTransitionManager.Instance.SwitchLocation(locationToSwitch);
        }
    }
}
