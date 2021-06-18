using System.Collections.Generic;
using UnityEngine;


namespace TCM.Planets
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Mesh mesh;

        private Vector3 Up, Forward, Right;
        private Vector2 chunkOffset;
        private float chunkPercent;


        private List<Vector3> vertices;
        private List<Vector3> normals;
        private List<int> triangles;

        private Planet.Data planetData;

        public Range elevationRange;
        public Vector3 averageUp;

        //> INITIALIZATION
        public void Initialize(int x, int y, Vector3 up, ref Planet.Data planetData)
        {
            this.elevationRange = new Range();
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshCollider = GetComponent<MeshCollider>();

            this.Up = up;
            this.planetData = planetData;
            this.chunkPercent = (1f / planetData.chunkResolution);
            this.chunkOffset = (new Vector2(x, y) / planetData.chunkResolution);
            
            this.meshRenderer.sharedMaterial = planetData.material;
            this.mesh = meshFilter.sharedMesh = meshCollider.sharedMesh = new Mesh {name = $"Mesh ({x},{y})"};

            this.Right = new Vector3(Up.y, Up.z, Up.x);
            this.Forward = Vector3.Cross(Up, Right);
            
            normals = new List<Vector3>(new Vector3[planetData.meshResolution * planetData.meshResolution]);
            vertices = new List<Vector3>(new Vector3[planetData.meshResolution * planetData.meshResolution]);
            triangles = new List<int>(new int[(planetData.meshResolution - 1) * (planetData.meshResolution - 1) * 6]);

            CalculateAverageUp();
        }

        //> CALCULATE THE AVERAGE DIRECTION OF THE CHUNK
        public void CalculateAverageUp(int resolution = 3)
        {
            //- Loop over every vertex
            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++)
                {
                    Vector2 percent = (new Vector2(x, y) / (resolution - 1) * chunkPercent) + chunkOffset;
                    Vector3 position = Up + ((percent.x - 0.5f) * 2 * Right) + ((percent.y - 0.5f) * 2 * Forward);
                    
                    //- Better method for converting unit cube to unit sphere
                    float xSquared = position.x * position.x;
                    float ySquared = position.y * position.y;
                    float zSquared = position.z * position.z;
                    Vector3 positionNormalized = new Vector3
                    {
                        x = position.x * Mathf.Sqrt(1f - (ySquared / 2f) - (zSquared / 2f) + (ySquared * zSquared / 3f)),
                        y = position.y * Mathf.Sqrt(1f - (xSquared / 2f) - (zSquared / 2f) + (xSquared * zSquared / 3f)),
                        z = position.z * Mathf.Sqrt(1f - (xSquared / 2f) - (ySquared / 2f) + (xSquared * ySquared / 3f)),
                    };
                    
                    averageUp += positionNormalized;
                }
            }
            averageUp.Normalize();
        }

        //> TRIANGULATE THIS CHUNK
        public void Triangulate(int resolution = -1)
        {
            if (resolution == -1) resolution = planetData.meshResolution;
            else
            {
                normals = new List<Vector3>(new Vector3[resolution * resolution]);
                vertices = new List<Vector3>(new Vector3[resolution * resolution]);
                triangles = new List<int>(new int [(resolution - 1) * (resolution - 1) * 6]);
            }

            //- Loop over every vertex
            for (int i = 0, t = 0, y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; i++, x++)
                {
                    Vector2 percent = (new Vector2(x, y) / (resolution - 1) * chunkPercent) + chunkOffset;
                    Vector3 position = Up + ((percent.x - 0.5f) * 2 * Right) + ((percent.y - 0.5f) * 2 * Forward);

                    //- Better method for converting unit cube to unit sphere
                    float xSquared = position.x * position.x;
                    float ySquared = position.y * position.y;
                    float zSquared = position.z * position.z;
                    Vector3 positionNormalized = new Vector3
                    {
                        x = position.x * Mathf.Sqrt(1f - (ySquared / 2f) - (zSquared / 2f) + (ySquared * zSquared / 3f)),
                        y = position.y * Mathf.Sqrt(1f - (xSquared / 2f) - (zSquared / 2f) + (xSquared * zSquared / 3f)),
                        z = position.z * Mathf.Sqrt(1f - (xSquared / 2f) - (ySquared / 2f) + (xSquared * ySquared / 3f)),
                    };

                    normals[i] = positionNormalized;
                    vertices[i] = GetElevation(positionNormalized);

                    //~ skip if on edge of mesh
                    if ((x == resolution - 1) || (y == resolution - 1)) continue;

                    // add the two respective triangles
                    triangles[t + 0] = i;
                    triangles[t + 1] = i + resolution + 1;
                    triangles[t + 2] = i + resolution;
                    triangles[t + 3] = i;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i + resolution + 1;
                    t += 6;
                }
            }
        }
        
        //> RETRIANGULATE THIS CHUNK WITH SAME RESOLUTIONS
        public void Retriangulate() => Triangulate();

        //> GET THE ELEVATION AT ANY GIVEN POINT
        public Vector3 GetElevation(Vector3 position)
        {
            float elevation = 0f;
            float firstLayerElevation = 0f;

            if (planetData.noiseLayers.Count > 0)
            {
                firstLayerElevation = Noise.GenerateValue(planetData.noiseLayers[0], position);
                if (planetData.noiseLayers[0].enabled) elevation = firstLayerElevation;
            }

            for (int i = 1; i < planetData.noiseLayers.Count; i++)
            {
                // ignore if not enabled
                if (!planetData.noiseLayers[i].enabled) continue;

                float firstLayerMask = (planetData.noiseLayers[i].useMask) ? firstLayerElevation : 1;
                elevation += Noise.GenerateValue(planetData.noiseLayers[i], position) * firstLayerMask;
            }

            elevation = (planetData.radius + (250 * elevation));
            elevationRange.Add(elevation);
            return position * elevation;
        }

        //> APPLY TRIANGULATION TO MESH
        public void Apply()
        {
            // Debug.Log($"{vertices.ToArray().Length}, {normals.ToArray().Length}, {triangles.ToArray().Length}");
            
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
            // mesh.RecalculateNormals();
        }
    }
}