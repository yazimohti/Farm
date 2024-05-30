using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
    public ItemIndex itemIndex;
    [Header("Tools")]
    //Tool Slots
    [SerializeField] private ItemSlotData[] toolSlots = new ItemSlotData[8];
    //Tool in the player's hand
    [SerializeField] private ItemSlotData equippedToolSlot = null;
    [Header("Items")]
    //Item Slots
    [SerializeField] private ItemSlotData[] itemSlots = new ItemSlotData[8];
    //Item in the player's hand
    [SerializeField] private ItemSlotData equippedItemSlot = null;

    //The transform for the player to hold itemSlots in the scene
    public Transform handPoint;

    //Load the inventory from a save
    public void LoadInventory(ItemSlotData[] toolSlots, ItemSlotData equippedToolSlot, ItemSlotData[] itemSlots, ItemSlotData equippedItemSlot)
    {
        this.toolSlots = toolSlots;
        this.equippedToolSlot = equippedToolSlot;
        this.itemSlots = itemSlots;
        this.equippedItemSlot = equippedItemSlot;

        //Updates the changes in the UI
        UIManager.Instance.RenderInventory();
    }
    //Equipping

    //Handles movement of item from inventory
    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {   
        //The slot to equip (Tool by default)
        ItemSlotData handToEquip = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            //Change the slot to item
            handToEquip = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }
        //Check if stackable
        if(handToEquip.Stackable(inventoryToAlter[slotIndex]))
        {
            ItemSlotData slotToAlter = inventoryToAlter[slotIndex];

            //Add to the hand slot
            handToEquip.AddQuantity(slotToAlter.quantity);

            //Empty the inventory slot
            slotToAlter.Empty();


        }
        else
        {
            //Not stackable
            //Cache the Inventory ItemSlotData
            ItemSlotData slotToEquip = new ItemSlotData(inventoryToAlter[slotIndex]);

            //Change the inventory slot to the hands
            inventoryToAlter[slotIndex] = new ItemSlotData(handToEquip);

            EquipHandSlot(slotToEquip);

            
        }
        //Updates the changes in the scene
        if(inventoryType == InventorySlot.InventoryType.Item) 
        {
            RenderHand();
        }
        //Updates the changes to the UI
        UIManager.Instance.RenderInventory();
    }

    //Handles movement of item from Hand to Inventory
    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {
        //The slot to move from (Tool by default)
        ItemSlotData handSlot = equippedToolSlot;
        //The array to change
        ItemSlotData[] inventoryToAlter = toolSlots;

        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            handSlot = equippedItemSlot;
            inventoryToAlter = itemSlots;
        }
        //Try stacking the hand slot
        //Check if the operation failed
        if(!StackItemToInventory(handSlot, inventoryToAlter))
        {
            //Find an empty slot to put the item in
            //Iterate through each Inventory slot and find empty slot
            for(int i=0; i<inventoryToAlter.Length; i++)
            {
                if(inventoryToAlter[i].IsEmpty())
                {   
                    //Send the equipped item over
                    inventoryToAlter[i]=new ItemSlotData(handSlot);
                    //Remove the item from the hand
                    handSlot.Empty();
                    break;
                }
            }
        }


        //Updates the changes in the scene
        if(inventoryType == InventorySlot.InventoryType.Item) 
        {
            RenderHand();
        }
        //Updates the changes to the UI
        UIManager.Instance.RenderInventory();

        
    }

    //Iterate through each of the items in the inventory to see if it can be stacked
    //Will perform the operation if found, returns false if unsuccessful
    public bool StackItemToInventory(ItemSlotData itemSlot, ItemSlotData[] inventoryArray)
    {
        for(int i = 0; i < inventoryArray.Length; i++)
        {
            if(inventoryArray[i].Stackable(itemSlot))
            {
                //Add to the inventory slot's stack
                inventoryArray[i].AddQuantity(itemSlot.quantity);
                //Empty the item slot
                itemSlot.Empty();
                return true;
            }
        }

        //Can't find any slot that can be stacked
        return false;
    }

    public void RenderHand()
    {
        //Reset objects on the hand
        if(handPoint.childCount > 0)
        {
            Destroy(handPoint.GetChild(0).gameObject);
        }
        
        //Check if player has anything equipped
        if(SlotEquipped(InventorySlot.InventoryType.Item))
        {
            //Instantiate the gamemodel on the player's hand and put it on the scene
            Instantiate(GetEquippedSlotItem(InventorySlot.InventoryType.Item).gameModel,handPoint);
        }
    }

    //Inventory Slot Data
    #region Gets and Checks
    public ItemData GetEquippedSlotItem(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot.itemData;
        }
        return equippedToolSlot.itemData;
    }

    //Get function for the slots (ItemSlotData)
    public ItemSlotData GetEquippedSlot(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return equippedItemSlot;
        }
        return equippedToolSlot;
    }
    //Get the function for the inventory slots
    public ItemSlotData[] GetInventorySlots(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return itemSlots;
        }
        return toolSlots;
    }
    
    //Check if a hand slot has an item
    public bool SlotEquipped(InventorySlot.InventoryType inventoryType)
    {
        if(inventoryType == InventorySlot.InventoryType.Item)
        {
            return !equippedItemSlot.IsEmpty();
        }
        return !equippedToolSlot.IsEmpty();
    }
    //Check if the item is a tool
    public bool IsTool(ItemData item)
    {
        //Is it equipment?
        //Try to cast it as equipment first
        EquipmentData equipment = item as EquipmentData;
        if(equipment != null)
        {
            return true;
        }
        //Is it a seed?
        //Try to cast it as a seed
        SeedData seed = item as SeedData;
        //If the seed is not null it is a seed
        return seed != null;
    }
    #endregion

    //Equip the hand slot with an ItemData(Will overwrite the slot) 
    public void EquipHandSlot(ItemData item)
    {
        if(IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(item);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(item);
        }
    }
    //Equip the hand slot with an ItemSlotData(Will overwrite the slot) 
    public void EquipHandSlot(ItemSlotData itemSlot)
    {
        //Get the item data from itemslot
        ItemData item = itemSlot.itemData;
        if(IsTool(item))
        {
            equippedToolSlot = new ItemSlotData(itemSlot);
        }
        else
        {
            equippedItemSlot = new ItemSlotData(itemSlot);
        }
    }
    public void ConsumeItem(ItemSlotData itemSlot)
    {
        if(itemSlot.IsEmpty())
        {
            Debug.LogError("There is nothing to consume");
            return;
        }
        //Use up one of the item slots
        itemSlot.Remove();
        //Refresh inventory
        RenderHand();
        UIManager.Instance.RenderInventory();
    }
    #region Inventory Slot Validation
    private void OnValidate()
    {
        //Validate the hand slots
        ValidateInventorySlot(equippedToolSlot);
        ValidateInventorySlot(equippedItemSlot);

        //Validate the slots in inventory
        ValidateInventorySlots(itemSlots);
        ValidateInventorySlots(toolSlots);
    }

    void ValidateInventorySlot(ItemSlotData slot)
    {
        if(slot.itemData != null && slot.quantity == 0)
        {
            slot.quantity = 1;
        }
    }
    void ValidateInventorySlots(ItemSlotData[] array)
    {
        foreach(ItemSlotData slot in array)
        {
            ValidateInventorySlot(slot);
        }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
