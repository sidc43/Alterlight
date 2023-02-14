using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using static ExtensionMethods.ExtensionMethods;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    [Header("Debugging")]
    public TextMeshProUGUI playerPosText;
    public TextMeshProUGUI mousePosText;
    public TextMeshProUGUI hoveringTile;
    public TextMeshProUGUI levelText;

    [Header("Attributes")]
    public const float defaultSpeed = 4f;
    public const float maxHealth = 100;
    public float speed = defaultSpeed;
    public float sprintSpeed = 5.5f;
    public float health = maxHealth;
    public int defense = 5;
    public int exp = 0;
    public int level = 0;
    public Item[] starting;
    private int expForNextLvl;

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    [Header("Functionality")]
    public Rigidbody2D rb;
    public Animator animator;
    public Vector2 movement;
    GameObject itemOnScreen = null;
    public Vector2Int mousePos;
    public TerrainGeneration terrainGenerator;
    public Camera cam;
    public Vector2 lastDirFaced;
    //private float useTimeCounter = 0;
    #endregion

    void Start()
    {
        foreach (Item i in starting)
            inventoryManager.AddItem(i);
    }

    void Update()
    {
        GetMousePosition();
        HandlePlayerMovement();
        UpdateDebuggingText();

        Item item = null;
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (leftClick && inventoryManager.AnyInHotbar())
            HandleLeftClickActions(item);

        if (rightClick && inventoryManager.AnyInHotbar())
            HandleRightClickActions(item);
    }
    void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
    public void CheckLevelUp()
    {
        expForNextLvl = 20;

        if (exp >= expForNextLvl)
        {
            level++;
            expForNextLvl *= 2;
        } 
    }
    private void UpdateDebuggingText()
    {
        Vector2Int movementInt = Vector2Int.RoundToInt(transform.position);
        playerPosText.text = $" PLAYER POSITION: Movement - ({movement.x}, {movement.y}) Transform - ({movementInt.x}, {movementInt.y})";
        mousePosText.text = $" MOUSE POSITION: ({mousePos.x}, {mousePos.y})";
        levelText.text = $" LEVEL: {level}, EXP: {exp}, EXP FOR NEXT LEVEL: {expForNextLvl - exp}";
        GameObject tileH = terrainGenerator.GetTile(mousePos.x, mousePos.y);
        if (tileH != null)
            hoveringTile.text = $" TILE: {tileH.name}\n WORLD TILE INDEX: {terrainGenerator.worldTileObjects.IndexOf(tileH)}";
        else
            hoveringTile.text = $" TILE: NULL\n WORLD TILE INDEX: NULL";
    }
    private void HandleLeftClickActions(Item item)
    {
        DestroyItem();

        item = inventoryManager.GetSelectedItem(false);
        if (item.type != ItemType.BuildingBlock || item.type != ItemType.Consumable)
        {
            switch (item.type)
            {
                case ItemType.Tool:
                
                    if (IsInRange(mousePos.x, mousePos.y, item))
                        terrainGenerator.BreakTile(mousePos.x, mousePos.y);

                    break;

                case ItemType.Melee:
                    break;
            }
        } 
    }
    private void HandleRightClickActions(Item item)
    {
        item = inventoryManager.GetSelectedItem(false);
        switch (item.type)
        {
            case ItemType.BuildingBlock:
                if (IsInRange(mousePos.x, mousePos.y, item))
                {
                    bool canPlace = terrainGenerator.PlayerPlaceTile(item, item.itemObject.name, item.itemObject.GetComponent<SpriteRenderer>().sprite, mousePos.x, mousePos.y);
                    if (canPlace)
                        item = inventoryManager.GetSelectedItem(true);
                }
                break;

            case ItemType.Consumable:
                break;
        }
    }
    public bool IsInRange(float mouseX, float mouseY, Item item)
    {
        float xDist = Mathf.Abs(mouseX - transform.position.x);
        float yDist = Mathf.Abs(mouseY - transform.position.y);

        return (xDist <= item.range.x && yDist <= item.range.y);
    }
    private void DrawItem(Item item)
    {
        switch (item.type)
        {
            case ItemType.Melee:
                break;
            case ItemType.Ranged:
                break;
            case ItemType.Magic:
                break;
            case ItemType.Tool:
                break;
                
        }
        itemOnScreen = GameObject.Instantiate(item.itemObject);
        itemOnScreen.transform.parent = this.transform;
        itemOnScreen.transform.position = this.transform.position;
        Invoke("DestroyItem", item.useTime);
    }
    private void DestroyItem()
    {
        if (itemOnScreen != null)
            GameObject.Destroy(itemOnScreen);
    }
    private void HandlePlayerMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            animator.SetFloat("lastMoveHorizontal", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("lastMoveVertical", Input.GetAxisRaw("Vertical"));

            lastDirFaced = movement;
        }

        if (Input.GetKey(KeyCode.RightShift))
        {
            if (speed < sprintSpeed)
                speed += 2f * Time.deltaTime;

            animator.SetBool("isSprinting", true);
        }

        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            speed = defaultSpeed;
            animator.SetBool("isSprinting", false);
        }
    }
    private void GetMousePosition()
    {
        float distance = cam.nearClipPlane;
        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
        mousePos.x = Mathf.RoundToInt(worldPoint.x - 0.5f);
        mousePos.y = Mathf.RoundToInt(worldPoint.y - 0.5f);
    }
    
}