using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class NPC_OLD_MAN : MonoBehaviour
{
    [Header("Attributes")]
    public float moveSpeed = defaultSpeed;
    public string npcName = "OLD MAN";
    public const float defaultSpeed = 1f;

    [Header("Movement")]
    public Rigidbody2D rb;
    private Vector2 spawnPoint;
    public Animator animator;
    private bool inRange;
    private float initx;
    private float inity;
    public bool isWalking;
    public float walkTime;
    private float walkCounter;
    public float waitTime;
    private float waitCounter;
    private int WalkDirection;
    private bool dialogueOpen;

    [Header("Interaction")]
    [SerializeField]
    private GameObject prompt;
    [SerializeField]
    public Sprite rmb;
    public GameObject dialogueBox;
    public TextMeshProUGUI nameField;
    public Item sword;
    private Button claim;

    [Header("Other")]
    [SerializeField]
    private InventoryManager invManager;
    public Light2D light2D;
    public DayCycle dayCycle;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        waitCounter = waitTime;
        walkCounter = walkTime;

        spawnPoint = this.transform.position;

        claim = dialogueBox.GetComponent<DialogueManager>().button;
    }
    public Vector2 GetSpawnPoint()
    {
        return this.spawnPoint;
    }
    void Update()
    {
        HandleMovement();
        Dialogue();
        ToggleSpotLight();
    }
    private void ToggleSpotLight()
    {
        if (dayCycle.isDay)
            light2D.enabled = false;
        else    
            light2D.enabled = true;
    }
    private void Dialogue()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

        if (inRange && hit2D.collider != null && hit2D.collider.name == this.gameObject.name && Input.GetMouseButtonDown(1))
        {
            dialogueBox.SetActive(true);
            nameField.text = npcName;
        }
    }
    private void HandleMovement()
    {
        animator.SetFloat("Speed", rb.velocity.sqrMagnitude);
        if (isWalking)
        {
            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0:
                    rb.velocity = new Vector2(0, moveSpeed);
                    break;
                case 1:
                    rb.velocity = new Vector2(moveSpeed, 0);
                    break;
                case 2:
                    rb.velocity = new Vector2(0, -moveSpeed);
                    break;
                case 3:
                    rb.velocity = new Vector2(-moveSpeed, 0);
                    break;
            }

            animator.SetFloat("Horizontal", rb.velocity.x);
            animator.SetFloat("Vertical", rb.velocity.y);

            if (walkCounter < 0)
            {
                isWalking = false;
                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;

            rb.velocity = Vector2.zero;

            if (waitCounter < 0)
            {
                ChooseDirection();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "player")
        {
            prompt.GetComponent<SpriteRenderer>().sprite = rmb;
            prompt.GetComponent<SpriteRenderer>().sortingOrder = 3;
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            prompt.transform.position = new Vector2(x, y + 1);
            inRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "player")
        {
            prompt.GetComponent<SpriteRenderer>().sprite = null;
            inRange = false;
        }
    }
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }
}
