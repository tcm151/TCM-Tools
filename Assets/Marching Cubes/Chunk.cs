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
        [SerializeField] private Material material;
        
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Mesh mesh;
        
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<float> values;
        

        private int resolution;

        //> INITIALIZATION
        private void Initialize(int resolution, Vector3Int index)
        {
            this.resolution = resolution;
            
            this.meshFilter = GetComponent<MeshFilter>();
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.meshCollider = GetComponent<MeshCollider>();

            this.meshRenderer.sharedMaterial = material;
            this.mesh = meshFilter.sharedMesh = meshCollider.sharedMesh = new Mesh
            {
                name = $"Mesh {index.ToString()}",
                indexFormat = IndexFormat.UInt32,
            };

            vertices = new List<Vector3>(new Vector3[resolution * resolution * resolution]);
            values = new List<float>(new float[resolution * resolution * resolution]);
            triangles = new List<int>();
        }

        private void Polygonalize(float localZero)
        {
            //- Loop over every vertex
            for (int i = 0, t = 0, z = 0; (z < resolution - 1); z++) {
                for (int y = 0; (y < resolution - 1); y++) {
                    for (int x = 0; (x < resolution - 1); i++, x++)
                    {
                        float[] value = new float[8];
                        Vector3[] edge = new Vector3[12];
                        Vector3[] vertex = new Vector3[8];
                        
                        //- Determine the index into the edge table which tells us which vertices are inside of the surface
                        int configuration = 0;
                        if (values[0] < localZero) configuration |= 1;
                        if (values[1] < localZero) configuration |= 2;
                        if (values[2] < localZero) configuration |= 4;
                        if (values[3] < localZero) configuration |= 8;
                        if (values[4] < localZero) configuration |= 16;
                        if (values[5] < localZero) configuration |= 32;
                        if (values[6] < localZero) configuration |= 64;
                        if (values[7] < localZero) configuration |= 128;
                        
                        //- Cube is entirely in/out of the surface
                        if (MarchingCubes.EdgeTable[configuration] == 0) return;
                        
                        //- Find the vertices where the surface intersects the cube
                        if ((MarchingCubes.EdgeTable[configuration] & 0001) == 1) edge[00] = MarchingCubes.Interpolate(localZero, vertex[0], vertex[1], value[0], value[1]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0002) == 1) edge[01] = MarchingCubes.Interpolate(localZero, vertex[1], vertex[2], value[1], value[2]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0004) == 1) edge[02] = MarchingCubes.Interpolate(localZero, vertex[2], vertex[3], value[2], value[3]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0008) == 1) edge[03] = MarchingCubes.Interpolate(localZero, vertex[3], vertex[0], value[3], value[0]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0016) == 1) edge[04] = MarchingCubes.Interpolate(localZero, vertex[4], vertex[5], value[4], value[5]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0032) == 1) edge[05] = MarchingCubes.Interpolate(localZero, vertex[5], vertex[6], value[5], value[6]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0064) == 1) edge[06] = MarchingCubes.Interpolate(localZero, vertex[6], vertex[7], value[6], value[7]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0128) == 1) edge[07] = MarchingCubes.Interpolate(localZero, vertex[7], vertex[4], value[7], value[4]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0256) == 1) edge[08] = MarchingCubes.Interpolate(localZero, vertex[0], vertex[4], value[0], value[4]);
                        if ((MarchingCubes.EdgeTable[configuration] & 0512) == 1) edge[09] = MarchingCubes.Interpolate(localZero, vertex[1], vertex[5], value[1], value[5]);
                        if ((MarchingCubes.EdgeTable[configuration] & 1024) == 1) edge[10] = MarchingCubes.Interpolate(localZero, vertex[2], vertex[6], value[2], value[6]);
                        if ((MarchingCubes.EdgeTable[configuration] & 2048) == 1) edge[11] = MarchingCubes.Interpolate(localZero, vertex[3], vertex[7], value[3], value[7]);
                        
                    }
                }
            }
        }
    }
}
