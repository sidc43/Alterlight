using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintProp : MonoBehaviour
{
    BoxCollider2D col;
    CircleCollider2D trigger;
    void Start()
    {
        this.gameObject.AddComponent<BoxCollider2D>();
        this.gameObject.AddComponent<CircleCollider2D>();

        col = this.GetComponent<BoxCollider2D>();
        trigger = this.GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;

        trigger.radius = 0.7f;
    }


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "player")
        {
            col.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.name == "player")
        {
            col.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
