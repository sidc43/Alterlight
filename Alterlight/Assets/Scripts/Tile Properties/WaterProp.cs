using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterProp : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        this.gameObject.AddComponent<BoxCollider2D>();
        this.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "player")
        {
            player = other.gameObject;
            player.GetComponent<PlayerMovement>().speed = 2f;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.name == "player")
        {
            player = other.gameObject;
            StartCoroutine(ResetPlayerSpeed(player.GetComponent<PlayerMovement>(), 1));
        }
    }

    IEnumerator ResetPlayerSpeed(PlayerMovement player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.speed = 4f;
    }
}
