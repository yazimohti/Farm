using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance {get; private set; }
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
    [Header("Tools")]
    //Tool Slots
    public ItemData[] tools = new ItemData[8];
    //Tool in the player's hand
    public ItemData equippedTool = null;
    [Header("Items")]
    //Item Slots
    public ItemData[] items = new ItemData[8];
    //Item in the player's hand
    public ItemData equippedItem = null;
    //Equipping

    //Handles movement of item from inventory
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Cache the Inventory Slot ItemData from InventoryManager
            ItemData itemToEquip = items[slotIndex];
            //Change the inventory slot to the Hand's
            items[slotIndex] = equippedItem;
            //Change the Hand's Slot to the Inventory Slot's
            equippedItem = itemToEquip; 
        }
        else
        {
            //Cache the Inventory Slot ItemData from InventoryManager
            ItemData toolToEquip = tools[slotIndex];
            //Change the inventory slot to the Hand's
            tools[slotIndex] = equippedTool;
            //Change the Hand's Slot to the Inventory Slot's
            equippedTool = toolToEquip;
        }
        //Updates the changes
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of item from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Iterate through each Inventory slot and find empty slot
            for(int i=0; i<items.Length; i++)
            {
                if(items[i]==null)
                {   
                    //Send the equipped item over
                    items[i]=equippedItem;
                    //Remove the item from the hand
                    equippedItem = null;
                    break;
                }
            }
            
        }
        else
        {
            //Iterate through each Inventory slot and find empty slot
            for(int i=0; i<tools.Length; i++)
            {
                if(tools[i]==null)
                {
                    tools[i]=equippedTool;
                    //Remove the tool from the hand
                    equippedTool = null;
                    break;
                }
            }
            
        }
        UIManager.Instance.RenderInventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
