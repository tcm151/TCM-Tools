using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;


namespace TCM.Planets
{
    [SelectionBase]
    public class Planet : MonoBehaviour
    {
        //> PLANET DATA STRUCT
        [System.Serializable]
        public class Data
        {
            [Header("Color")]
            public Material material;
            public Gradient terrainGradient;
            public Gradient underwaterGradient;
            public int textureResolution = 64;

            [Header("Shape")] public int radius = 150;
            public List<Noise.Layer> noiseLayers;
            
            [Header("Mesh Generation")]
            [Range(1, 32)] public int chunkResolution = 4;
            [Range(16, 256)] public int meshResolution = 16;

            //> SAVE THE DATA TO DISK
            public void Serialize()
            {
                //@ DEVISE A WAY TO SAVE PLANET DATA TO FILE 
            }
        }

        public Data planetData;

        private const int SIX_FACES = 6;
        private readonly Stopwatch timer = new Stopwatch();
        [SerializeReference, HideInInspector] private List<Chunk> chunks;
        [SerializeReference, HideInInspector] private List<GameObject> faces;

        //> GENERATE PLANET ON PLAY
        private void Awake() => GeneratePlanet(true);

        //> INITIALIZE THE MESH
        private void Initialize()
        {
            // cleanup old game objects
            foreach (var chunk in chunks.Where(chunk => chunk)) DestroyImmediate(chunk.gameObject);
            foreach (var face in faces.Where(face => face)) DestroyImmediate(face.gameObject);
            chunks.Clear();
            faces.Clear();

            chunks = new List<Chunk>(new Chunk[planetData.chunkResolution * planetData.chunkResolution * SIX_FACES]);

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
                            chunks[i] = new GameObject($"Chunk {face}-({x},{y})").AddComponent<Chunk>();
                            chunks[i].transform.position = transform.position;
                            chunks[i].transform.SetParent(faceObject.transform);
                        }

                        chunks[i].Initialize(x, y, Directions[face], ref planetData);
                    }
                }
            }
        }

        //> GENERATE THE PLANET FROM SCRATCH
        public void GeneratePlanet(bool printComputeTime)
        {
            timer.Start();
            Initialize();
            float initializationTime = timer.ElapsedMilliseconds;
            timer.Restart();
            TriangulateChunks();
            float triangulationTime = timer.ElapsedMilliseconds;
            timer.Restart();
            ApplyColors();
            float colorizationTime = timer.ElapsedMilliseconds;
            timer.Reset();

            if (!printComputeTime)
            {
                Debug.Log($"Initialization in: {initializationTime} ms");
                Debug.Log($"Triangulation in: {triangulationTime} ms");
                Debug.Log($"Colorization in: {colorizationTime} ms");
            }
        }

        //> TRIANGULATE ALL CHUNKS
        public void TriangulateChunks() => TriangulateChunks(chunks);

        //> TRIANGULATE A LIST OF CHUNKS (MULTI-THREADED)
        private static void TriangulateChunks(IReadOnlyList<Chunk> chunks)
        {
            Task[] triangulations = new Task[chunks.Count];
            for (int i = 0; i < chunks.Count; i++)
            {
                int j = i;
                triangulations[j] = Task.Factory.StartNew(() => chunks[j].Triangulate());
            }
            Task.WaitAll(triangulations, 5000);

            // apply triangulations on main thread
            foreach (var chunk in chunks) chunk.Apply();
        }

        //? UNSURE IF NOT WORKING PROPERLY 
        //> GET THE RESPONSIBLE CHUNK FROM ANY POSITION
        public Chunk GenerateClosestChunks(Vector3 targetPosition, float threshold = 0.95f)
        {
            targetPosition.Normalize();
            var closestChunk = chunks[0];

            foreach (var chunk in chunks)
            {
                if (Vector3.Dot(targetPosition, chunk.averageUp) > Vector3.Dot(targetPosition, closestChunk.averageUp))
                    closestChunk = chunk;
            }
            
            TriangulateChunks(chunks.Where(chunk => Vector3.Dot(targetPosition, chunk.averageUp) > threshold).ToList());
            
            return closestChunk;
        }

        [SerializeField, HideInInspector] private Color[] terrainColors;
        [SerializeField, HideInInspector] private Color[] underwaterColors;
        [SerializeField, HideInInspector] private Texture2D terrainTexture;
        [SerializeField, HideInInspector] private Texture2D underwaterTexture;
        
        private static readonly int ElevationRangeID = Shader.PropertyToID("Elevation_Range");
        private static readonly int TerrainTextureID = Shader.PropertyToID("Terrain_Texture");
        private static readonly int UnderwaterTextureID = Shader.PropertyToID("Underwater_Texture");

        //> APPLY COLOR TO THE PLANET
        public void ApplyColors()
        {
            terrainColors = new Color[planetData.textureResolution];
            terrainTexture = new Texture2D(planetData.textureResolution, 1);
            
            underwaterColors = new Color[planetData.textureResolution];
            underwaterTexture = new Texture2D(planetData.textureResolution, 1);

            for (int i = 0; i < planetData.textureResolution; i++)
            {
                terrainColors[i] = planetData.terrainGradient.Evaluate(i / (planetData.textureResolution - 1f));
                terrainTexture.SetPixels(terrainColors);
                terrainTexture.Apply();
                // File.WriteAllBytes(Application.dataPath + "/Planets/Graphics/terrainTexture.png", terrainTexture.EncodeToPNG());
            }
            
            for (int i = 0; i < planetData.textureResolution; i++)
            {
                underwaterColors[i] = planetData.underwaterGradient.Evaluate(i / (planetData.textureResolution - 1f));
                underwaterTexture.SetPixels(underwaterColors);
                underwaterTexture.Apply();
                // File.WriteAllBytes(Application.dataPath + "/Planets/Graphics/underwaterTexture.png", underwaterTexture.EncodeToPNG());
            }

            var elevationRange = new Range();
            foreach (var chunk in chunks) elevationRange += chunk.elevationRange;

            planetData.material.SetVector(ElevationRangeID, new Vector4(elevationRange.min, elevationRange.max));
            planetData.material.SetTexture(TerrainTextureID, terrainTexture);
            planetData.material.SetTexture(UnderwaterTextureID, underwaterTexture);
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