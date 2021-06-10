using System;
using UnityEngine;


namespace TCM
{
    public class BoidSpawner : MonoBehaviour
    {
        public Boid boidPrefab;

        public int amount = 25;

        private Boid[] boids;

        private void Awake()
        {
            boids = new Boid[amount];

            for (int i = 0; i < amount; i++)
            {
                boids[i] = Instantiate(boidPrefab);
                boids[i].transform.SetParent(transform);
                boids[i].Initialize();
            }
        }
    }
}