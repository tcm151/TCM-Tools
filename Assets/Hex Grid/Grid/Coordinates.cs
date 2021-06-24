using UnityEngine;

namespace TCM.HexGrid
{
    [System.Serializable]
    public struct Coordinates
    {
        //- PRIVATE COORDS
        [SerializeField] private int x;
        [SerializeField] private int z;
        
        //- PUBLIC COORDS
        public int X => x;
        public int Z => z;
        public int Y => (-x - z);
        
        //> CONSTRUCTOR
        public Coordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        //> TO STRINGS
        public string ToStringSplit() => $"{X}\n {Y}\n {Z}";
        override public string ToString() => $"({X}, {Y}, {Z})";
        
        
        //> GET HEX COORDS FROM REGULAR GRID
        public static Coordinates FromOffset(int x, int z) => new Coordinates(x - z / 2, z);

        //> CALCULATE THE HEX COORDINATE FROM THE WORLD POSITION
        public static Coordinates FromPosition(Vector3 position)
        {
            // get raw position
            float xf = position.x / (Metrics.InnerRadius * 2f);
            float yf = -xf;

            // account for offset grid nature
            float offset = position.z / (Metrics.OuterRadius * 3f);
            xf -= offset;
            yf -= offset;

            // round to the nearest int
            int xi = Mathf.RoundToInt(xf);
            int yi = Mathf.RoundToInt(yf);
            int zi = Mathf.RoundToInt(-xf-yf);

            // fix for rounding errors
            if (xi + yi + zi != 0)
            {
                float xr = Mathf.Abs(xf - xi);
                float yr = Mathf.Abs(yf - yi);
                float zr = Mathf.Abs(-xf - yf - zi);

                if (xr > yr && xr > zr) xi = -yi - zi;

                else if (zr > yr) zi = -xi - yi;
            }
            
            return new Coordinates(xi, zi);
        }
    }
}