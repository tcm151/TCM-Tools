using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TCM.NoiseGeneration;
using TCM.Tools;


namespace MarchedRendering
{
    public class ChunkGrid : MonoBehaviour
    {
        //> GLOBAL CHUNK VOLUME DATA STRUCT
        [Serializable] public class Data
        {
            [Header("Graphics")]
            public Material material;
            
            [Header("Mesh Generation")]
            [Range(500, 2500)] public int radius = 1000;
            [Range(002, 0064)] public int meshResolution = 16;
            [Range(001, 0008)] public int chunkResolution = 2;

            [Header("Noise")]
            [Range(0, 16)] public float localZero = 0.5f;
            public List<Noise.Layer> noiseLayers;
        }
        
        public Data data;
        [SerializeReference] public List<Chunk> chunks;

        private Range elevationRange;

        //> ON PLAY MODE
        private void Awake() => Generate(true);

        public void Generate(bool printSummary)
        {
            Initialize();
            PolygonalizeChunks();

            if (printSummary)
            {
                UpdateElevationRange();
                Debug.Log($"Min Elevation: {elevationRange.min}, Max Elevation: {elevationRange.max}");
            }
        }

        //> INITIALIZE THE MESH
        private void Initialize()
        {
            // cleanup old game objects
            foreach (var chunk in chunks.Where(chunk => chunk)) DestroyImmediate(chunk.gameObject);
            chunks.ForEach(DestroyImmediate);
            chunks.Clear();

            chunks = new List<Chunk>(new Chunk[data.chunkResolution * data.chunkResolution * data.chunkResolution]);

            for (int i = 0, y = 0; y < data.chunkResolution; y++) {
                for (int z = 0; z < data.chunkResolution; z++) {
                    for (int x = 0; x < data.chunkResolution; i++, x++)
                    {
                        if (chunks[i] is null)
                        {
                            chunks[i] = new GameObject($"Chunk ({x},{y},{z})").AddComponent<Chunk>();
                            chunks[i].transform.position = transform.position;
                            chunks[i].transform.SetParent(transform);
                        }

                        chunks[i].Initialize(new Vector3(x, y, z), data);
                    }
                }
            }
            
            
        }

        private void UpdateElevationRange()
        {
            elevationRange = new Range();
            foreach (var c in chunks) elevationRange += c.noiseRange;
        }

        public void PolygonalizeChunks()
        {
            Task[] polygonalizations = new Task[chunks.Count];
            for (int i = 0; i < chunks.Count; i++)
            {
                polygonalizations[i] = Task.Factory.StartNew(chunks[i].Polygonalize);
            }
            Task.WaitAll(polygonalizations, 5000);

            // apply triangulations on main thread
            foreach (var chunk in chunks) chunk.Apply();
        }
    }
}