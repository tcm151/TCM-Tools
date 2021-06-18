using System;
using UnityEngine;


namespace TCM.Planets
{
    public class WireChunk : MonoBehaviour
    {
        private Vector3 Up, Forward, Right;
        private Vector2 chunkOffset;
        private float chunkPercent;

        private WirePlanet.Data planetData;
        private Vector3[] vertices = new Vector3[8];

        private bool initialized = false;

        //> INITIALIZATION
        public void Initialize(int x, int y, Vector3 up, ref WirePlanet.Data planetData)
        {
            this.chunkPercent = (1f / planetData.chunkResolution);
            this.chunkOffset  = (new Vector2(x, y) / planetData.chunkResolution);
            this.planetData   = planetData;

            this.Up      = up;
            this.Right   = new Vector3(Up.y, Up.z, Up.x);
            this.Forward = Vector3.Cross(Up, Right);

            CalculateVertices();

            initialized = true;
        }

        //> CALCULATE THE AVERAGE DIRECTION OF THE CHUNK
        private void CalculateVertices()
        {
            const int resolution = 2;

            //- Loop over every vertex
            for (int i = 0, z = 0; z < resolution; z++) {
                for (int y = 0; y < resolution; y++) {
                    for (int x = 0; x < resolution; x++, i++)
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

                        vertices[i] = positionNormalized * (planetData.radius + z * planetData.chunkHeight);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!initialized) return;

            Gizmos.color = Color.red;
            
            foreach (Vector3 t in vertices) Gizmos.DrawSphere(t, 5f);

            Gizmos.DrawLine(vertices[0], vertices[1]);
            Gizmos.DrawLine(vertices[0], vertices[2]);
            Gizmos.DrawLine(vertices[2], vertices[3]);
            Gizmos.DrawLine(vertices[1], vertices[3]);
            
            Gizmos.DrawLine(vertices[0], vertices[0+4]);
            Gizmos.DrawLine(vertices[1], vertices[1+4]);
            Gizmos.DrawLine(vertices[2], vertices[2+4]);
            Gizmos.DrawLine(vertices[3], vertices[3+4]);
            
            Gizmos.DrawLine(vertices[0+4], vertices[1+4]);
            Gizmos.DrawLine(vertices[0+4], vertices[2+4]);
            Gizmos.DrawLine(vertices[2+4], vertices[3+4]);
            Gizmos.DrawLine(vertices[1+4], vertices[3+4]);
            
        }
    }
}