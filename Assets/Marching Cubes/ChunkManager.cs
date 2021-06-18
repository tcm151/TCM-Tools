using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


namespace TCM.MarchingCubes
{
    public class ChunkManager : MonoBehaviour
    {
        public Material material;
        
        
        [SerializeField, Range(500, 2500)] private int size = 16;
        [SerializeField, Range(  2,   64)] private int meshResolution = 16;
        [SerializeField, Range(  1,   32)] private int chunkResolution = 4;
        [SerializeField, Range(250, 1750)] private float localZero = 8f;
        
        [SerializeReference, HideInInspector] private List<Chunk> chunks;
        [SerializeReference, HideInInspector] private List<GameObject> faces;

        //> ON PLAY MODE
        private void Awake() => Initialize();

        //> INITIALIZE THE MESH
        public void Initialize()
        {
            // cleanup old game objects
            foreach (var chunk in chunks.Where(chunk => chunk)) DestroyImmediate(chunk.gameObject);
            foreach (var face in faces.Where(face => face)) DestroyImmediate(face.gameObject);
            chunks.Clear();
            faces.Clear();

            chunks = new List<Chunk>(new Chunk[chunkResolution * chunkResolution * chunkResolution]);

            for (int i = 0, y = 0; y < chunkResolution; y++) {
                for (int z = 0; z < chunkResolution; z++) {
                    for (int x = 0; x < chunkResolution; i++, x++)
                    {
                        if (chunks[i] is null)
                        {
                            chunks[i] = new GameObject($"Chunk ({x},{y},{z})").AddComponent<Chunk>();
                            chunks[i].transform.position = transform.position;
                            chunks[i].transform.SetParent(transform);
                        }

                        chunks[i].Initialize(new Vector3(x, y, z), size, chunkResolution, meshResolution, localZero, material);
                    }
                }
            }
            
            PolygonalizeChunks();
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