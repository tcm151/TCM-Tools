using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need
{
    private float value;
    private readonly float max;
    // private float min = 0.0f;
    private bool running;

    public Need(float max)
    {
        this.max = max;
        value = max;
    }

    public bool Start()
    {
        if (running) return false;
        
        running = true;
        return true;
    }
    public bool Stop()
    {
        if (!running) return false;
        
        running = false;
        return true;
    }

    public void Update(float offset = 1f) => value -= offset;

    public void Add(float v)
    {
        value += v;
        Mathf.Clamp(value, 0f, max);
    }

    public bool IsRunning() => running;
    public float Get() => value / max * 10;
    public void Set(float v) => value = v;
}