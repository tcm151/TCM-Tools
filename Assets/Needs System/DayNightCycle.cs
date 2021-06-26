using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private static int CURRENT_DATE;
    private static int UPDATE_INTERVAL = 50;
    private static int SECONDS = 5, MINUTES = 60, HOURS = 720, DAYS = 8640, MONTHS = 103680, YEARS = 1244160;

        
    void Start() => CURRENT_DATE = 0;

    void FixedUpdate() => CURRENT_DATE++;

    int getCurrentDate() => CURRENT_DATE;

    int getSeconds() => CURRENT_DATE / SECONDS;

    int getMinutes() => CURRENT_DATE / MINUTES;

    int getHours() => CURRENT_DATE / HOURS;

    int getDays() => CURRENT_DATE / DAYS;

    int getMonths() => CURRENT_DATE / MONTHS;

    int getYears() => CURRENT_DATE / YEARS;

}
