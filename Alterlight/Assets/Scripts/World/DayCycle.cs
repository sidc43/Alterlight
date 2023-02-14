using System.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycle : MonoBehaviour
{
    private Stopwatch sw = new Stopwatch();

    [Header("Time for overall day/night in seconds")]
    public int fullDayTime;
    public int fullNightTime;

    [Header("Debugging")]
    public bool isDay = true;
    public Light2D light2D;
    [SerializeField] private bool dayCycleEnabled; //? false = Always Day
    [SerializeField] private bool dontCycleLight; //? true = Always night

    [Header("Lighting values (increase for more light)")]
    [SerializeField] float dayMaxOuterRadiusTarget;
    [SerializeField] float dayMaxIntensityTarget;
    [SerializeField] float nightMinOuteRadiusrTarget;
    [SerializeField] float nightMinIntensityTarget;

    private void Start()
    {
        sw.Start();
    }
    private void Update() 
    {
        ToggleDayNight();
    }
    private void FixedUpdate()
    {
        if (!dontCycleLight)
            CycleLighting();
    }
    private void ToggleDayNight()
    {
        if (dayCycleEnabled)
        {
            if (isDay && sw.ElapsedMilliseconds >= (fullDayTime * 1000))
            {
                isDay = false;
                RestartSw();
            }

            if (!isDay && sw.ElapsedMilliseconds >= (fullNightTime * 1000))
            {
                isDay = true;
                RestartSw();
            }
        }
        else
        {
            light2D.enabled = false;
        }
    }
    private void CycleLighting()
    {
        if (isDay)
        {
            if (light2D.pointLightOuterRadius < dayMaxOuterRadiusTarget)
                    light2D.pointLightOuterRadius += 4f * Time.deltaTime;

            if (light2D.intensity < dayMaxIntensityTarget)
                light2D.intensity += 2f * Time.deltaTime;
        }
        else
        {
            if (light2D.pointLightOuterRadius > nightMinOuteRadiusrTarget)
                light2D.pointLightOuterRadius -= 4f * Time.deltaTime;
            
            if (light2D.intensity > nightMinIntensityTarget)
                light2D.intensity -= 2f * Time.deltaTime;
        }
    }
    private void RestartSw()
    {
        sw.Reset();
        sw.Start();
    }
}
