using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TCM.NoiseGeneration;


namespace TCM.Planets
{
    public class ChunkedSpherePlanet : MonoBehaviour
    {
        //> CHUNK DATA
        [System.Serializable] public class Data
        {
            [Header("Graphics")]
            public Material material;
            
            [Header("Mesh Generation")]
            [Range(500, 2500)] public int radius = 1000;
            [Range(002, 0064)] public int meshResolution = 16;
            [Range(001, 0008)] public int chunkResolution = 2;
            [Range(100, 1000)] public float chunkHeight;

            [Header("Noise")]
            [Range(0, 16)] public float localZero = 0.5f;
            public List<Noise.Layer> noiseLayers;
        }

        public Data data;
        
        private const int SIX_FACES = 6;
        
        [SerializeReference, HideInInspector] private List<SphereChunk> chunks;
        [SerializeReference, HideInInspector] private List<GameObject> faces;

        //> ON PLAY MODE
        private void Awake() => Generate(true);
        
        public void Generate(bool printSummary)
        {
            Initialize();
            PolygonalizeChunks();

            if (printSummary)
            {
                // UpdateElevationRange();
                // Debug.Log($"Min Elevation: {elevationRange.min}, Max Elevation: {elevationRange.max}");
            }
        }

        //> INITIALIZE THE MESH
        public void Initialize()
        {
            // cleanup old game objects
            foreach (var chunk in chunks.Where(chunk => chunk)) DestroyImmediate(chunk.gameObject);
            foreach (var face in faces.Where(face => face)) DestroyImmediate(face.gameObject);
            chunks.Clear();
            faces.Clear();

            chunks = new List<SphereChunk>(new SphereChunk[data.chunkResolution * data.chunkResolution * SIX_FACES]);

            // loop over SIX faces
            for (int i = 0, face = 0; face < SIX_FACES; face++)
            {
                // create container for each face
                var faceObject = new GameObject($"Face {face}");
                faceObject.transform.SetParent(this.transform);
                faces.Add(faceObject);
                
                // create every chunk and initialize
                for (int y = 0; y < data.chunkResolution; y++) {
                    for (int x = 0; x < data.chunkResolution; x++, i++)
                    {
                        if (chunks[i] is null)
                        {
                            chunks[i] = new GameObject($"Chunk {face}-({x},{y})").AddComponent<SphereChunk>();
                            chunks[i].transform.position = transform.position;
                            chunks[i].transform.SetParent(faceObject.transform);
                        }

                        chunks[i].Initialize(new Vector2(x, y), Directions[face], ref data);
                    }
                }
            }
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

        //> PLANET RELATIVE DIRECTIONS
        private static readonly Vector3[] Directions =
        {
            Vector3.up,    Vector3.forward,
            Vector3.right, Vector3.back,
            Vector3.left,  Vector3.down,
        };
    }
}