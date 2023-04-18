using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveEntryProp : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit2D.collider != null && hit2D.collider.name == this.gameObject.name)
            {
                Debug.Log("clicked " + this.gameObject.name);
                LoadingManager.Instance.LoadScene("Cave");
            }
        }
    }
}
