using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need
{
    public float value;
    float MAX, MIN = 0.0f;
    bool running = false;

    public Need(float max)
    {
        MAX = max;
        value = max;
    }

    public bool start()
    {
        if (!running)
        {
            running = true;
            return true;
        }
        return false;
    }
    public bool stop()
    {
        if (running)
        {
            running = false;
            return true;
        }
        return false;
    }

    public void update(float offset = 1f)
    {
        value -= offset;
    }

    public void add(float v)
    {
        value += v;
        Mathf.Clamp(value, 0f, MAX);
    }

    public bool isRunning() => running;
    public float get() => value / MAX * 10;
    public void set(float v) => value = v;
}