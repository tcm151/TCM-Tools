using System.Collections;
using System.Collections.Generic;
using TCM.Planets;
using UnityEngine;

namespace TCM
{
    public class UniverseGeneration : MonoBehaviour
    {
        [SerializeField] private Material planetMaterial;
        [SerializeField] private Gradient planetGradient;
        
        private Planet homePlanet;

        private void Start()
        {
            //+ PLANET CREATION
            var newPlanetData = new Planet.Data
            {
                material = planetMaterial,
                colorGradient = planetGradient,
                textureResolution = 64,
                
                radius = 5000,
                noiseLayers = new List<NoiseFilter.Settings>(),
                
                chunkResolution = 16,
                meshResolution = 16,
            };
            
            homePlanet = new GameObject("HomePlanet").AddComponent<Planet>();
            homePlanet.planetData = newPlanetData;


            homePlanet.GeneratePlanet(true);

        }
    }
}
