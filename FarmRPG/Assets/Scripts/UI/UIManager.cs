using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set; }

    [Header("Status Bar")]
    //Tool equip status on the bar
    public Image toolEquipSlot;
    [Header("Inventory System")]
    //Inventory Panel
    public GameObject InventoryPanel;
    //The tool equip slot UI on the Inventory Panel
    public HandToInventory toolHandSlot;
    //The tool slot UIs
    public InventorySlot[] toolSlots;
    //The item equip slot UI on the Inventory Panel
    public HandToInventory itemHandSlot;
    //The item slot UIs
    public InventorySlot[] itemSlots;
    //Item info box
    public Text itemNameText;
    public Text itemDescriptionText;
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
    private void Start()
    {
        RenderInventory();
        AssignSlotIndex();
    }

    public void AssignSlotIndex()
    {
        for(int i=0;i<toolSlots.Length;i++)
        {
            toolSlots[i].AssignIndex(i);
            itemSlots[i].AssignIndex(i);
        }
    }

    //Render the inventory screen to reflect the Player's Inventory
    public void RenderInventory()
    {
        //Get the inventory tool slots from Inventory Manager
        ItemData[] inventoryToolSlots = InventoryManager.Instance.tools;

        //Get the inventory item slots from Inventory Manager
        ItemData[] inventoryItemSlots = InventoryManager.Instance.items;
        //Render the Tool section
        RenderInventoryPanel(inventoryToolSlots,toolSlots);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots,itemSlots);

        //Render the equipped tool slots
        toolHandSlot.Display(InventoryManager.Instance.equippedTool);

        //Render the equipped item slots
        itemHandSlot.Display(InventoryManager.Instance.equippedItem);
        
        //Get tool equip from InventoryManager
        ItemData equippedTool = InventoryManager.Instance.equippedTool;

        //Check if there is an item to display
        if(equippedTool != null)
        {
            //Switch the thumbnail
            toolEquipSlot.sprite = equippedTool.thumbnail;
            
            toolEquipSlot.gameObject.SetActive(true);
            return;
        }
        toolEquipSlot.gameObject.SetActive(false);
    }
    public void RenderInventoryPanel(ItemData[] slots, InventorySlot[] UISlots)
    {
        for(int i=0;i<UISlots.Length;i++)
        {
            //Display them accordingly
            UISlots[i].Display(slots[i]);
        }
    }
    public void ToggleInventoryPanel()
    {
        //If the panel is hidden, show it and vice versa
        InventoryPanel.SetActive(!InventoryPanel.activeSelf);

        RenderInventory();
    }

    //Display Item info on the Item infobox
    public void DipslayItemInfo(ItemData data)
    {   
        //If data is null, reset
        if(data == null)
        {
            itemNameText.text ="";
            itemDescriptionText.text ="";
            
            return;
        }
        itemNameText.text = data.name;
        itemDescriptionText.text = data.description;
    } 
}
