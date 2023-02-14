using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static ExtensionMethods.ExtensionMethods;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public TextMeshProUGUI countText;
    public Item item;
    [HideInInspector] public int count = 1;
    [HideInInspector] public Transform parentAfterDrag;
    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.itemObject.GetComponent<SpriteRenderer>().sprite; 
        RerfreshCount();
    }
    public void RerfreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = this.transform.parent;
        this.transform.SetParent(transform.root);
        this.transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
}
