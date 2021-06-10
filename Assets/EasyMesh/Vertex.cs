using UnityEngine;


namespace EasyMesh
{
    public struct Vertex
    {
        public int index {get; set;}
        public float x {get; set;}
        public float y {get; set;}
        public float z {get; set;}

        // public int index {get;}

        public UnityEngine.Vector3 ToVector3() => new UnityEngine.Vector3(this.x, this.y, this.z);

        #region Constructors
        public Vertex(float x, float y, float z, int index = -1)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Vertex(UnityEngine.Vector3 v3, int index = -1)
        {
            this.index = index;
            this.x = v3.x;
            this.y = v3.y;
            this.z = v3.z;
        }
        
        public Vertex(Vector2 v2, float z, int index = -1)
        {
            this.index = index;
            this.x = v2.x;
            this.y = v2.y;
            this.z = z;
        }
        #endregion

        #region Operators
        public static Vertex operator +(Vertex a, Vertex b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
            return a;
        }
        
        public static Vertex operator -(Vertex a, Vertex b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            return a;
        }
        
        public static Vertex operator *(Vertex a, float s)
        {
            a.x *= s;
            a.y *= s;
            a.z *= s;
            return a;
        }
        
        public static Vertex operator /(Vertex a, float s)
        {
            a.x /= s;
            a.y /= s;
            a.z /= s;
            return a;
        }
        #endregion
    }
}
