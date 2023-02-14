using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDropController : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item item;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "player")
        {
            Destroy(this.gameObject);
            inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            inventoryManager.AddItem(item);
        }
    }
}
