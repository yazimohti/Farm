using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITimeTracker
{
    public static UIManager Instance {get; private set; }

    [Header("Status Bar")]
    //Tool equip status on the bar
    public Image toolEquipSlot;
    //Tool quantity status on the bar
    public Text toolQuantityText;

    //Time UI
    public Text timeText;
    public Text dateText;
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
    [Header("Screen Transition")]
    public GameObject fadeIn;
    public GameObject fadeOut;

    [Header("Yes No Prompt")]
    public YesNoPrompt yesNoPrompt;
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

        //Add UIManager to the list of objects TimeManager will notify when the time updates 
        TimeManager.Instance.RegisterTracker(this);
    }
    #region FadeIn and FadeOut

    public void FadeOutScreen()
    {
        fadeOut.SetActive(true);
    }
    public void FadeInScreen()
    {
        fadeIn.SetActive(true);
    }
    public void OnFadeInComplete()
    {
        //Disable fadeIn screen when animation finished
        fadeIn.SetActive(false);
    }
    public void ResetFadeDefaults()
    {
        fadeOut.SetActive(false);
        fadeIn.SetActive(true);
    }



    #endregion
    public void TriggerYesNoPrompt(string message, System.Action onYesCallback)
    {
        yesNoPrompt.gameObject.SetActive(true);

        yesNoPrompt.CreatePrompt(message, onYesCallback);
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
        //Get the respective slots to process
        ItemSlotData[] inventoryToolSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Tool);
        ItemSlotData[] inventoryItemSlots = InventoryManager.Instance.GetInventorySlots(InventorySlot.InventoryType.Item);
        //Render the Tool section
        RenderInventoryPanel(inventoryToolSlots,toolSlots);

        //Render the Item section
        RenderInventoryPanel(inventoryItemSlots,itemSlots);

        //Render the equipped tool slots
        toolHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool));

        //Render the equipped item slots
        itemHandSlot.Display(InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Item));
        
        //Get tool equip from InventoryManager
        ItemData equippedTool = InventoryManager.Instance.GetEquippedSlotItem(InventorySlot.InventoryType.Tool);

        //Text should be empty by default
        toolQuantityText.text = ""; 

        //Check if there is an item to display
        if(equippedTool != null)
        {
            //Switch the thumbnail
            toolEquipSlot.sprite = equippedTool.thumbnail;
            
            toolEquipSlot.gameObject.SetActive(true);

            //Get quantity 
            int toolQuantity = InventoryManager.Instance.GetEquippedSlot(InventorySlot.InventoryType.Tool).quantity;
            if(toolQuantity > 1)
            {
                toolQuantityText.text = toolQuantity.ToString();
            }
            return;
        }
        toolEquipSlot.gameObject.SetActive(false);
    }
    public void RenderInventoryPanel(ItemSlotData[] slots, InventorySlot[] UISlots)
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

    //Callback to handle the UI for time
    public void UpdateClock(GameTimeStamp timeStamp)
    {
        //Handle the time 

        //Get the hours and minutes
        int hours = timeStamp.hour;
        int minutes = timeStamp.minute;

        //AM or PM
        string prefix = "AM ";

        //Convert hours to 12 hour clock 
        if(hours >= 12)
        {
            //Time becomes PM
            prefix = "PM ";
            hours -= 12;
        }

        timeText.text = prefix + hours + ":" + minutes.ToString("00");

        //Handle the Date 
        int day = timeStamp.day;
        string season = timeStamp.season.ToString();
        string daysOfTheWeek = timeStamp.GetDaysOfTheWeek().ToString();

        //Format it for the date text display 
        dateText.text = season + " " + day + " " + "(" + daysOfTheWeek + ")";
    }
}
