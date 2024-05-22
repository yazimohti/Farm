using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Search;

public class InventorySlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    ItemData itemToDisplay;
    public Image itemDisplayImage;

    public enum InventoryType
    {
        Tool,Item
    }
    public InventoryType inventoryType;
    int slotIndex;

    //Check if there is an item to display
    public void Display(ItemData itemToDisplay)
    {
        if(itemToDisplay != null)
        {
            //Switch the thumbnail
            itemDisplayImage.sprite = itemToDisplay.thumbnail;
            this.itemToDisplay = itemToDisplay;

            itemDisplayImage.gameObject.SetActive(true);

            return;
        }

        itemDisplayImage.gameObject.SetActive(false);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        InventoryManager.Instance.InventoryToHand(slotIndex,inventoryType);
    }
    //Set the slot index
    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }
    //Display the item info on the item info box when the player mouses over
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.DipslayItemInfo(itemToDisplay);
    }
    //Reset the item info box when the player leaves
    public void OnPointerExit(PointerEventData eventData)
    {
      UIManager.Instance.DipslayItemInfo(null);  
    }
}
