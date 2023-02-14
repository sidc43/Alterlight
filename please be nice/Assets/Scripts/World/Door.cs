using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite closed;
    public Sprite open;
    public Sprite sidewaysClosed;
    public Sprite sidewaysOpen;
    public bool facingSide;
    public bool isOpen = false;
    private readonly Vector3 sidewaysScale = new Vector3(1.375275f, 1.402689f, 1);
    private readonly Vector3 normalScale = new Vector3(1.36f, 0.8558f, 1);
    public BoxCollider2D col;
    private void Start() 
    {
        col = GetComponent<BoxCollider2D>();
        if (facingSide)
        {
            transform.localScale = sidewaysScale;
            col.offset = new Vector2(-0.2043271f, -0.01355509f);
            col.size = new Vector2(0.1153069f, 0.7379304f);
        }
        else
            transform.localScale = normalScale;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit2D.collider != null && hit2D.collider.name == this.gameObject.name)
            {
                if (isOpen)
                {
                    if (facingSide)
                    {
                        GetComponent<SpriteRenderer>().sprite = sidewaysClosed;
                        isOpen = false;
                        transform.localScale = sidewaysScale;
                        transform.position = new Vector2(transform.position.x, transform.position.y + 0.75f);
                        col.offset = new Vector2(-0.2043271f, -0.01355509f);
                        col.size = new Vector2(0.1153069f, 0.7379304f);
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().sprite = closed;
                        isOpen = false;
                        col.offset = new Vector2(0f, 0f);
                        col.size = new Vector2(0.72f, 1.25f);
                    }
                }
                else    
                {
                    if (facingSide)
                    {
                        GetComponent<SpriteRenderer>().sprite = sidewaysOpen;
                        isOpen = true;
                        transform.localScale = sidewaysScale;
                        transform.position = new Vector2(transform.position.x, transform.position.y - 0.75f);
                        col.offset = new Vector2(-0.01908609f, 0.193688f);
                        col.size = new Vector2(0.7138367f, 0.1054387f);
                    }
                    else
                    {
                        GetComponent<SpriteRenderer>().sprite = open;
                        isOpen = true;
                        col.offset = new Vector2(-0.24f, 0f);
                        col.size = new Vector2(0.16f, 1.28f);
                    }
                }
            }
        }
    }
}