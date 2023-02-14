using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, unselectedColor;

    private void Awake() 
    {
        Deselect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }
    public Item GetItemInSlot()
    {
        Item item = null;

        try
        {
            item = this.GetComponentInChildren<InventoryItem>().item;
        }
        catch (NullReferenceException n)
        {
            if (n.Data == null)
                throw;
        }

        return item;
    }
    public void Deselect()
    {
        image.color = unselectedColor;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            InventoryItem dropped = eventData.pointerDrag.GetComponent<InventoryItem>();
            dropped.parentAfterDrag = transform;
        }
    }
}
