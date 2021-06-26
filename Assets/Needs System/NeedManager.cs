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
        health.update(item.healthModifer);
        thirst.update(item.thirstModifier);
         sleep.update(item.sleepModifier);
        mental.update(item.mentalModifier);
        hunger.update(item.hungerModifier);
    }

    float currentHealth() => (hunger.get() + sleep.get() + thirst.get()) / 3;



    void print()
    {
        string s = "";
        s += "| HEALTH: " + health.get().ToString("F1") + " ";
        s += "| THIRST: " + thirst.get().ToString("F1") + " ";
        s += "| SLEEP: "  +  sleep.get().ToString("F1") + " ";
        s += "| HUNGER: " + hunger.get().ToString("F1") + " |";
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
            hunger.update();
             sleep.update();
            thirst.update();
            health.set(currentHealth());
            i = 0;

            print();

            if (health.get() < 0 || thirst.get() < 0 || sleep.get() < 0 || hunger.get() < 0)
                youDied();
        }
    }
}
