using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace TCM.Planets
{
    public class WirePlanet : MonoBehaviour
    {
        
        [System.Serializable] public class Data
        {
            public int radius = 500;
            public float chunkHeight = 100f;
            
            [Range(1, 32)] public int chunkResolution = 4;
        }

        public Data planetData;
        
        [SerializeReference, HideInInspector] private List<WireChunk> chunks;
        [SerializeReference, HideInInspector] private List<GameObject> faces;

        private const int SIX_FACES = 6;
        
        //> INITIALIZE THE MESH
        public void Initialize()
        {
            // cleanup old game objects
            foreach (var chunk in chunks.Where(chunk => chunk)) DestroyImmediate(chunk.gameObject);
            foreach (var face in faces.Where(face => face)) DestroyImmediate(face.gameObject);
            chunks.Clear();
            faces.Clear();

            chunks = new List<WireChunk>(new WireChunk[planetData.chunkResolution * planetData.chunkResolution * SIX_FACES]);

            // loop over SIX faces
            for (int i = 0, face = 0; face < SIX_FACES; face++)
            {
                // create container for each face
                var faceObject = new GameObject($"Face {face}");
                faceObject.transform.SetParent(this.transform);
                faces.Add(faceObject);
                
                // create every chunk and initialize
                for (int y = 0; y < planetData.chunkResolution; y++) {
                    for (int x = 0; x < planetData.chunkResolution; x++, i++)
                    {
                        if (chunks[i] is null)
                        {
                            chunks[i] = new GameObject($"Chunk {face}-({x},{y})").AddComponent<WireChunk>();
                            chunks[i].transform.position = transform.position;
                            chunks[i].transform.SetParent(faceObject.transform);
                        }

                        chunks[i].Initialize(x, y, Directions[face], ref planetData);
                    }
                }
            }
        }

        //> PLANET RELATIVE DIRECTIONS
        private static readonly Vector3[] Directions =
        {
            Vector3.up,    Vector3.forward,
            Vector3.right, Vector3.back,
            Vector3.left,  Vector3.down,
        };
    }
}