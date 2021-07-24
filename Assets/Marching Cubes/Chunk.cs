using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TCM.Tools;
using TCM.NoiseGeneration;


namespace MarchedRendering
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

        private Cell outside;
        private float chunkPercent;
        private Vector3 chunkOffset;
        private ChunkGrid.Data data;

        public Range noiseRange;
        
        [SerializeField, HideInInspector] public Cell[] cells;
        [SerializeField, HideInInspector] public Vector4[] points;
        [SerializeField, HideInInspector] private List<int> triangles;
        [SerializeField, HideInInspector] private List<Vector3> vertices;

        
        //> INITIALIZATION
        public void Initialize(Vector3 index, ChunkGrid.Data data)
        {
            this.data = data;

            this.noiseRange = new Range();

            this.chunkPercent = (1f / data.chunkResolution);
            this.chunkOffset = (index / data.chunkResolution);
            
            triangles = new List<int>();
            vertices = new List<Vector3>();
            
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshCollider = GetComponent<MeshCollider>();

            this.meshRenderer.sharedMaterial = data.material;
            this.mesh = meshFilter.sharedMesh = meshCollider.sharedMesh = new Mesh
            {
                name = $"Mesh {index.ToString()}",
                indexFormat = IndexFormat.UInt32,
            };

            cells = new Cell[(data.meshResolution - 1) * (data.meshResolution - 1) * (data.meshResolution - 1)];
            points = new Vector4[data.meshResolution * data.meshResolution * data.meshResolution];

            BuildVertices();
            BuildCells();
            GetChunkCorners();
        }

        private void BuildVertices()
        {
            //- Loop over every vertex
            for (int i = 0, y = 0; y < data.meshResolution; y++) {
                for (int z = 0; z < data.meshResolution; z++) {
                    for (int x = 0; x < data.meshResolution; i++, x++)
                    {
                        Vector3 percent = (new Vector3(x, y, z) / (data.meshResolution - 1) * chunkPercent) + chunkOffset;
                        Vector3 position3D = (percent - 0.5f * Vector3.one) * data.radius;
                        Vector4 position4D = position3D;
                        position4D.w = GetElevation(position3D);
                        points[i] = position4D;
                    }
                }
            }
        }

        private float GetElevation(Vector3 position3D)
        {
            position3D /= data.radius;
            // position3D.Normalize();
            
            float value = position3D.magnitude;
            // value += Noise.GenerateValue(data.noiseLayers[0], position3D);
            value += Noise.GenerateValue(data.noiseLayers[0], position3D);
            
            noiseRange.Add(value);
            return value;
        }

        private void BuildCells()
        {
            //- Loop over every vertex - 1
            for (int i = 0, v = 0, y = 0; y < data.meshResolution - 1; y++, v += data.meshResolution) {
                for (int z = 0; z < data.meshResolution - 1; z++, v++) {
                    for (int x = 0; x < data.meshResolution - 1; i++, x++, v++)
                    {
                        cells[i] = new Cell { corner =
                        {
                            [0] = points[v + 0],
                            [1] = points[v + 1],
                            [2] = points[v + data.meshResolution + 1],
                            [3] = points[v + data.meshResolution],
                            [4] = points[v + data.meshResolution * data.meshResolution],
                            [5] = points[v + data.meshResolution * data.meshResolution + 1],
                            [6] = points[v + data.meshResolution * data.meshResolution + data.meshResolution + 1],
                            [7] = points[v + data.meshResolution * data.meshResolution + data.meshResolution],
                        }};
                    }
                }
            }
        }
        
        private void GetChunkCorners()
        {
            int r = data.meshResolution;
            outside = new Cell { corner =
                {
                    [0] = points[  0  ],
                    [1] = points[ r-1 ],
                    [2] = points[r*r-r],
                    [3] = points[r*r-1],
                    
                    [4] = points[   r*r*(r-1)   ],
                    [5] = points[r*r*(r-1)+(r-1)],
                    [6] = points[    r*r*r-r    ],
                    [7] = points[    r*r*r-1    ],
                },
            };
        }

        public void Polygonalize()
        {
            vertices.Clear();
            triangles.Clear();

            foreach (var cell in cells)
            {
                var edges = MarchingCubes.GetVertices(cell, data.localZero);

                if (edges == null) continue;

                foreach (var v in edges)
                {
                    vertices.Add(v);
                    triangles.Add(vertices.Count - 1);
                }
            }
        }

        public void Apply()
        {
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
        }

        private void OnDrawGizmosSelected()
        {
            //+ DRAW CHUNK BOUNDARY
            outside.DrawGizmo(Color.blue);

            //+ DRAW ALL CELL VERTICES
            // foreach (var v in cells.SelectMany(c => c.corner)) Gizmos.DrawSphere(v, 0.15f);

            //+ DRAW ALL CELL EDGES
            foreach (var c in cells) c.DrawGizmo(Color.gray);
        }
    }
}