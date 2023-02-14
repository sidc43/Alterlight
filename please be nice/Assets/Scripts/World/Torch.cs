using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    public DayCycle dayCycle;
    private Light2D light2D;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        light2D.enabled = false;
        light2D.pointLightOuterRadius = 2.11f;
        light2D.intensity = 0.56f;
    }
    void Update()
    {
        if (dayCycle.isDay)
            light2D.enabled = false;
        else if (!dayCycle.isDay)
            light2D.enabled = true;
    }
}
