using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private static int currentDate;
    // private static int updateInterval = 50;
    private const int Seconds = 5, Minutes = 60, Hours = 720, Days = 8640, Months = 103680, Years = 1244160;

    private void Start() => currentDate = 0;

    private void FixedUpdate() => currentDate++;

    private int GetCurrentDate() => currentDate;
    private int GetSeconds() => currentDate / Seconds;
    private int GetMinutes() => currentDate / Minutes;
    private int GetHours() => currentDate / Hours;
    private int GetDays() => currentDate / Days;
    private int GetMonths() => currentDate / Months;
    private int GetYears() => currentDate / Years;
}
