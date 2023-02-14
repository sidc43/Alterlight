using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{

    [Header("Only gameplay")]
    public float useTime; // The amount of time the item will appear on screen (for weapons when you use them)

    [Range(1, 100)]
    public int stackSize;
    public ItemType type;
    public int damage;
    public ActionType[] actionType;
    public Vector2Int range = new Vector2Int(3, 3);

    [Header("Only UI")]
    public bool stackable = true;

    [Header("Both")]
    public GameObject itemObject;   

    [Header("Crafting")]
    public List<Item> requiredItemsToCraft;
    public List<int> correspondingCount;
    public Dictionary<Item, int> reqItemsAndCount = new Dictionary<Item, int>();

    private void OnEnable()
    {
        for (int i = 0; i < requiredItemsToCraft.Count; i++)
            reqItemsAndCount.Add(requiredItemsToCraft[i], correspondingCount[i]);
    }
}

public enum ItemType 
{
    BuildingBlock,
    Tool,
    Melee,
    Ranged,
    Magic,
    Consumable
}

public enum ActionType 
{
    Dig, // Shovel
    Mine, // Pickaxe, drill, etc
    Till, // Hoe
    Forage, // Axes
    Attack // Weapons
}
