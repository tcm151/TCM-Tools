using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Consumable
{
    public float healthModifer, thirstModifier, sleepModifier, mentalModifier, hungerModifier;
    public Consumable(float hhm = 0, float tm = 0, float sm = 0, float mm = 0, float hgm = 0)
    {
        healthModifer  = hhm;
        thirstModifier = tm;
        sleepModifier  = sm;
        mentalModifier = mm;
        hungerModifier = hgm;
    }
}
