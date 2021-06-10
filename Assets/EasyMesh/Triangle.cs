namespace EasyMesh
{
    public class Triangle
    {
        public Vertex v1 {get; set;}
        public Vertex v2 {get; set;}
        public Vertex v3 {get; set;}

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    public class TriangleExtensions
    {
        
    }
}