using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePrompt : MonoBehaviour
{
    public GameObject prompt;
    public Sprite sprite;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "player")
        {
            prompt.GetComponent<SpriteRenderer>().sprite = sprite;
            prompt.GetComponent<SpriteRenderer>().sortingOrder = 3;
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            prompt.transform.position = new Vector2(x, y + 1);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "player")
        {
            prompt.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
