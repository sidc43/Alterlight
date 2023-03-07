using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{

    [Header("Only gameplay")]
    public float useTime; // The amount of time the item will appear on screen (for weapons when you use them)

    [Range(1, 100)]
    public int stackSize;
    public MainType mainType;

    [EnableIf("mainType", MainType.Tool)]
    public ItemType type;

    [EnableIf("mainType", MainType.Tile)]
    public int defaultTileHealth;
    [EnableIf("mainType", MainType.Tile)]
    public int tileHealth;
    [EnableIf("mainType", MainType.Tool)]
    public int damage;
    [EnableIf("mainType", MainType.Tool)]
    public int blockDamage;
    [EnableIf("mainType", MainType.Tool)]
    public ActionType[] actionType;
    [EnableIf("mainType", MainType.Tool)]

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

public enum MainType
{
    Tile,
    Tool, // All weapons, tools, anything that does damage or breaks stuff
    Consumable
}

public enum ItemType 
{
    Axe,
    Pickaxe,
    Shovel,
    Melee,
    Ranged,
    Magic
}

public enum ActionType 
{
    Dig, // Shovel
    Mine, // Pickaxe, drill, etc
    Till, // Hoe
    Forage, // Axes
    Attack // Weapons
}
