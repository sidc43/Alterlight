using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ExtensionMethods.ExtensionMethods;

public class CraftingManager : MonoBehaviour
{
    public InventoryManager inventory;
    public InventorySlot[] craftingSlots;
    public InventorySlot resultSlot;
    public List<Item> allItems;
    public List<Item> itemsInCraftingSlots;
    public List<int> correspondingCount;
    private Dictionary<Item, int> itemsAndCountInCraftingSlot = new Dictionary<Item, int>();
    public List<int> correspondingAmountToRemove = new List<int>();
    
    private void Update()
    {
        HandleCrafting();
        EmptyAllCollections(); //? If all crafting slots are not filled, empty all collections
    }
    private void HandleCrafting()
    {
        if (AllCraftingSlotsFilled())
        {
            UpdateCraftingItemsDict();
            if (Input.GetKeyDown(KeyCode.C)) 
            {
                Item item = CheckRecipe();
                if (item != null)
                {
                    inventory.SpawnNewItem(item, resultSlot);
                    UseCraftingItems();
                }
            }
        }
    }
    private bool AllCraftingSlotsFilled()
    {
        List<bool> c = new List<bool>();
        foreach (InventorySlot slot in craftingSlots)
            c.Add(slot.GetItemInSlot() != null);

        return !c.Contains(false);
    }
    private Item CheckRecipe()
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            // Check dictionaries
            bool hasItemsAndAmount = DictionaryEquals(itemsAndCountInCraftingSlot, allItems[i].reqItemsAndCount);
            
            if (hasItemsAndAmount)
            {
                foreach (var amount in allItems[i].reqItemsAndCount)
                    correspondingAmountToRemove.Add(amount.Value);
                return allItems[i];
            }
        }
        return null;
    }
    private void UseCraftingItems()
    {
        int i = 0;
        foreach (InventorySlot slot in craftingSlots)
        {
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null)
            {
                itemInSlot.count -= correspondingAmountToRemove[i];
                if (itemInSlot.count <= 0)
                    Destroy(itemInSlot.gameObject);
                else
                    itemInSlot.RerfreshCount();
            }
            i++;
        }
    }
    private void UpdateCraftingItemsDict()
    {
        if (itemsInCraftingSlots.Count <= 2)
        {
            if (AllCraftingSlotsFilled()) 
            {
                for (int i = 0; i < craftingSlots.Length; i++)
                {
                    itemsInCraftingSlots.Add(craftingSlots[i].GetItemInSlot());
                    correspondingCount.Add(craftingSlots[i].GetComponentInChildren<InventoryItem>().count);
                }

                for (int j = 0; j < itemsInCraftingSlots.Count; j++)
                    itemsAndCountInCraftingSlot.Add(itemsInCraftingSlots[j], correspondingCount[j]);
            }
        }
    }
    private void EmptyAllCollections()
    {
        if (!AllCraftingSlotsFilled())
        {
            itemsInCraftingSlots.Clear();
            correspondingCount.Clear();
            itemsAndCountInCraftingSlot.Clear();
            correspondingAmountToRemove.Clear();
        }
    }
}