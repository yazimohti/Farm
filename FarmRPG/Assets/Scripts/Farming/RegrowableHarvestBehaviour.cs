using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowableHarvestBehaviour : InteractableObject
{
    CropBehaviour parentCrop;

    //Sets the parent crop
    public void SetParent(CropBehaviour parentCrop)
    {
        this.parentCrop = parentCrop;
    }
    public override void PickUp()
    {
         //Set the player's inventory to the item
        InventoryManager.Instance.EquipHandSlot(item);
        //Updates the change in the scene
        InventoryManager.Instance.RenderHand();

        //Set the parent crop back to seedling to regrow it
        parentCrop.Regrow();
    }
}
