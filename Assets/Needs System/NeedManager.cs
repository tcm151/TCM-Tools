using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedManager : MonoBehaviour
{
    private const float FIXED_TIMESTEP = 0.02f;
    [SerializeField] private int UPDATE_INTERVAL = 250;
    private Need health, thirst, sleep, mental, hunger;
    private float HH_MAX = 10.0f, T_MAX = 6912, S_MAX = 10368, HG_MAX = 20736;
    private int i = 0;
    private bool dead = false;

    void youDied()
    {
        dead = true;
        Debug.Log("You Died.");
    }

    public void consume(Consumable item)
    {
        health.Update(item.healthModifer);
        thirst.Update(item.thirstModifier);
         sleep.Update(item.sleepModifier);
        mental.Update(item.mentalModifier);
        hunger.Update(item.hungerModifier);
    }

    float currentHealth() => (hunger.Get() + sleep.Get() + thirst.Get()) / 3;



    void print()
    {
        string s = "";
        s += "| HEALTH: " + health.Get().ToString("F1") + " ";
        s += "| THIRST: " + thirst.Get().ToString("F1") + " ";
        s += "| SLEEP: "  +  sleep.Get().ToString("F1") + " ";
        s += "| HUNGER: " + hunger.Get().ToString("F1") + " |";
        Debug.Log(s);
    }

    void Start()
    {
        health = new Need(HH_MAX);
        thirst = new Need(T_MAX);
        sleep  = new Need(S_MAX);
        mental = new Need(10.0f);
        hunger = new Need(HG_MAX);
    }

    void FixedUpdate()
    {
        if (++i >= UPDATE_INTERVAL && !dead)
        {
            hunger.Update();
             sleep.Update();
            thirst.Update();
            health.Set(currentHealth());
            i = 0;

            print();

            if (health.Get() < 0 || thirst.Get() < 0 || sleep.Get() < 0 || hunger.Get() < 0)
                youDied();
        }
    }
}
