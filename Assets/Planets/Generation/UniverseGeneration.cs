// using System.Collections;
// using System.Collections.Generic;
// using TCM.Planets;
// using UnityEngine;
//
// namespace TCM.Planets.NoiseGeneration
// {
//     public class UniverseGeneration : MonoBehaviour
//     {
//         [SerializeField] private Material planetMaterial;
//         [SerializeField] private Gradient planetGradient;
//         
//         private Planet homePlanet;
//
//         private void Start()
//         {
//             //+ PLANET CREATION
//             var newPlanetData = new Planet.Data
//             {
//                 material = planetMaterial,
//                 terrainGradient = planetGradient,
//                 textureResolution = 64,
//                 
//                 radius = 5000,
//                 noiseLayers = new List<Noise.Layer>(),
//                 
//                 chunkResolution = 16,
//                 meshResolution = 16,
//             };
//             
//             homePlanet = new GameObject("HomePlanet").AddComponent<Planet>();
//             homePlanet.planetData = newPlanetData;
//
//
//             homePlanet.GeneratePlanet(true);
//
//         }
//     }
// }
