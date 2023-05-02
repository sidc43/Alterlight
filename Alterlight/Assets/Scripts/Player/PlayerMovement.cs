using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static ExtensionMethods.ExtensionMethods;

public class PlayerMovement : MonoBehaviour
{
    #region Fields
    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI playerPosText;
    [SerializeField] private TextMeshProUGUI mousePosText;
    [SerializeField] private TextMeshProUGUI hoveringTile;
    [SerializeField] private TextMeshProUGUI levelText;
    private const string apikey = "$2b$10$zbBt5sJdmMGEdmzye1WIh.46GI1n.79/QcRlrrazVzFFiWh8ZJsr2";

    [Header("Attributes")]
    public float speed = defaultSpeed;
    public float sprintSpeed = 5.5f;
    public float health = maxHealth;
    public int defense = 5;
    public int exp = 0;
    public int level = 0;
    public int expForNextLvl;
    public int mobsKilled;
    public Item[] starting;
    public const float defaultSpeed = 4f;
    public const float maxHealth = 100;

    [Header("Inventory")]
    public InventoryManager inventoryManager;

    [Header("Functionality")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 movement;
    [SerializeField] private GameObject itemOnScreen = null;
    [SerializeField] private Vector2Int mousePos;
    [SerializeField] private TerrainGeneration terrainGenerator;
    [SerializeField] private Camera cam;
    public Vector2 lastDirFaced;
    #endregion

    private void Start()
    {
        foreach (Item i in starting)
            inventoryManager.AddItem(i);

        healthBar.SetMaxHealth(health);

        GetSavedData();
    }
    private void Update()
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
    private void FixedUpdate() 
    {
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
    private async void OnApplicationQuit() 
    {
        string path = @"C:\Users\sidc2\Documents\GitHub\alterlight\Alterlight\Assets\UserData\user_data.json";
        string jsonData = File.ReadAllText(path);
        PlayerSaveData d = JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);
        PlayerSaveData updatedData = new PlayerSaveData{ username = d.username, Level = this.level, EXP = this.exp, MobsKilled = this.mobsKilled };
        string updatedJson = JsonConvert.SerializeObject(updatedData, Formatting.Indented);
        File.WriteAllText(path, updatedJson);

        dynamic test = await MakeReq("https://api.jsonbin.io/v3/b/62bfeeed449a1f382128468b/latest");
        bool foundUser = false;
        string binid = "";

        foreach (var prop in test.record)
        {
            if (d.username == prop.Name)
            {
                Print("Found User");
                foundUser = true;
                binid = test.record[d.username];
                break;
            }
        }

        if (!foundUser)
            Print("No User Found");
        else
        {
            string url = $"https://api.jsonbin.io/v3/b/{binid}";

            var res2 = await PutJsonData(url, updatedData);

            Print($"Status code: {res2.StatusCode}");

            dynamic res3 = await MakeReq(url);
            Print(res3);
        }
    }
    public static async Task<HttpResponseMessage> PutJsonData(string url, PlayerSaveData player)
    {
        HttpClient client = new HttpClient();
        string jsonData = JsonConvert.SerializeObject(player);
        StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Add("X-Master-Key", apikey);

        return await client.PutAsync(url, content);
    }
    public void GetSavedData()
    {
        string path = @"C:\Users\sidc2\Documents\GitHub\alterlight\Alterlight\Assets\UserData\user_data.json";
        string jsonData = File.ReadAllText(path);

        PlayerSaveData lastSave = JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);

        this.level = lastSave.Level;
        this.exp = lastSave.EXP;
        this.mobsKilled = lastSave.MobsKilled;
        CheckLevelUp();
    }
    public static async Task<dynamic> MakeReq(string path)
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-Master-Key", apikey);
        string res = await client.GetStringAsync(path);
        dynamic json = JsonConvert.DeserializeObject(res);

        return json!;
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
        levelText.text = $" LEVEL: {level}, EXP: {exp}, EXP FOR NEXT LEVEL: {expForNextLvl - exp}\nMOBS KILLED: {mobsKilled}";
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
        if (item.mainType != MainType.Tile || item.mainType != MainType.Consumable)
        {
            switch (item.type)
            {
                case ItemType.Axe:
                case ItemType.Pickaxe:
                    if (IsInRange(mousePos.x, mousePos.y, item))
                        terrainGenerator.BreakTile(mousePos.x, mousePos.y, item);
                    break;

                case ItemType.Melee:
                    break;
            }
        } 
    }
    private void HandleRightClickActions(Item item)
    {
        item = inventoryManager.GetSelectedItem(false);
        switch (item.mainType)
        {
            case MainType.Tile:
                if (IsInRange(mousePos.x, mousePos.y, item))
                {
                    bool canPlace = terrainGenerator.PlayerPlaceTile(item, item.itemObject.name, item.itemObject.GetComponent<SpriteRenderer>().sprite, mousePos.x, mousePos.y);
                    if (canPlace)
                        item = inventoryManager.GetSelectedItem(true);
                }
                break;

            case MainType.Consumable:
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