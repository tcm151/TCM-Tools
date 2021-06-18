using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace TCM.MarchingCubes
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {

        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Material material;
        private Mesh mesh;

        [SerializeField] public Vector4[] points;
        [SerializeField] public Cell[] cells;

        [SerializeField] private List<Vector3> vertices;
        [SerializeField] private List<int> triangles;

        private Vector3[] corners = new Vector3[8];

        private float localZero, chunkPercent;
        private int resolution, size;

        private Vector3 index;
        private Vector3 chunkOffset;

        //> INITIALIZATION
        public void Initialize(Vector3 index, int size, int chunkResolution, int resolution, float localZero, Material material)
        {
            this.index = index;
            
            this.size = size;
            this.localZero = localZero;
            this.resolution = resolution;
            
            this.material = material;
            
            this.chunkPercent = (1f / chunkResolution);
            this.chunkOffset = (index / chunkResolution);
            
            triangles = new List<int>();
            vertices = new List<Vector3>();
            
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshCollider = GetComponent<MeshCollider>();

            this.meshRenderer.sharedMaterial = material;
            this.mesh = meshFilter.sharedMesh = meshCollider.sharedMesh = new Mesh
            {
                name = $"Mesh {index.ToString()}",
                indexFormat = IndexFormat.UInt32,
            };

            cells = new Cell[(resolution - 1) * (resolution - 1) * (resolution - 1)];
            points = new Vector4[resolution * resolution * resolution];

            BuildVertices();
            BuildCells();
            AssignChunkCorners();
            // Polygonalize();
        }

        private void BuildVertices()
        {
            //- Loop over every vertex
            for (int i = 0, y = 0; y < resolution; y++) {
                for (int z = 0; z < resolution; z++) {
                    for (int x = 0; x < resolution; i++, x++)
                    {
                        Vector3 percent = (new Vector3(x, y, z) / (resolution - 1) * chunkPercent) + chunkOffset;
                        Vector3 position3D = (percent - 0.5f * Vector3.one) * size;
                        Vector4 position4D = position3D;
                        position4D.w = position3D.magnitude;
                        
                        points[i] = position4D;
                    }
                }
            }
        }

        private void AssignChunkCorners()
        {
            int r = resolution;
            
            corners[0] = points[  0  ];
            corners[1] = points[ r-1 ];
            corners[2] = points[r*r-r];
            corners[3] = points[r*r-1];
            
            corners[4] = points[   r*r*(r-1)   ];
            corners[5] = points[r*r*(r-1)+(r-1)];
            corners[6] = points[    r*r*r-r    ];
            corners[7] = points[    r*r*r-1    ];
        }

        private void BuildCells()
        {
            //- Loop over every vertex - 1
            for (int i = 0, v = 0, y = 0; y < resolution - 1; y++, v += resolution) {
                for (int z = 0; z < resolution - 1; z++, v++) {
                    for (int x = 0; x < resolution - 1; i++, x++, v++)
                    {
                        cells[i] = new Cell { corner =
                        {
                            [0] = points[v + 0],
                            [1] = points[v + 1],
                            [2] = points[v + resolution + 1],
                            [3] = points[v + resolution],
                            [4] = points[v + resolution * resolution],
                            [5] = points[v + resolution * resolution + 1],
                            [6] = points[v + resolution * resolution + resolution + 1],
                            [7] = points[v + resolution * resolution + resolution],
                        }};
                    }
                }
            }
        }

        public void Polygonalize()
        {
            vertices.Clear();
            triangles.Clear();

            foreach (var cell in cells)
            {
                var edges = MarchingCubes.GetVertices(cell, localZero);

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
            // for (int index = 0; index < corners.Length; index++)
            // {
            //     // Gizmos.color = Color.Lerp(Color.white, Color.black, (float) index / vertices.Count);
            //     Gizmos.color = Color.LerpUnclamped(Color.white, Color.black, corners[index].w);
            //     Gizmos.DrawSphere(corners[index], 0.025f);
            // }

            // Gizmos.color = Color.red;
            // foreach (var v in vertices)
            // {
            //     Gizmos.DrawSphere(v, 0.025f);
            // }

            // foreach (var v in cells.SelectMany(c => c.corner))
            // {
            //     Gizmos.DrawSphere(v, 0.15f);
            // }
            //
            // foreach (var cell in cells)
            // {
            //     Gizmos.DrawLine(cell.corner[0], cell.corner[1]);
            //     Gizmos.DrawLine(cell.corner[1], cell.corner[3]);
            //     Gizmos.DrawLine(cell.corner[3], cell.corner[2]);
            //     Gizmos.DrawLine(cell.corner[2], cell.corner[0]);
            //     
            //     Gizmos.DrawLine(cell.corner[0], cell.corner[4]);
            //     Gizmos.DrawLine(cell.corner[1], cell.corner[5]);
            //     Gizmos.DrawLine(cell.corner[3], cell.corner[7]);
            //     Gizmos.DrawLine(cell.corner[2], cell.corner[6]);
            //     
            //     Gizmos.DrawLine(cell.corner[4], cell.corner[5]);
            //     Gizmos.DrawLine(cell.corner[5], cell.corner[7]);
            //     Gizmos.DrawLine(cell.corner[7], cell.corner[6]);
            //     Gizmos.DrawLine(cell.corner[6], cell.corner[4]);
            // }
            //
            // Gizmos.color = Color.red;
            // foreach (var v in cells[cellChoice].corner)
            // {
            //     Gizmos.DrawSphere(v, 0.2f);
            // }
            
            Gizmos.color = Color.white;
            int r = resolution;
            
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[3]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[2], corners[0]);
            
            Gizmos.DrawLine(corners[0], corners[4]);
            Gizmos.DrawLine(corners[1], corners[5]);
            Gizmos.DrawLine(corners[2], corners[6]);
            Gizmos.DrawLine(corners[3], corners[7]);
            
            Gizmos.DrawLine(corners[4], corners[5]);
            Gizmos.DrawLine(corners[5], corners[7]);
            Gizmos.DrawLine(corners[6], corners[7]);
            Gizmos.DrawLine(corners[6], corners[4]);
            
            
            
            
            // Gizmos.color = Color.green;
            // Gizmos.DrawSphere(cellCorners[vertexChoice], 0.25f);
            //
            // Gizmos.color = Color.blue;
            // Gizmos.DrawSphere(cells[cellChoice].corner[cellVertexChoice], 0.3f);
        }
    }
}