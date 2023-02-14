using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ExtensionMethods.ExtensionMethods;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public PlayerMovement player;
    public GameObject inventoryItemPrefab;
    private List<Item> _items; // Backing field
    public List<Item> items; // Distinct list 
    int selectedSlot = -1;
    private int maxCount = 5;
    private int maxHotBarIndex = 6;

    private void Start() 
    {
        ChangeSelectedSlot(0);
        _items = new List<Item>();
    }
    private void Update() 
    {
        int count = selectedSlot;

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // forward
        {
            if (count > maxCount)
                count = 0;
            else
                count++;
            
            ChangeSelectedSlot(count);
            
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // backward
        {
            if (count <= 0)
                count = maxHotBarIndex;
            else    
                count--;

            ChangeSelectedSlot(count);
        }
    }
    private void ChangeSelectedSlot(int newVal)
    {
        if (selectedSlot >= 0)
            inventorySlots[selectedSlot].Deselect();

        inventorySlots[newVal].Select();
        selectedSlot = newVal;
    }
    public bool AddItem(Item item)
    {
        // Check for full inv
        if (items.Count > inventorySlots.Length)
        {
            Print("Inventory full");
            return false;
        }

        // Check if any slot has the same item with count lower than max
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < itemInSlot.item.stackSize && itemInSlot.item.stackable)
            {
                itemInSlot.count++;
                itemInSlot.RerfreshCount();

                _items.Add(item);
                items = _items.Distinct().ToList();
                return true;
            }
        }

        // Find empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);

                _items.Add(item);
                items.Add(item);
                return true;
            }
        }
        return false;
    }
    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }
    public Item GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if (use)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                    Destroy(itemInSlot.gameObject);
                else
                    itemInSlot.RerfreshCount();
            }

            return item;
        }

        return null;
    }
    public bool AnyInHotbar()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        return itemInSlot != null;
    }
}
