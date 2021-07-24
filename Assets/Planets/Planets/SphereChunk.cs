using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MarchedRendering;
using TCM.NoiseGeneration;
using TCM.Tools;
using UnityEngine.Rendering;


namespace TCM.Planets
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class SphereChunk : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Mesh mesh;

        private Vector3 LocalUp, LocalAxis2, LocalAxis1;
        private Vector2 chunkOffset;
        private float chunkPercent;

        private Cell outside;
        private ChunkedSpherePlanet.Data data;

        public Range noiseRange;
        public float buildDelay;

        [Header("Gizmo Debugging")]
        [Min(0)] public int currentPoint;
        [Min(0)] public int currentCell;

        [SerializeField] private Cell[] cells;
        [SerializeField] private Vector4[] points;
        [SerializeField] private List<int> triangles;
        [SerializeField] private List<Vector3> vertices;

        //> INITIALIZATION
        public void Initialize(Vector2 index, Vector3 localUp, ref ChunkedSpherePlanet.Data data)
        {
            this.noiseRange = new Range();
            
            this.data = data;
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

            this.LocalUp = localUp;
            this.LocalAxis1 = new Vector3(LocalUp.y, LocalUp.z, LocalUp.x);
            this.LocalAxis2 = Vector3.Cross(LocalUp, LocalAxis1);
            
            cells = new Cell[(data.meshResolution - 1) * (data.meshResolution - 1) * (data.meshResolution - 1)];
            points = new Vector4[data.meshResolution * data.meshResolution * data.meshResolution];

            BuildVertices();
            BuildCells();
            CalculateOutside();
        }

        //> CALCULATE THE AVERAGE DIRECTION OF THE CHUNK
        private void BuildVertices()
        {
            // const int resolution = 2;

            //- Loop over every vertex
            for (int i = 0, z = 0; z < data.meshResolution; z++) {
                for (int y = 0; y < data.meshResolution; y++) {
                    for (int x = 0; x < data.meshResolution; i++, x++)
                    {
                        Vector2 percent = (new Vector2(x, y) / (data.meshResolution - 1) * chunkPercent) + chunkOffset;
                        Vector3 position3D = LocalUp + ((percent.x - 0.5f) * 2 * LocalAxis1) + ((percent.y - 0.5f) * 2 * LocalAxis2);

                        //- Better method for converting unit cube to unit sphere
                        float xSquared = position3D.x * position3D.x;
                        float ySquared = position3D.y * position3D.y;
                        float zSquared = position3D.z * position3D.z;
                        Vector4 position4D = new Vector4
                        {
                            // normalize to a sphere
                            x = position3D.x * Mathf.Sqrt(1f - (ySquared / 2f) - (zSquared / 2f) + (ySquared * zSquared / 3f)),
                            y = position3D.y * Mathf.Sqrt(1f - (xSquared / 2f) - (zSquared / 2f) + (xSquared * zSquared / 3f)),
                            z = position3D.z * Mathf.Sqrt(1f - (xSquared / 2f) - (ySquared / 2f) + (xSquared * ySquared / 3f)),
                        };

                        position4D *= (data.radius + z * data.chunkHeight / (data.meshResolution - 1));
                        position4D.w = 0.5f;
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
            for (int i = 0, v = 0, z = 0; (z < data.meshResolution - 1); z++, v += data.meshResolution) {
                for (int y = 0; (y < data.meshResolution - 1); y++, v++) {
                    for (int x = 0; (x < data.meshResolution - 1); i++, x++, v++)
                    {
                        cells[i] = new Cell { corner =
                        {
                            [0] = points[v + 0],
                            [1] = points[v + 1],
                            [2] = points[v + data.meshResolution],
                            [3] = points[v + data.meshResolution + 1],
                            
                            [4] = points[v + data.meshResolution * data.meshResolution],
                            [5] = points[v + data.meshResolution * data.meshResolution + 1],
                            [6] = points[v + data.meshResolution * data.meshResolution + data.meshResolution],
                            [7] = points[v + data.meshResolution * data.meshResolution + data.meshResolution + 1],
                        }};
                    }
                }
            }
        }
        
        private void CalculateOutside()
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
            //+ DRAW ALL CELL EDGES
            // foreach (var c in cells) c.DrawGizmo(Color.gray);
            
            //+ DRAW CURRENT SELECTED CELL & POINT
            // if (currentPoint < points.Length && currentCell < cells.Length)
            // {
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(points[currentPoint], 10f);
            //     cells[currentCell].DrawGizmo(Gizmos.color);
            // }
            
            //+ DRAW ALL CELL VERTICES
            // foreach (var v in cells.SelectMany(c => c.corner)) Gizmos.DrawSphere(v, 10f);


            //+ DRAW CHUNK BOUNDARY
            outside.DrawGizmo(Color.blue);
        }
    }
}