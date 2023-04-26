using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ExtensionMethods.ExtensionMethods;

public class Beetle : MonoBehaviour, IFriendlyMob
{
    public PlayerMovement player;
    private float moveSpeed = 1;
    private int expReward = 2;
    private Rigidbody2D rb;
    private const float maxHealth = 10f;
    private float health = maxHealth;
    private bool isWalking;
    private float walkTime;
    private float walkCounter;
    private float waitTime; 
    private float waitCounter;
    private int WalkDirection;
    private Vector2 mousePos;
    private void Start()
    {
        this.walkTime = Random.Range(3, 6);
        this.waitTime = Random.Range(1, 4);

        this.player = FindObjectOfType<PlayerMovement>();
        this.rb = this.gameObject.GetComponent<Rigidbody2D>();

        this.waitCounter = this.waitTime;
        this.walkCounter = this.walkTime;
        
    }
    private void Update()
    {   
        GetMousePosition();
        HandleDamage();
        RunAwayFromPlayer();
    }
    
    private void RunAwayFromPlayer()
    {
        float distance = Vector2.Distance(this.transform.position, this.player.transform.position);

        if (distance <= 9.5f)
        {
            float step = moveSpeed * Time.deltaTime;
            this.transform.position = Vector2.MoveTowards(this.transform.position, this.player.transform.position, -step);
            Print(distance);
        }
    }
    private void ChooseDirection()
    {
        this.WalkDirection = Random.Range(0, 4);
        this.isWalking = true;
        this.walkCounter = this.walkTime;
    }
    public void GetMousePosition()
    {
        float distance = Camera.main.nearClipPlane;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
        mousePos.x = Mathf.RoundToInt(worldPoint.x - 0.5f);
        mousePos.y = Mathf.RoundToInt(worldPoint.y - 0.5f);
    }
    public void HandleDamage()
    {
        if (health <= 0)
        {
            this.player.exp += this.expReward;
            this.player.CheckLevelUp();
            this.player.mobsKilled++;
            Destroy(this.gameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            InventoryManager inventory = this.player.inventoryManager;
            Item item = inventory.GetSelectedItem(false);

            if (hit2D.collider != null && hit2D.collider.name == this.gameObject.name && player.IsInRange(mousePos.x, mousePos.y, item))
            {
                foreach (ActionType action in item.actionType)
                {
                    if (action == ActionType.Attack)
                    {
                        Print("Hit with " + item.name);
                        this.health -= item.damage;
                        moveSpeed = 2;
                        Print(health);
                        StartCoroutine(ResetSpeedAfterHit(1f));
                    }
                }
            }
        }
    }
    IEnumerator ResetSpeedAfterHit(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = 1;
    }
    public void HandleMovement()
    {
        if (this.isWalking)
        {
            this.walkCounter -= Time.deltaTime;

            switch (this.WalkDirection)
            {
                case 0:
                    this.rb.velocity = new Vector2(0, moveSpeed);
                    break;
                case 1:
                    this.rb.velocity = new Vector2(moveSpeed, 0);
                    break;
                case 2:
                    this.rb.velocity = new Vector2(0, -moveSpeed);
                    break;
                case 3:
                    this.rb.velocity = new Vector2(-moveSpeed, 0);
                    break;
            }

            if (this.walkCounter < 0)
            {
                this.isWalking = false;
                this.waitCounter = waitTime;
            }
        }
        else
        {
            this.waitCounter -= Time.deltaTime;

            this.rb.velocity = Vector2.zero;

            if (this.waitCounter < 0)
            {
                ChooseDirection();
            }
        }
    }
}