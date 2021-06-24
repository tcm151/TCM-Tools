using System.Collections.Generic;
using UnityEngine;


namespace TCM.HexGrid
{
    public static class Metrics
    {
        //> COLORS
        public static List<Color> colors = new List<Color>();

        //> PUBLIC MEASUREMENTS
        public const int   XChunks = 10, ZChunks = 10;
        public const float ElevationStep = 0.5f;
        public const float OuterRadius = 1.5f;
        public const float InnerRadius = OuterRadius * 0.866025404f;
        
        //> PRIVATE MEASUREMENTS
        private const float SolidFactor = 0.75f;
        private const float BlendFactor = 1f - SolidFactor;

        //> POSITION OF EACH CORNER
        private static readonly Vector3[] Corners =
        {
            new Vector3(0f, 0f, OuterRadius),
            new Vector3(InnerRadius, 0f, OuterRadius * 0.5f),
            new Vector3(InnerRadius, 0f, OuterRadius * -0.5f),
            new Vector3(0f, 0f, -OuterRadius),
            new Vector3(-InnerRadius, 0f, OuterRadius * -0.5f),
            new Vector3(-InnerRadius, 0f, OuterRadius * 0.5f),
            new Vector3(0f, 0f, OuterRadius), // circular repeat (REQUIRED)
        };

        public static float XPositionFromOffset(int x, int z) => (x + (0.5f * z) - (0.5f * z)) * (2f * InnerRadius);
        public static float ZPositionFromOffset(int z) => z * (1.5f * Metrics.OuterRadius);

        public static Vector3 GetCorner1(Direction direction) => Corners[(int)direction + 0];
        public static Vector3 GetCorner2(Direction direction) => Corners[(int)direction + 1];
        public static Vector3 GetSolidCorner1(Direction direction) => Corners[(int)direction + 0] * SolidFactor;
        public static Vector3 GetSolidCorner2(Direction direction) => Corners[(int)direction + 1] * SolidFactor;
        
        public static Vector3 GetBridge(Direction direction) => (Corners[(int)direction] + Corners[(int)direction + 1]) * BlendFactor;
    }
    
    public enum Direction
    {
        NE, E, SE, SW, W, NW,
    }

    public static class HexDirectionExtensions
    {
        public static Direction Opposite(this Direction direction) => (int)direction < 3 ? (direction + 3) : (direction - 3);
        public static Direction Prev(this Direction direction) => direction == Direction.NE ? Direction.NW : (direction - 1);
        public static Direction Next(this Direction direction) => direction == Direction.NW ? Direction.NE : (direction + 1);
    }
}