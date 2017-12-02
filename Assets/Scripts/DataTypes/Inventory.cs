using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class Inventory
{
    public GameObject inventory;
    [InlineButton("ToggleInventory", "Click")]
    public Toggle inventoryToggle;

    [ListDrawerSettings(NumberOfItemsPerPage = 4, ShowIndexLabels = true, DraggableItems = false)]
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private GlobalState globalState;


    public Inventory() { }

    public void LoadFromState(GlobalState globalState)
    {
        this.globalState = globalState;

        // TODO: Replace with solution that will keep in mind index of the slot
        this.AddItems(this.globalState.inventory);
    }

    public void AddItems(List<Item> items)
    {
        items.ForEach(item => this.AddItem(item));
    }

    public void AddItem(Item item)
    {
        InventorySlot firstFreeSlot = this.inventorySlots.Find(slot => slot.item == null);

        if (firstFreeSlot != default(InventorySlot))
        {
            this.globalState.inventory.Add(item);
            firstFreeSlot.AddItem(item);
        }
        else
        {
            Debug.Log("No free slots in inventory left");
        }
    }

    public bool HasItem(ItemId itemId)
    {
        return this.globalState.inventory.Exists(item => item.id == itemId);
    }

    [ButtonGroup("Slot Availability")]
    [Button(ButtonSizes.Medium)]
    public void EnableAllSlots()
    {
        foreach (InventorySlot slot in this.inventorySlots)
        {
            slot.enabled = true;
            slot.slotButton.interactable = true;
        }
    }

    [ButtonGroup("Slot Availability")]
    [Button(ButtonSizes.Medium)]
    public void DisableAllSlots()
    {
        foreach (InventorySlot slot in this.inventorySlots)
        {
            slot.enabled = false;
            slot.slotButton.interactable = false;
        }
    }

    public void ToggleInventory()
    {
        this.inventoryToggle.isOn = !this.inventoryToggle.isOn;
        this.inventory.SetActive(this.inventoryToggle.isOn);
    }
}


public class InventorySlot
{
    [ReadOnly]
    public bool enabled = false;
    public Item item;

    [Required]
    public Image slotImage;
    [Required]
    public Button slotButton;


    public InventorySlot() { }

    [ButtonGroup("Slot Availability")]
    [Button]
    public void EnableSlot()
    {
        this.enabled = true;
        this.slotButton.interactable = true;
    }

    [ButtonGroup("Slot Availability")]
    [Button]
    public void DisableSlot()
    {
        this.enabled = false;
        this.slotButton.interactable = false;
    }

    public void AddItem(Item item)
    {
        this.item = item;
        // Update UI
        this.slotImage.sprite = this.item.icon;
        this.slotImage.gameObject.SetActive(true);
    }
}
