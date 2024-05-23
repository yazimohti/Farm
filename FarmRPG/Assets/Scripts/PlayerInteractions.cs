using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{  
    Land selectedLand = null;
    PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down,out hit, 1))
        {
            OnInteractableHit(hit);
        }

    }
    //Handles what happens when the interaction raycast hits something interactable
    void OnInteractableHit(RaycastHit hit)
    {
        Collider other = hit.collider;
        //Check if the player is going to interact with land
        if(other.tag == "Land")
        {
            //Get the land component
            Land land = other.GetComponent<Land>();
            SelectLand(land);
            return;
        }
        //Deselect the land if the player is not standing on any land at the moment
        if(selectedLand !=null)
        {
            selectedLand.Select(false);
            selectedLand = null;
        }
    }
    //Handles the selection process
    void SelectLand(Land land)
    {
        if(selectedLand != null)
        {
            selectedLand.Select(false);
        }
        //Set the new selected land to the land we're selecting now
        selectedLand = land;
        land.Select(true);
    }
    //Triggered when the player presses the interact key
    public void Interact()
    {
        //Check if the player is selecting any land
        if(selectedLand != null)
        {
            selectedLand.Interact();
            return;
        }
        Debug.Log("Interact");
    }
    public void ItemInteract()
    {
        //If the player is holding something, keep it in his inventory
        if(InventoryManager.Instance.equippedItem != null)
        {
            InventoryManager.Instance.HandToInventory(InventorySlot.InventoryType.Item);
            return;
        }
    }
}
