using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    Vector2 mousePos;
    Vector3 worldPoint;

    float distance;
    Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        distance = mainCam.nearClipPlane;
    }

    private void FixedUpdate()
    {
        worldPoint = mainCam.ScreenToWorldPoint(
            new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y));

        mousePos.x = worldPoint.x;
        mousePos.y = worldPoint.y;
        transform.position = mousePos;
    }
}